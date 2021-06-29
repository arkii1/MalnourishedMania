using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class PlatformController : RaycastController
    {
        public LayerMask passengerMask;

        [Header("Saw properties")]
        [SerializeField] float speed;
        [SerializeField] bool cyclic = false;
        [SerializeField] float waitTime;
        [Range(0,2)]
        [SerializeField] float easeAmount;

        [Header("Chain properties")]
        [SerializeField] GameObject chainPrefab;
        [SerializeField] float chainSpacing = 1f;

        [Header("Waypoints")]
        [SerializeField] Vector3[] localWaypoints;
        Vector3[] globalWaypoints;

        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;

        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, PlayerController> passengerDictionary = new Dictionary<Transform, PlayerController>();

        MovingPlatformAnimatorSystem movingPlatformAnimatorSystem;

        public override void Start()
        {
            base.Start();

            CalculateGlobalWaypoints();
            SpawnChains();
            InitAnimatorSystem();
        }

        private void CalculateGlobalWaypoints()
        {
            globalWaypoints = new Vector3[localWaypoints.Length];

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }
        }

        void SpawnChains()
        {
            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                if (i == globalWaypoints.Length - 1)
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

        private void InitAnimatorSystem()
        {
            movingPlatformAnimatorSystem = gameObject.AddComponent<MovingPlatformAnimatorSystem>();
            movingPlatformAnimatorSystem.Init();
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();

            Vector3 velocity = CalculatePlatformMovement();

            if(velocity != Vector3.zero)
            {
                CalculatePassengerMovement(velocity);

                MovePassengers(true);
                transform.Translate(velocity);
                MovePassengers(false);
                
                movingPlatformAnimatorSystem.ChangeAnimationState(movingPlatformAnimatorSystem.on);
            }
            else
            {
                movingPlatformAnimatorSystem.ChangeAnimationState(movingPlatformAnimatorSystem.idle);
            }
        }

        Vector3 CalculatePlatformMovement()
        {
            if(Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }

            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length; ;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if(percentBetweenWaypoints >= 1)
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

        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        void CalculatePassengerMovement(Vector3 velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>(); //Hash sets are particularly fast at adding and checking if something is contained
            passengerMovement = new List<PassengerMovement>();


            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            //Vertically moving platform
            if (velocity.y != 0)
            {

                float rayLength = Mathf.Abs(velocity.y + skinWidth);
                rayLength = rayLength < 0.02f ? 0.02f : rayLength; //clamped as there are jitters if goes below this value

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength * directionY, Color.red);

                    if (hit)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            float pushX = (directionY == 1) ? velocity.x : 0;
                            float pushY = Mathf.Clamp(velocity.y < (hit.distance - skinWidth) ? velocity.y * directionY : (velocity.y - (hit.distance - skinWidth)) * directionY, 0.01f, 64f);

                            bool test = velocity.y < (hit.distance - skinWidth) ? true : false;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                            movedPassengers.Add(hit.transform);
                        }
                    }
                }
            }


            //Horizontally moving platform
            if (velocity.x != 0)
            {
                float rayLength = Mathf.Abs(velocity.x + skinWidth);

                if (Mathf.Abs(velocity.x) < skinWidth)
                {
                    rayLength = 2 * skinWidth;
                }

                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                    if (hit)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                            float pushY = -skinWidth;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                            movedPassengers.Add(hit.transform);
                        }
                    }
                }
            }

            //If passenger is on top of horizontally or downward moving platform
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {

                float rayLength = skinWidth * 2;
                rayLength = rayLength < 0.02f ? 0.02f : rayLength; //clamped as there are jitters if goes below this value

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = raycastOrigins.topLeft += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);


                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength * directionY, Color.blue);

                    if (hit)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));

                            movedPassengers.Add(hit.transform);
                        }
                    }
                }
            }
        }

        void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMovement passenger in passengerMovement)
            {
                if (!passengerDictionary.ContainsKey(passenger.transform))
                    passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<PlayerController>());

                if (passenger.moveBeforePlatform == beforeMovePlatform)
                {
                    passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform); //was move
                }
            }
        }

        public struct PassengerMovement
        {
            public Transform transform;
            public Vector3 velocity;
            public bool standingOnPlatform;
            public bool moveBeforePlatform;

            public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
            {
                transform = _transform;
                velocity = _velocity;
                standingOnPlatform = _standingOnPlatform;
                moveBeforePlatform = _moveBeforePlatform;
            }

        }

        private void OnDrawGizmos()
        {
            if(localWaypoints != null)
            {
                Gizmos.color = Color.red;
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
