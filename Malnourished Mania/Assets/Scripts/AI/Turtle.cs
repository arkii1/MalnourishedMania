using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Turtle : RaycastController
    {
        [SerializeField] LayerMask playerMask;
        [SerializeField] float timeBetweenSpikesChange = 2.5f;
        [SerializeField] float jumpForceOnKill = 30;

        bool spikesIn = true;
        bool hit = false;

        float elapsed = 0;

        TurtleAnimatorSystem turtleAnimatorSystem;
        SpriteRenderer sr;

        public override void Start()
        {
            base.Start();
            InitAnimatorSystem();

            sr = GetComponent<SpriteRenderer>();
        }

        private void InitAnimatorSystem()
        {
            turtleAnimatorSystem = gameObject.AddComponent<TurtleAnimatorSystem>();
            turtleAnimatorSystem.Init();
        }

        private void Update()
        {
            if (NeedToAlternateSpikes())
            {
                AlternateSpikes();
            }

            elapsed += Time.deltaTime;
        }

        bool NeedToAlternateSpikes()
        {
            return elapsed >= timeBetweenSpikesChange;
        }

        void AlternateSpikes()
        {
            if (spikesIn)
            {
                turtleAnimatorSystem.ChangeAnimationState(turtleAnimatorSystem.spikesOut, sr.flipX);
            }
            else
            {
                turtleAnimatorSystem.ChangeAnimationState(turtleAnimatorSystem.spikesIn, sr.flipX);
            }

            spikesIn = !spikesIn;
            elapsed = 0;
        }

        private void FixedUpdate()
        {
            if (!sr.enabled)
                return;

            if (spikesIn)
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
            }
            else
            {
                if(GetAllCollisions(playerMask, skinWidth * 2).Count > 0)
                {
                    FindObjectOfType<PlayerManager>().Hit();
                }
            }
        }

        private void Hit()
        {
            turtleAnimatorSystem.ChangeAnimationState(turtleAnimatorSystem.hit, sr.flipX);
            FindObjectOfType<PlayerManager>().SetVerticalVelocity(jumpForceOnKill);
            hit = true;
        }

        public void DisableGameObject()
        {
            hit = false;
            sr.enabled = false;
            FindObjectOfType<MonoLevelManager>().AddCreature(gameObject);
        }
    }
}

