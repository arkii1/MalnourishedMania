using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Skull : RaycastController
    {
        [SerializeField] LayerMask playerMask;

        [Header("Skull properties")]
        [SerializeField] float speed;
        [SerializeField] float enrageSpeedMult = 1.5f;
        [SerializeField] float waitTimeMin = 0.5f, waitTimeMax = 3f;
        [SerializeField] float jumpForceOnKill;
        [SerializeField] float detectionRange = 10;
        [SerializeField] float timeBetweenEnrage = 5f;
        [SerializeField] float timeEnraged = 7f;

        [Header("Patrol waypoints")]
        [SerializeField] Vector3[] localStopPoints;
        Vector3[] globalStopPoints;

        Vector3 target;

        bool hit = false;
        bool waiting = false;
        bool enraged = false;

        float timeToWait;
        float elapsed;
        float enrageElapsed;
        float timeBetweenEnrageElapsed;

        SkullAnimatorSystem skullAnimatorSystem;
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
            skullAnimatorSystem = gameObject.AddComponent<SkullAnimatorSystem>();
            skullAnimatorSystem.Init();
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
            HandleEnrageMechanic();

            if (PlayerIsInDetectionRange() && !FindObjectOfType<PlayerManager>().hit)
            {
                if (enraged)
                    ChasePlayer();
                else
                    RunFromPlayer();
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

        private void HandleEnrageMechanic()
        {
            if (enraged)
            {
                if (enrageElapsed >= timeEnraged)
                {
                    Derage();
                    enrageElapsed = 0;
                }
                else
                {
                    enrageElapsed += Time.deltaTime;
                }
            }
            else
            {
                if (timeBetweenEnrageElapsed >= timeBetweenEnrage)
                {
                    Enrage();
                    timeBetweenEnrageElapsed = 0;
                }
                else
                {
                    timeBetweenEnrageElapsed += Time.deltaTime;
                }
            }
        }

        void Derage()
        {
            skullAnimatorSystem.ChangeAnimationState(skullAnimatorSystem.derage, sr.flipX);
        }

        void Enrage()
        {
            skullAnimatorSystem.ChangeAnimationState(skullAnimatorSystem.enrage, sr.flipX);
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
            sr.flipX = FindObjectOfType<PlayerManager>().transform.position.x < transform.position.x;
            skullAnimatorSystem.ChangeAnimationState(enraged ? skullAnimatorSystem.idleEnrage : skullAnimatorSystem.idle, sr.flipX);

            transform.Translate(targetVec.normalized * Time.deltaTime * speed * (enraged ? enrageSpeedMult : 1));

            if ((FindObjectOfType<PlayerManager>().transform.position - transform.position).magnitude < 0.1f)
            {
                FindObjectOfType<PlayerManager>().Hit();
            }
        }

        void RunFromPlayer()
        {
            Vector3 targetVec = -(FindObjectOfType<PlayerManager>().transform.position - transform.position);
            Vector3 dirToPlayer = targetVec.x > transform.position.x ? Vector3.right : Vector3.left;
            sr.flipX = FindObjectOfType<PlayerManager>().transform.position.x > transform.position.x;
            skullAnimatorSystem.ChangeAnimationState(enraged ? skullAnimatorSystem.idleEnrage : skullAnimatorSystem.idle, sr.flipX);
            transform.Translate(targetVec.normalized * Time.deltaTime * speed * (enraged ? enrageSpeedMult : 1));

            if ((FindObjectOfType<PlayerManager>().transform.position - transform.position).magnitude < 0.1f)
            {
                FindObjectOfType<PlayerManager>().Hit();
            }
        }

        void SetEnragedToFalse() //for end of derage animation
        {
            enraged = false;
        }

        void SetEnragedToTrue()
        {
            enraged = true;
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
            skullAnimatorSystem.ChangeAnimationState(enraged ? skullAnimatorSystem.idleEnrage : skullAnimatorSystem.idle, sr.flipX);
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
            transform.Translate(dir * speed * Time.deltaTime * (enraged ? enrageSpeedMult : 1));

            sr.flipX = target.x < transform.position.x;
            skullAnimatorSystem.ChangeAnimationState(enraged ? skullAnimatorSystem.idleEnrage : skullAnimatorSystem.idle, sr.flipX);
        }

        private void FixedUpdate()
        {
            if (hit)
                return;

            UpdateRayCastOrigins();
            if (AIIsHit())
            {
                if (enraged)
                {
                    FindObjectOfType<PlayerManager>().Hit();
                }
                else
                {
                    hit = true;
                    Hit();
                }
                
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
            skullAnimatorSystem.ChangeAnimationState(skullAnimatorSystem.hit, sr.flipX);
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
