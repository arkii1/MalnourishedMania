using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Charmeleon : RaycastController
    {
        [SerializeField] LayerMask playerMask;

        [Header("Properties")]
        [SerializeField] float speed;
        [SerializeField] float waitTimeMin = 0.5f, waitTimeMax = 3f;
        [SerializeField] float jumpForceOnKill = 30f;
        [SerializeField] float detectionRange = 10;
        [SerializeField] float attackRange = 2;
        [SerializeField] float timeBetweenAttacks = 1.5f;

        [Header("Patrol waypoints")]
        [SerializeField] Vector3[] localStopPoints;
        Vector3[] globalStopPoints;

        bool hit = false;
        bool waiting = false;
        bool attacking = false;

        float timeToWait;
        float waitElapsed;
        float elapsedSinceLastAttack;

        Vector3 target;

        CharmeleonAnimatorSystem charmeleonAnimator;
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
            charmeleonAnimator = gameObject.AddComponent<CharmeleonAnimatorSystem>();
            charmeleonAnimator.Init();
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
            CheckForCollisions();

            if (attacking)
                return;

            if (InAttackRange() && CanAttack())
            {
                Attack();
            }
            else if (SeesPlayer() && !FindObjectOfType<PlayerManager>().hit)
            {
                if (!InAttackRange() && !BlockedByWall() && !AtEdgeOfPlatform())
                    ChasePlayer();
                else
                    charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.idle, sr.flipX);
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

            HandleTimeBetweenAttacks();
        }

        bool InAttackRange()
        {
            Vector3 vecToPlayer = FindObjectOfType<PlayerManager>().transform.position - transform.position;
            return vecToPlayer.magnitude < attackRange;
        }

        bool CanAttack()
        {
            return elapsedSinceLastAttack >= timeBetweenAttacks;
        }

        void Attack()
        {
            charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.attack, sr.flipX);
            attacking = true;
        }

        bool SeesPlayer()
        {
            Vector3 vecToPlayer = FindObjectOfType<PlayerManager>().transform.position - transform.position;
            if (vecToPlayer.magnitude < detectionRange)
            {
                if (!Physics2D.Raycast(transform.position, vecToPlayer.normalized, vecToPlayer.magnitude, collisionMask))
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
            if (Physics2D.Raycast(transform.position, dirToRun, (Time.deltaTime * speed) - 2 * skinWidth, collisionMask))
                return true;

            return false;
        }

        bool AtEdgeOfPlatform()
        {
            float targetX = FindObjectOfType<PlayerManager>().transform.position.x;
            Vector3 dirToRun = targetX > transform.position.x ? Vector3.right : Vector3.left;

            if (dirToRun == Vector3.right)
            {
                Vector3 origin = (Vector3)raycastOrigins.bottomRight + (dirToRun * speed * Time.deltaTime) + (dirToRun * skinWidth);
                if (!Physics2D.Raycast(origin, Vector3.down, (Time.deltaTime * speed) - 2 * skinWidth, collisionMask))
                    return true;
            }
            else
            {
                Vector3 origin = (Vector3)raycastOrigins.bottomLeft + (dirToRun * speed * Time.deltaTime) + (dirToRun * skinWidth);
                if (!Physics2D.Raycast(origin, Vector3.down, (Time.deltaTime * speed) - 2 * skinWidth, collisionMask))
                    return true;
            }

            return false;
        }

        void ChasePlayer()
        {
            float targetX = FindObjectOfType<PlayerManager>().transform.position.x;
            Vector3 dirToRun = targetX > transform.position.x ? Vector3.right : Vector3.left;
            sr.flipX = dirToRun.normalized == Vector3.right;
            charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.run, sr.flipX);
            transform.Translate(dirToRun * Time.deltaTime * speed);

            if ((FindObjectOfType<PlayerManager>().transform.position - transform.position).magnitude < 0.1f)
            {
                FindObjectOfType<PlayerManager>().Hit();
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
            waitElapsed = 0;
        }

        float GetWaitTime()
        {
            return Random.Range(waitTimeMin, waitTimeMax);
        }

        private void Wait()
        {
            if (timeToWait <= waitElapsed)
            {
                waiting = false;
                target = CalculateNewTarget();
            }
            else
            {
                waitElapsed += Time.deltaTime;
            }
            charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.idle, sr.flipX);
        }

        void MoveToTarget()
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime);

            sr.flipX = dir == Vector3.right;
            charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.run, sr.flipX);
        }

        private void HandleTimeBetweenAttacks()
        {
            if (attacking)
                elapsedSinceLastAttack = 0;
            elapsedSinceLastAttack += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (hit)
                return;

            UpdateRayCastOrigins();
            CheckForCollisions();
        }

        private void CheckForCollisions()
        {
            if (GetCollisionsAbove(playerMask, skinWidth * 2).Count > 0)
            {
                if (FindObjectOfType<PlayerController>().collisions.below)
                {
                    FindObjectOfType<PlayerManager>().Hit();
                }
                else
                {
                    Hit();
                }
            }
            else if (GetCollisionsToTheLeft(playerMask, skinWidth * 2).Count > 0 || GetCollisionsToTheRight(playerMask, skinWidth * 2).Count > 0)
            {
                FindObjectOfType<PlayerManager>().Hit();
            }
        }

        private void Hit()
        {
            charmeleonAnimator.ChangeAnimationState(charmeleonAnimator.hit, sr.flipX);
            FindObjectOfType<PlayerManager>().SetVerticalVelocity(jumpForceOnKill);
            hit = true;
        }

        Vector3 CalculateNewTarget()
        {
            Vector3 retVal = Vector3.zero;

            int randIndex = Random.Range(0, globalStopPoints.Length);
            if (globalStopPoints[randIndex] != target)
                return globalStopPoints[randIndex];

            return CalculateNewTarget(); //horrible practice but shouldn't cause too much overflow
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

       


        public void EnableTongueHitBox()
        {
            transform.GetChild(0).localPosition = sr.flipX ? new Vector3(1.785f, 0, 0) : new Vector3(-0.655f, 0, 0);
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void DisableTongueHitBox()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            attacking = false;
        }


        public void DisableGameObject()
        {
            hit = false;
            gameObject.SetActive(false);
            FindObjectOfType<MonoLevelManager>().AddCreature(gameObject);
        }
    }

}
