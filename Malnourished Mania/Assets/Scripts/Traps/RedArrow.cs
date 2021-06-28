using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class RedArrow : RaycastController
    {
        public LayerMask creatureMask;

        public float timeDissapeared = 3;
        public float speedApplied = 15;
        public float rayLength = 0.1f;

        ArrowAnimatorSystem arrowAnimatorSystem;

        public override void Start()
        {
            base.Start();

            InitAnimatorSystem();
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
                creatures[i].transform.GetComponent<CreatureManager>().SetVelocity(transform.up * speedApplied);
                creatures[i].transform.GetComponent<CreatureManager>().ResetDoubleJump();

                arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.triggered);

                FindObjectOfType<CameraShake>().Trauma += 0.2f;
            }
        }

        public void EndArrowTriggeredAnimation()
        {
            SetGameObjectActiveToValue(false);
            arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.idle);
            Invoke("MakeArrowAppear", timeDissapeared);
        }

        void SetGameObjectActiveToValue(bool value)
        {
            gameObject.SetActive(value);
        }

        void MakeArrowAppear()
        {
            SetGameObjectActiveToValue(true);
            arrowAnimatorSystem.ChangeAnimationState(arrowAnimatorSystem.appear);
        }
    }
}