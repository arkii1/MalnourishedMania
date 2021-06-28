using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Skull : RaycastController
    {
        public LayerMask playerMask;

        public Vector3[] localStopPoints;
        Vector3[] globalStopPoints;

        public float speed;
        public float enrageSpeedMult = 1.5f;
        public float waitTimeMin = 0.5f, waitTimeMax = 3f;
        public float jumpForceOnKill;
        public float detectionRange = 10;
        public float timeBetweenEnrage = 5f;
        public float timeEnraged = 7f;

        bool hit = false;
        bool waiting = false;
        bool enraged = false;

        Vector3 target;

        float timeToWait;
        float elapsed;
        float enrageElapsed;
        float timeBetweenEnrageElapsed;

        SkullAnimatorSystem skullAnimatorSystem;
        SpriteRenderer sr;

        public override void Start()
        {
            base.Start();
            skullAnimatorSystem = gameObject.AddComponent<SkullAnimatorSystem>();
            skullAnimatorSystem.Init();
            sr = GetComponent<SpriteRenderer>();

            CalculateGlobalStopPoints();
            SetInitialTargetStopPoint();
        }

        private void CalculateGlobalStopPoints()
        {
            globalStopPoints = new Vector3[localStopPoints.Length];

            for (int i = 0; i < globalStopPoints.Length; i++)
            {
                globalStopPoints[i] = localStopPoints[i] + transform.position;
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
                Debug.Log("hit by distance");
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

        void SetEnragedToFalse() //for end of derage animation
        {
            enraged = false;
        }

        void Enrage()
        {
            skullAnimatorSystem.ChangeAnimationState(skullAnimatorSystem.enrage, sr.flipX);
        }

        void SetEnragedToTrue()
        {
            enraged = true;
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
                Debug.Log("hit by distance");
            }
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

        private bool ShouldWait()
        {
            return Vector3.Distance(transform.position, target) < 0.1f && !waiting;
        }

        private void StartWaiting()
        {
            //start waiting
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

        void MoveToTarget()
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime * (enraged ? enrageSpeedMult : 1));

            sr.flipX = target.x < transform.position.x;
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

        List<RaycastHit2D> GetObstacle(Vector3 velocity)
        {
            List<RaycastHit2D> retList = new List<RaycastHit2D>();
            
            
            // /------------------------|
            ///-------------X--------|/ |
            //|            /         |  |
            //|           /          |  |
            //|          .           |  |
            //|                      |  |
            //|                      |  |
            //|                      | -
            //|----------------------|-
            
            //so, from dir start from transform and get the point on the collider that goes in that direction
            //then do a raycast from that point
          
            //



            return retList;
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
