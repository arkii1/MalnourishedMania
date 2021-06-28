using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Saw : RaycastController
    {
        public LayerMask playerMask;
        public GameObject chainPrefab;
        public float chainSpacing;

        public bool cyclic = false;
        public bool isStationary = true;
        public bool rotateRightIfStationary = false;

        public float speed;
        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;

        public Vector3[] localWaypoints;
        Vector3[] globalWaypoints;


        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;

        int rotationDir = 0;
        int lastRotationDir = 1;

        SawAnimatorSystem sawAnimatorSystem;

        public override void Start()
        {
            base.Start();

            sawAnimatorSystem = gameObject.AddComponent<SawAnimatorSystem>();
            sawAnimatorSystem.Init();

            globalWaypoints = new Vector3[localWaypoints.Length];

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }

            SpawnChains();
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
            

            if(isStationary)
            {
                if(rotateRightIfStationary)
                    sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateRight);
                else
                    sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateLeft);
            }
            else if (rotationDir == 0)
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.idle);
            else if(rotationDir == 1)
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateRight);
            else
                sawAnimatorSystem.ChangeAnimationState(sawAnimatorSystem.rotateLeft);
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();
            if (UpCollisions() || DownCollisions() || LeftCollisions() || RightCollisions())
                HitPlayer();
        }

        void HitPlayer()
        {
            FindObjectOfType<PlayerManager>().Hit();
        }

        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        bool RightCollisions()
        {
            float rayLength = 0.02f;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topRight + Vector2.down * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, playerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * rayLength, Color.red);

                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        bool LeftCollisions()
        {
            float rayLength = 0.02f;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.down * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayLength, playerMask);

                Debug.DrawRay(rayOrigin, Vector2.left * rayLength, Color.red);

                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        bool UpCollisions()
        {
            float rayLength = 0.02f;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        bool DownCollisions()
        {
            float rayLength = 0.02f;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, playerMask);

                Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        Vector3 CalculateSawMovement()
        {
            if (isStationary)
                return Vector3.zero;

            if (Time.time < nextMoveTime)
            {
                rotationDir = 0;
                return Vector3.zero;
            }

            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

            if (globalWaypoints[toWaypointIndex].x - globalWaypoints[fromWaypointIndex].x > 0)
            {
                rotationDir = 1;
                lastRotationDir = 1;
            }
            else if (globalWaypoints[toWaypointIndex].x - globalWaypoints[fromWaypointIndex].x < 0)
            {
                rotationDir = -1;
                lastRotationDir = -1;
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

