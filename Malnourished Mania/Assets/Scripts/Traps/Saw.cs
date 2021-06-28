using UnityEngine;

enum SawRotationDirection
{
    left, right, none
}

namespace MalnourishedMania
{
    public class Saw : RaycastController
    {
        public LayerMask playerMask;

        [Header("Properties")]
        public bool cyclic = false;
        public bool isStationary = true;
        public bool rotateRightIfStationary = false;
        public float speed;
        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;

        [Header("Waypoints")]
        public Vector3[] localWaypoints;
        Vector3[] globalWaypoints;

        [Header("Chain properties")]
        public GameObject chainPrefab;
        public float chainSpacing;

        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;

        float rayLength = 0.02f;

        SawRotationDirection rotationDir = SawRotationDirection.none;
        SawRotationDirection lastRotationDir = SawRotationDirection.right;

        SawAnimatorSystem sawAnimatorSystem;

        public override void Start()
        {
            base.Start();

            CalculateGlobalWaypoints();

            InitAnimatorSystem();

            SpawnChains();
        }

        private void CalculateGlobalWaypoints()
        {
            globalWaypoints = new Vector3[localWaypoints.Length];

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }
        }

        private void InitAnimatorSystem()
        {
            sawAnimatorSystem = gameObject.AddComponent<SawAnimatorSystem>();
            sawAnimatorSystem.Init();
        }

        void SpawnChains()
        {
            if (isStationary)
                return;

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                if(i == globalWaypoints.Length - 1)
                {
                    if (cyclic)
                    {
                        Vector2 vecToNextPoint = globalWaypoints[0] - globalWaypoints[i];
                        float distance = vecToNextPoint.magnitude;
                        int noOfChains = Mathf.FloorToInt(distance / chainSpacing);
                        for (int j = 0; j < noOfChains; j++)
                        {
                            Vector2 spawnPos = (Vector2)globalWaypoints[i] + (chainSpacing * vecToNextPoint.normalized * j);
                            GameObject chain = Instantiate(chainPrefab, GameObject.FindGameObjectWithTag("Chains").transform);
                            chain.transform.position = spawnPos;
                        }
                    }
                    else
                    {
                        GameObject chainLast = Instantiate(chainPrefab, GameObject.FindGameObjectWithTag("Chains").transform);
                        chainLast.transform.position = (Vector2)globalWaypoints[i];
                    }
                }
                else
                {
                    Vector2 vecToNextPoint = globalWaypoints[i + 1] - globalWaypoints[i];
                    float distance = vecToNextPoint.magnitude;
                    int noOfChains = Mathf.FloorToInt(distance / chainSpacing);
                    for (int j = 0; j < noOfChains; j++)
                    {
                        Vector2 spawnPos = (Vector2)globalWaypoints[i] + (chainSpacing * vecToNextPoint.normalized * j);
                        GameObject chain = Instantiate(chainPrefab, GameObject.FindGameObjectWithTag("Chains").transform);
                        chain.transform.position = spawnPos;
                    }
                }
            }
        }

        private void Update()
        {
            transform.Translate(CalculateSawMovement());
            HandleAnimation();
        }

        Vector3 CalculateSawMovement()
        {
            if (isStationary)
                return Vector3.zero;

            if (NeedToWait())
            {
                rotationDir = SawRotationDirection.none;
                return Vector3.zero;
            }

            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

            if (globalWaypoints[toWaypointIndex].x - globalWaypoints[fromWaypointIndex].x > 0)
            {
                rotationDir = SawRotationDirection.right;
                lastRotationDir = SawRotationDirection.right;
            }
            else if (globalWaypoints[toWaypointIndex].x - globalWaypoints[fromWaypointIndex].x < 0)
            {
                rotationDir = SawRotationDirection.left;
                lastRotationDir = SawRotationDirection.left;
            }
            else rotationDir = lastRotationDir;

            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;

                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        System.Array.Reverse(globalWaypoints);
                    }
                }

                nextMoveTime = Time.time + waitTime;
            }

            return newPosition - transform.position;
        }

        private bool NeedToWait()
        {
            return Time.time < nextMoveTime;
        }

        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        private void HandleAnimation()
        {
            if (isStationary)
            {
                sawAnimatorSystem.ChangeAnimationState(rotateRightIfStationary ? sawAnimatorSystem.rotateRight : sawAnimatorSystem.rotateRight;);
            }
            else if (rotationDir == SawRotationDirection.none)
            {
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.idle);
            }
            else if (rotationDir == SawRotationDirection.right)
            {
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateRight);
            }
            else
            {
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateLeft);
            }
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();

            if (GetAllCollisions(playerMask, rayLength + skinWidth).Count > 0)
                HitPlayer();
        }

        void HitPlayer()
        {
            FindObjectOfType<PlayerManager>().Hit();
        }

        private void OnDrawGizmos()
        {
            if (isStationary)
                return;

            if (localWaypoints != null)
            {
                Gizmos.color = Color.black;
                float size = 0.3f;

                for (int i = 0; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPosition = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
                }
            }
        }

    }
}

