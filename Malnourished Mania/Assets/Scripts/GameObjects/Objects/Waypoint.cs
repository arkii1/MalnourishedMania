using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Waypoint : RaycastController
    {
        public LayerMask playerMask;
        public int index = 0; //higher means prioritised over others. So want check points near the end at a higher order
        public bool triggered = false;
        [HideInInspector]
        public Vector2 spawnPoint;

        WaypointAnimatorSystem waypointAnimatorSystem;
        [HideInInspector]
        public AudioSource audioSource;

        public override void Start()
        {
            base.Start();
            UpdateRayCastOrigins();
            spawnPoint = transform.position;

            audioSource = GetComponent<AudioSource>();

            waypointAnimatorSystem = gameObject.AddComponent<WaypointAnimatorSystem>();
            waypointAnimatorSystem.Init();

            FindObjectOfType<MonoLevelManager>().AddWaypoint(this);
        }

        public virtual void FixedUpdate()
        {
            if (triggered)
                return;

            if(UpCollisions() || LeftCollisions() || RightCollisions())
            {
                waypointAnimatorSystem.ChangeAnimationState(waypointAnimatorSystem.flagOut);

                FindObjectOfType<MonoLevelManager>().TriggerWaypoint(this);
                FindObjectOfType<MonoLevelManager>().BankFruit();
                triggered = true;

                audioSource.Stop();
                audioSource.Play();

            }
           
        }

        bool RightCollisions()
        {
            float rayLength = 0.1f;

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
            float rayLength = 0.1f;

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
            float rayLength = 0.1f;

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
    }
}

