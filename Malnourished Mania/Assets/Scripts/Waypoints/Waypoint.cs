using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Waypoint : RaycastController
    {
        [HideInInspector]
        public AudioSource audioSource;
        [HideInInspector]
        public Vector2 spawnPoint;
        public int index = 0; //higher means prioritised over others. So want check points near the end at a higher order

        [SerializeField] LayerMask playerMask;
        [SerializeField] bool triggered = false;

        WaypointAnimatorSystem waypointAnimatorSystem;
       
        float rayLength = 0.1f;

        public override void Start()
        {
            base.Start();
            UpdateRayCastOrigins();
            spawnPoint = transform.position;

            audioSource = GetComponent<AudioSource>();

            InitAnimatorSystem();

            FindObjectOfType<MonoLevelManager>().AddWaypoint(this);
        }

        private void InitAnimatorSystem()
        {
            waypointAnimatorSystem = gameObject.AddComponent<WaypointAnimatorSystem>();
            waypointAnimatorSystem.Init();
        }

        public virtual void FixedUpdate()
        {
            if (triggered)
                return;

            if(GetCollisionsAbove(playerMask, rayLength).Count > 0 || GetCollisionsToTheLeft(playerMask, rayLength).Count > 0 || GetCollisionsToTheRight(playerMask, rayLength).Count > 0)
            {
                waypointAnimatorSystem.ChangeAnimationState(waypointAnimatorSystem.flagOut);

                FindObjectOfType<MonoLevelManager>().TriggerWaypoint(this);
                FindObjectOfType<MonoLevelManager>().BankFruit();
                triggered = true;

                audioSource.Stop();
                audioSource.Play();
            }
           
        }
    }
}

