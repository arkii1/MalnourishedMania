using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Arrow : RaycastController
    {
        public LayerMask affectedMask;

        public float speedApplied = 15;
        public float _rayLength = 0.1f;

        ArrowAnimatorSystem arrowAnimatorSystem;

        AudioSource audioSource;

        public override void Start()
        {
            base.Start();

            arrowAnimatorSystem = gameObject.AddComponent<ArrowAnimatorSystem>();
            arrowAnimatorSystem.Init();

            audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            AffectHitObjects(GetHitList());
        }

        List<RaycastHit2D> GetHitList()
        {
            float rayLength = _rayLength + skinWidth;

            return GetAllCollisions(affectedMask, rayLength);
        }

        void AffectHitObjects(List<RaycastHit2D> hitList)
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                hitList[i].transform.GetComponent<CreatureManager>().SetVelocity(transform.up * speedApplied);
                hitList[i].transform.GetComponent<CreatureManager>().ResetDoubleJump();
                arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.triggered);
                audioSource.Play();
                if (FindObjectOfType<CameraShake>())
                    FindObjectOfType<CameraShake>().Trauma += 0.2f;
            }
        }

        public void EndArrowTriggeredAnimation()
        {
            arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.idle);
        }
    }
}
