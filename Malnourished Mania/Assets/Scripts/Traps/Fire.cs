using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Fire : RaycastController
    {
        [SerializeField] bool startDelay = false;
        [SerializeField] float delayAmount = 1f;
        [SerializeField] float timeBetweenBursts = 2.5f;
        [SerializeField] int delayNumber = 1;

        bool isBursting = false;
        bool delayComplete = false;
        float delayElapsed = 0f;
        float elapsed = 0f;

        ParticleSystem particleSystem;
        FireAnimatorSystem fireAnimatorSystem;
        AudioSource audioSource;

        public override void Start()
        {
            base.Start();

            particleSystem = GetComponent<ParticleSystem>();
            particleSystem.Stop();

            audioSource = GetComponent<AudioSource>();

            InitAnimatorSystem();
        }

        private void InitAnimatorSystem()
        {
            fireAnimatorSystem = gameObject.AddComponent<FireAnimatorSystem>();
            fireAnimatorSystem.Init();
        }

        private void Update()
        {
            if (NeedToDelay())
            {
                HandleDelayTimer();
                return;
            }

            if (ShouldBurst())
            {
                TriggerBurstAnimation();
            }

            elapsed += Time.deltaTime;
        }

        private void TriggerBurstAnimation()
        {
            fireAnimatorSystem.ChangeAnimationState(fireAnimatorSystem.shoot);
            isBursting = true;
        }

        private bool ShouldBurst()
        {
            return elapsed >= timeBetweenBursts && !isBursting;
        }

        private bool NeedToDelay()
        {
            if(startDelay && delayElapsed < (delayAmount * delayNumber))
            {
                return true;
            }

            return false;
        }

        void HandleDelayTimer()
        {
            delayElapsed += Time.deltaTime;
        }

        void HandleBurst()
        {
            particleSystem.Play();
            Invoke("EndBurst", particleSystem.main.duration);

            if (GetComponent<SpriteRenderer>().isVisible)
                audioSource.Play();
        }

        void EndBurst()
        {
            particleSystem.Stop();
            elapsed = 0f;
            isBursting = false;
            fireAnimatorSystem.ChangeAnimationState(fireAnimatorSystem.idle);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("fire collieded with player");
                FindObjectOfType<PlayerManager>().Hit();
            }
        }
    }
}

