using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Fan : RaycastController
    {
        public LayerMask affectedMask;
        public ParticleSystem particles;

        public float pushSpeed = 2;
        public float range = 10;

        FanAnimatorSystem fanAnimatorSystem;

        public override void Start()
        {
            base.Start();

            InitAnimatorSystem();

            CalculateStaticTrapDirCase();
        }

        private void InitAnimatorSystem()
        {
            fanAnimatorSystem = gameObject.AddComponent<FanAnimatorSystem>();
            fanAnimatorSystem.Init();
            fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.idle, false);
        }

        private void FixedUpdate()
        {
            List<RaycastHit2D> affectedCreatures = GetAffectedCreaturesList();

            if(affectedCreatures.Count > 0)
            {
                AffectCreatures(affectedCreatures);
                fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.triggered, false);
            }
            else
            {
                fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.idle, false);
            }
        }

        List<RaycastHit2D> GetAffectedCreaturesList()
        {
            switch (trapFacingDirection)
            {
                case TrapFacingDirection.up: return GetCollisionsAbove(affectedMask, range);
                case TrapFacingDirection.right: return GetCollisionsToTheRight(affectedMask, range);
                case TrapFacingDirection.down: return GetCollisionsBelow(affectedMask, range);
                case TrapFacingDirection.left: return GetCollisionsToTheLeft(affectedMask, range);
                default: return null;
            }
        }

        void AffectCreatures(List<RaycastHit2D> hitList)
        {
            HashSet<Transform> affectedTransforms = new HashSet<Transform>();

            for (int i = 0; i < hitList.Count; i++)
            {
                if (!affectedTransforms.Contains(hitList[i].transform))
                {
                    affectedTransforms.Add(hitList[i].transform);

                    Vector2 velocityToAdd = transform.up * pushSpeed * Time.deltaTime;
                    hitList[i].transform.GetComponent<CreatureManager>().AddVelocity(velocityToAdd);
                }
            }
        }

        public void PlayParticles()
        {
            if (particles.isPlaying)
                return;

            particles.Play();
        }

        public void StopParticles()
        {
           particles.Stop();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);

            Vector2 size = Vector3.Dot(Vector3.up, transform.up)  == 1 || Vector3.Dot(Vector3.up, transform.up)  == -1 ? new Vector2(1, range) : new Vector2(range, 1);

            Gizmos.DrawCube(transform.position + (transform.up * range / 2), size);

        }
    }
}

