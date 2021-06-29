using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Trampoline : RaycastController
    {
        [SerializeField] LayerMask playerMask;

        [SerializeField] float upwardsVelocity = 10;
        [SerializeField] float rayLength = 0.02f;

        TrampolineAnimatorSystem trampolineAnimatorSystem;
        AudioSource audioSource;

        public override void Start()
        {
            base.Start();

            InitAnimatorSystem();

            CalculateStaticTrapDirCase();

            audioSource = GetComponent<AudioSource>();
        }

        private void InitAnimatorSystem()
        {
            trampolineAnimatorSystem = gameObject.AddComponent<TrampolineAnimatorSystem>();
            trampolineAnimatorSystem.Init();
        }

        private void FixedUpdate()
        {
            switch (trapFacingDirection)
            {
                case TrapFacingDirection.up: TriggerTrampoline(GetCollisionsAbove(playerMask, rayLength));
                    break;
                case TrapFacingDirection.right:
                    TriggerTrampoline(GetCollisionsToTheRight(playerMask, rayLength));
                    break;
                case TrapFacingDirection.down:
                    TriggerTrampoline(GetCollisionsBelow(playerMask, rayLength));
                    break;
                case TrapFacingDirection.left:
                    TriggerTrampoline(GetCollisionsToTheLeft(playerMask, rayLength));
                    break;
                default:
                    break;
            }
        }

        private void TriggerTrampoline(List<RaycastHit2D> hitList)
        {
            if(hitList.Count > 0)
            {
                HashSet<RaycastHit2D> alreadyAffectedSet = new HashSet<RaycastHit2D>();
                for (int i = 0; i < hitList.Count; i++)
                {
                    if (alreadyAffectedSet.Contains(hitList[i])) continue;

                    alreadyAffectedSet.Add(hitList[i]);

                    hitList[i].transform.GetComponent<CreatureManager>().SetVelocity(transform.up * upwardsVelocity);
                    hitList[i].transform.GetComponent<CreatureManager>().ResetDoubleJump();

                    audioSource.Play();

                    trampolineAnimatorSystem.ChangeAnimationState(trampolineAnimatorSystem.triggered, false);

                    if (FindObjectOfType<CameraShake>())
                        FindObjectOfType<CameraShake>().Trauma += 0.12f;
                }
                
            }
        }

        public void EndTriggeredAnimation()
        {
            trampolineAnimatorSystem.ChangeAnimationState(trampolineAnimatorSystem.idle, false);
        }
    }
}

