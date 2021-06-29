using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Mushroom : RaycastController
    {
        [SerializeField] LayerMask playerMask;

        [Header("Properties")]
        [SerializeField] float speed;
        [SerializeField] float waitTimeMin = 0.5f, waitTimeMax = 3f;
        [SerializeField] float jumpForceOnKill;
        [SerializeField] float detectionRange = 10;

        [Header("Patrol waypoints")]
        [SerializeField] Vector3[] localWayPoints;

        bool hit = false;
        bool waiting = false;

        float timeToWait;
        float elapsed;

        Vector3 target;
        Vector3[] globalStopPoints;

        MushroomAnimatorSystem mushroomAnimatorSystem;
        SpriteRenderer sr;

        AudioSource hitAudio;

        public override void Start()
        {
            base.Start();

            sr = GetComponent<SpriteRenderer>();
            hitAudio = GetComponent<AudioSource>();

            InitAnimatorSystem();
            CalculateGlobalWayPoints();
            SetInitialTargetStopPoint();
        }

        private void InitAnimatorSystem()
        {
            mushroomAnimatorSystem = gameObject.AddComponent<MushroomAnimatorSystem>();
            mushroomAnimatorSystem.Init();
        }

        private void CalculateGlobalWayPoints()
        {
            globalStopPoints = new Vector3[localWayPoints.Length];

            for (int i = 0; i < globalStopPoints.Length; i++)
            {
                globalStopPoints[i] = localWayPoints[i] + transform.position;
            }
        }

        void SetInitialTargetStopPoint()
        {
            int randIndex = Random.Range(0, globalStopPoints.Length);
            target = globalStopPoints[randIndex];
        }

        private void Update()
        {
            if (hit)
                return;

            UpdateRayCastOrigins();

            if (SeesPlayer() && !FindObjectOfType<PlayerManager>().hit)
            {
                if (!BlockedByWall() && !AtEdgeOfPlatform())
                    ChasePlayer();
                else
                    mushroomAnimatorSystem.ChangeAnimationState(mushroomAnimatorSystem.idle, sr.flipX);
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

        bool SeesPlayer()
        {
            Vector3 vecToPlayer = FindObjectOfType<PlayerManager>().transform.position - transform.position;
            if(vecToPlayer.magnitude < detectionRange)
            {
                if(!Physics2D.Raycast(transform.position, vecToPlayer.normalized, vecToPlayer.magnitude, collisionMask))
                {
                    return true;
                }
            }

            return false;
        }

        bool BlockedByWall()
        {
            float targetX = FindObjectOfType<PlayerManager>().transform.position.x;
            Vector3 dirToRun = targetX > transform.position.x ? Vector3.right : Vector3.left;
            if (Physics2D.Raycast(transform.position + (dirToRun * boxCollider.bounds.extents.x), dirToRun, (Time.deltaTime * speed) - boxCollider.bounds.extents.x, collisionMask))
                return true;

            return false;
        }

        bool AtEdgeOfPlatform()
        {
            float targetX = FindObjectOfType<PlayerManager>().transform.position.x;
            Vector3 dirToRun = targetX > transform.position.x ? Vector3.right : Vector3.left;

            if(dirToRun == Vector3.right)
            {
                Vector3 origin = (Vector3)raycastOrigins.bottomRight + (dirToRun * speed * Time.deltaTime) + (dirToRun * skinWidth);
                if (!Physics2D.Raycast(origin, Vector3.down, 0.1f, collisionMask))
                    return true;
            }
            else
            {
                Vector3 origin = (Vector3)raycastOrigins.bottomLeft + (dirToRun * speed * Time.deltaTime) + (dirToRun * skinWidth);
                if (!Physics2D.Raycast(origin, Vector3.down, 0.1f, collisionMask))
                    return true;
            }

            return false;
        }

        void ChasePlayer()
        {
            if (BlockedByWall() || AtEdgeOfPlatform())
                return;

            float targetX = FindObjectOfType<PlayerManager>().transform.position.x;
            Vector3 dirToRun = targetX > transform.position.x ? Vector3.right : Vector3.left;
            sr.flipX = dirToRun.normalized == Vector3.right;
            mushroomAnimatorSystem.ChangeAnimationState(mushroomAnimatorSystem.run, sr.flipX);
            transform.Translate(dirToRun * Time.deltaTime * speed);

            if((FindObjectOfType<PlayerManager>().transform.position - transform.position).magnitude < 0.1f)
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
            mushroomAnimatorSystem.ChangeAnimationState(mushroomAnimatorSystem.idle, sr.flipX);
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

            sr.flipX = dir == Vector3.right;
            mushroomAnimatorSystem.ChangeAnimationState(mushroomAnimatorSystem.run, sr.flipX);
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
            mushroomAnimatorSystem.ChangeAnimationState(mushroomAnimatorSystem.hit, sr.flipX);
            FindObjectOfType<PlayerManager>().SetVerticalVelocity(jumpForceOnKill);
            hitAudio.Play();
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
            if (localWayPoints != null)
            {
                Gizmos.color = Color.black;
                float size = 0.3f;

                for (int i = 0; i < localWayPoints.Length; i++)
                {
                    Vector3 globalWaypointPosition = Application.isPlaying ? globalStopPoints[i] : localWayPoints[i] + transform.position;
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

