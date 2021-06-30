using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Arrow : RaycastController
    {
        [SerializeField] LayerMask creatureMask;

        [SerializeField] float speedApplied = 15;
        [SerializeField] float rayLength = 0.1f;

        ArrowAnimatorSystem arrowAnimatorSystem;
        AudioSource audioSource;

        public override void Start()
        {
            base.Start();

            InitAnimatorSystem();

            audioSource = GetComponent<AudioSource>();
        }

        private void InitAnimatorSystem()
        {
            arrowAnimatorSystem = gameObject.AddComponent<ArrowAnimatorSystem>();
            arrowAnimatorSystem.Init();
        }

        private void FixedUpdate()
        {
            CheckIfTriggered();
        }

        void CheckIfTriggered()
        {
            List<RaycastHit2D> creatures = GetAllCollisions(creatureMask, rayLength + skinWidth);

            if (creatures.Count > 0)
            {
                TriggerArrow(creatures);
            }
        }

        void TriggerArrow(List<RaycastHit2D> creatures)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].transform.GetComponent<PlayerManager>().SetVelocity(transform.up * speedApplied);
                creatures[i].transform.GetComponent<PlayerManager>().ResetDoubleJump();

                arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.triggered);

                audioSource.Play();

                FindObjectOfType<CameraShake>().Trauma += 0.2f;
            }
        }

        public void EndArrowTriggeredAnimation()
        {
            arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.idle);
        }
    }
}
