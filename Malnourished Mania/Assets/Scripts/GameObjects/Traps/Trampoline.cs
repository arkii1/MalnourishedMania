using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Trampoline : RaycastController
    {
        public LayerMask affectedMask;
        public float upwardsVelocity = 10;
        public float rayLength = 0.02f;

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
            switch (staticTrapDirCase)
            {
                case 0: AffectHitList(GetCollisionsAbove(affectedMask, rayLength));
                    break;
                case 1:
                    AffectHitList(GetCollisionsToTheRight(affectedMask, rayLength));
                    break;
                case 2:
                    AffectHitList(GetCollisionsBelow(affectedMask, rayLength));
                    break;
                case 3:
                    AffectHitList(GetCollisionsToTheLeft(affectedMask, rayLength));
                    break;
                default:
                    break;
            }
        }

        private void AffectHitList(List<RaycastHit2D> hitList)
        {
            if(hitList.Count > 0)
            {
                HashSet<RaycastHit2D> alreadyAffectedSet = new HashSet<RaycastHit2D>();
                for (int i = 0; i < hitList.Count; i++)
                {
                    if (alreadyAffectedSet.Contains(hitList[i])) continue;

                    alreadyAffectedSet.Add(hitList[i]);

                    hitList[i].transform.GetComponent<CreatureManager>().SetVelocity(transform.up * upwardsVelocity);
                    hitList[i].transform.GetComponent<CreatureManager>().ResetDoubleJump(); //FIX THIS
                    audioSource.Play();

                    trampolineAnimatorSystem.ChangeAnimationState(trampolineAnimatorSystem.triggered, false);

                    if (FindObjectOfType<CameraShake>())
                        FindObjectOfType<CameraShake>().Trauma += 0.12f;
                }
                
            }
        }

        List<RaycastHit2D> GetHitList()
        {
            return GetAllCollisions(affectedMask, rayLength + skinWidth);
        }
    

        public void EndTriggeredAnimation()
        {
            trampolineAnimatorSystem.ChangeAnimationState(trampolineAnimatorSystem.idle, false);
        }
    }
}

