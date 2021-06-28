using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Fan : RaycastController
    {
        public float pushSpeed = 2;
        public float range = 10;
        public LayerMask affectedMask;

        FanAnimatorSystem fanAnimatorSystem;

        public ParticleSystem particles;

        public override void Start()
        {
            base.Start();

            fanAnimatorSystem = gameObject.AddComponent<FanAnimatorSystem>();
            fanAnimatorSystem.Init();
            fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.idle, false);

            CalculateStaticTrapDirCase();
        }

        private void FixedUpdate()
        {
            List<RaycastHit2D> hitList = GetHitList();

            if(hitList.Count > 0)
            {
                AffectHitObjects(GetHitList());
                fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.triggered, false);
            }
            else
                fanAnimatorSystem.ChangeAnimationState(fanAnimatorSystem.idle, false);
        }

        List<RaycastHit2D> GetHitList()
        {
            switch (staticTrapDirCase)
            {
                case 0: return GetCollisionsAbove(affectedMask, range);
                case 1: return GetCollisionsToTheRight(affectedMask, range);
                case 2: return GetCollisionsBelow(affectedMask, range);
                case 3: return GetCollisionsToTheLeft(affectedMask, range);
                default: return null;
            }
        }

        void AffectHitObjects(List<RaycastHit2D> hitList)
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
            if (particles.isPlaying) return;
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

