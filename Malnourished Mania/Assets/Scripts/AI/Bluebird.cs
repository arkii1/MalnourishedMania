using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Bluebird : RaycastController
    {

        [SerializeField] LayerMask playerMask;

        [Header("Bluebird properties")]
        [SerializeField] float speed;
        [SerializeField] float waitTimeMin = 0.5f, waitTimeMax = 3f;
        [SerializeField] float jumpForceOnKill;
        [SerializeField] float detectionRange = 10;

        [Header("Waypoints")]
        [SerializeField] Vector3[] localStopPoints;
        Vector3[] globalStopPoints;

        bool hit = false;
        bool waiting = false;
        float timeToWait;
        float elapsed;

        Vector3 target;

        BluebirdAnimatorSystem bluebirdAnimatorSystem;
        SpriteRenderer sr;

        public override void Start()
        {
            base.Start();

            InitAnimatorSystem();
            sr = GetComponent<SpriteRenderer>();
            CalculateGlobalWayPoints();
            SetInitialTargetWayPoint();
        }

        private void InitAnimatorSystem()
        {
            bluebirdAnimatorSystem = gameObject.AddComponent<BluebirdAnimatorSystem>();
            bluebirdAnimatorSystem.Init();
        }

        private void CalculateGlobalWayPoints()
        {
            globalStopPoints = new Vector3[localStopPoints.Length];

            for (int i = 0; i < globalStopPoints.Length; i++)
            {
                globalStopPoints[i] = localStopPoints[i] + transform.position;
            }
        }

        void SetInitialTargetWayPoint()
        {
            int randIndex = Random.Range(0, globalStopPoints.Length);
            target = globalStopPoints[randIndex];
        }

        private void Update()
        {
            if (hit)
                return;

            UpdateRayCastOrigins();

            if (PlayerIsInDetectionRange() && !FindObjectOfType<PlayerManager>().hit)
            {
                ChasePlayer();
            }
            else if (ShouldWait())
            {
                StartWaiting();
            }
            else if (waiting)
            {
                Wait();
            }
            else
            {
                MoveToTarget();
            }
        }

        bool PlayerIsInDetectionRange()
        {
            Vector3 vecToPlayer = FindObjectOfType<PlayerManager>().transform.position - transform.position;
            return vecToPlayer.magnitude < detectionRange;
        }

        void ChasePlayer()
        {
            Vector3 targetVec = FindObjectOfType<PlayerManager>().transform.position - transform.position;
            Vector3 dirToPlayer = targetVec.x > transform.position.x ? Vector3.right : Vector3.left;

            sr.flipX = dirToPlayer.normalized == Vector3.right;

            bluebirdAnimatorSystem.ChangeAnimationState(bluebirdAnimatorSystem.flying, sr.flipX);

            transform.Translate(targetVec.normalized * Time.deltaTime * speed);

            if ((FindObjectOfType<PlayerManager>().transform.position - transform.position).magnitude < 0.1f)
            {
                FindObjectOfType<PlayerManager>().Hit();
                Debug.Log("hit by distance");
            }
        }

        private bool ShouldWait()
        {
            return Vector3.Distance(transform.position, target) < 0.1f && !waiting;
        }

        private void StartWaiting()
        {
            waiting = true;
            timeToWait = GetWaitTime();
            elapsed = 0;
        }

        float GetWaitTime()
        {
            return Random.Range(waitTimeMin, waitTimeMax);
        }

        private void Wait()
        {
            if (timeToWait <= elapsed)
            {
                waiting = false;
                target = CalculateNewTarget();
            }
            else
            {
                elapsed += Time.deltaTime;
            }
            bluebirdAnimatorSystem.ChangeAnimationState(bluebirdAnimatorSystem.flying, sr.flipX);
        }

        Vector3 CalculateNewTarget()
        {
            Vector3 retVal = Vector3.zero;

            int randIndex = Random.Range(0, globalStopPoints.Length);
            if (globalStopPoints[randIndex] != target)
                return globalStopPoints[randIndex];

            return CalculateNewTarget(); //horrible practice but shouldnt cause too much overflow at worst
        }

        void MoveToTarget()
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime);

            sr.flipX = target.x > transform.position.x;
            bluebirdAnimatorSystem.ChangeAnimationState(bluebirdAnimatorSystem.flying, sr.flipX);
        }


        private void FixedUpdate()
        {
            if (hit)
                return;

            UpdateRayCastOrigins();
            if (AIIsHit())
            {
                hit = true;
                Hit();
            }
            else if (HitPlayer())
            {
                FindObjectOfType<PlayerManager>().Hit();
            }

        }

        bool AIIsHit()
        {
            List<RaycastHit2D> aboveCollisions = GetCollisionsAbove(playerMask, 0.1f);
            if (aboveCollisions.Count > 0)
            {
                for (int i = 0; i < aboveCollisions.Count; i++)
                {
                    if (aboveCollisions[i].transform.CompareTag("Player"))
                    {
                        if (!aboveCollisions[i].transform.GetComponent<PlayerController>().collisions.below)
                            return true;
                    }
                }
            }

            return false;
        }

        private void Hit()
        {
            bluebirdAnimatorSystem.ChangeAnimationState(bluebirdAnimatorSystem.hit, sr.flipX);
            FindObjectOfType<PlayerManager>().SetVerticalVelocity(jumpForceOnKill);
        }


        bool HitPlayer()
        {
            if (waiting)
            {
                List<RaycastHit2D> hits = GetCollisionsToTheRight(playerMask, 2 * skinWidth);
                if (hits.Count > 0)
                {
                    for (int i = 0; i < hits.Count; i++)
                    {
                        if (hits[i].transform.CompareTag("Player"))
                        {
                            return true;
                        }
                    }
                }
                List<RaycastHit2D> hits2 = GetCollisionsToTheLeft(playerMask, 2 * skinWidth);
                if (hits2.Count > 0)
                {
                    for (int i = 0; i < hits2.Count; i++)
                    {
                        if (hits2[i].transform.CompareTag("Player"))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                Vector3 dir = (target - transform.position).normalized;
                if (dir == Vector3.right)
                {
                    List<RaycastHit2D> hits = GetCollisionsToTheRight(playerMask, speed * Time.deltaTime + 2 * skinWidth);
                    if (hits.Count > 0)
                    {
                        for (int i = 0; i < hits.Count; i++)
                        {
                            if (hits[i].transform.CompareTag("Player"))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    List<RaycastHit2D> hits = GetCollisionsToTheLeft(playerMask, speed * Time.deltaTime + 2 * skinWidth);
                    if (hits.Count > 0)
                    {
                        for (int i = 0; i < hits.Count; i++)
                        {
                            if (hits[i].transform.CompareTag("Player"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            return false;
        }

        private void OnDrawGizmos()
        {
            if (localStopPoints != null)
            {
                Gizmos.color = Color.black;
                float size = 0.3f;

                for (int i = 0; i < localStopPoints.Length; i++)
                {
                    Vector3 globalWaypointPosition = Application.isPlaying ? globalStopPoints[i] : localStopPoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
                }
            }
        }

        public void DisableGameObject()
        {
            hit = false;
            gameObject.SetActive(false);
            FindObjectOfType<MonoLevelManager>().AddCreature(gameObject);
        }
    }


}
