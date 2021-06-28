using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class FallingPlatformController : RaycastController
    {
        public ParticleSystem particles;
        public LayerMask passengerMask;

        public float initialFallMult = -9.8f;
        public float riseMult = 10;

        bool doingFallDelay = true;

        public float timeContinueFallingWithNoPassengers = 1f;

        float startingY = 0;

        Vector2 velocity = Vector2.zero;

        FallingPlatformAnimatorSystem fallingPlatformAnimatorSystem;

        float elapsedSincePassengerJoined = 0;
        float elapsedSincePassengerLeft = 0;

        public override void Start()
        {
            base.Start();

            fallingPlatformAnimatorSystem = gameObject.AddComponent<FallingPlatformAnimatorSystem>();
            fallingPlatformAnimatorSystem.Init();

            CalculateRaySpacing();

            startingY = transform.position.y;
            elapsedSincePassengerLeft = timeContinueFallingWithNoPassengers + 0.1f; //stops bug of platform dropping as soon as game starts
        }

        private void FixedUpdate()
        {

            //if got a passenger
                //if first contact && !already done fall delay
                    //do fall delay
                //else
                    //fall
            //...


            UpdateRayCastOrigins();

            List<Transform> passengers = GetPassengers();

            if (passengers.Count > 0) // we have a passenger
            {
                if (doingFallDelay)
                {
                    fallingPlatformAnimatorSystem.ChangeAnimationState(fallingPlatformAnimatorSystem.fallDelay);
                }
                else //fall
                {
                    fallingPlatformAnimatorSystem.ChangeAnimationState(fallingPlatformAnimatorSystem.off);

                    CalculateFallVelocity();
                    elapsedSincePassengerJoined += Time.deltaTime;
                    elapsedSincePassengerLeft = 0;
                }
            }
            else if (ShouldStillFall()) // small post passenger window to continue falling
            {   
           

                fallingPlatformAnimatorSystem.ChangeAnimationState(fallingPlatformAnimatorSystem.off);

                CalculateFallVelocity();
                elapsedSincePassengerJoined += Time.deltaTime;
                elapsedSincePassengerLeft += Time.deltaTime;
            }
            else if (!AtOriginalY()) // we aren't falling and have no passenger
            {
                fallingPlatformAnimatorSystem.ChangeAnimationState(fallingPlatformAnimatorSystem.on);

                CalculateRiseVelocity();
                elapsedSincePassengerJoined = 0;
                elapsedSincePassengerLeft += Time.deltaTime;
            }
            else // we are at starting point and need to idle
            {
                fallingPlatformAnimatorSystem.ChangeAnimationState(fallingPlatformAnimatorSystem.on);

                SetVelocityToZero();
                doingFallDelay = true;
            }

            if(velocity != Vector2.zero)
                transform.Translate(velocity);
            if (passengers.Count > 0)
                MovePassengers(passengers);

        }

        void CalculateFallVelocity()
        {
            velocity.y = EaseInExp(elapsedSincePassengerJoined) * -Mathf.Abs(initialFallMult) * Time.deltaTime;
        }
       
        void CalculateRiseVelocity()
        {
            velocity.y = EaseInOutQuad(elapsedSincePassengerLeft - timeContinueFallingWithNoPassengers) * Mathf.Abs(riseMult) * Time.deltaTime;
        }

        void MovePassengers(List<Transform> transforms)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                transforms[i].GetComponent<PlayerController>().Move(velocity, true) ;
            }
        }

        void SetVelocityToZero()
        {
            velocity = Vector2.zero;
        }

        bool ShouldStillFall()
        {
            if(elapsedSincePassengerLeft < timeContinueFallingWithNoPassengers)
            {
                return true;
            }

            return false;
        }

        public void StopDoingFalLDelay()
        {
            doingFallDelay = false;
        }

        bool AtOriginalY()
        {
            return transform.position.y >= startingY;
        }

        void HandleBelowCollisions()
        {
            float rayLength = Mathf.Abs(velocity.y + skinWidth);
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
                rayOrigin += Vector2.down * 0.01f; // this is because the ray kept hitting itself and theres no real way to ask the raycast to ignore this without changing the gameobjects layer so we start the raycast beyond the objects collider
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);


                if (hit && hit.transform != transform)
                {
                    Debug.DrawLine(rayOrigin, rayOrigin + Vector2.down * (hit.distance - skinWidth), Color.red);
                    velocity.y = hit.distance > 0 ? -(hit.distance - skinWidth) : 0; //move platfrom to hit area or stop if already cannot move more
                }
                else
                    Debug.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLength, Color.green);
            }
        }

        List<Transform> GetPassengers()
        {
            List<Transform> transformList = new List<Transform>();
            float rayLength = 0.1f;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    transformList.Add(hit.transform);
                }
            }

            return transformList;
        }

        float EaseInExp(float t)
        {
            return t*t;
        }

        float EaseInOutQuad(float t)
        {
            return 1 - Mathf.Cos((t * Mathf.PI) / 2);
        }

        public void PlayParticles()
        {
            if (particles.isPlaying) return;
            particles.Play();
        }

        public void StopParticles()
        {
            particles.Stop();
        }
    }
}

