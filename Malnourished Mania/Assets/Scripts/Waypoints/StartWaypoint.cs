using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class StartWaypoint : Waypoint
    {
        public override void Start()
        {
            CalculateRaySpacing();
            UpdateRayCastOrigins();
            spawnPoint = transform.position;

            audioSource = GetComponent<AudioSource>();

            FindObjectOfType<MonoLevelManager>().AddWaypoint(this);
        }

        public override void FixedUpdate() // this is not used so waypoint fixed update doesn't get called
        {

        }

        public void Trigger()
        {
            audioSource.Stop();
            audioSource.Play();
            FindObjectOfType<MonoLevelManager>().TriggerWaypoint(this);
            GetComponent<Animator>().SetTrigger("triggered");
        }
    }
}

