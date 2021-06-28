using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Fire : RaycastController
    {
        public bool startDelay = false;
        public float delayAmount = 1f;
        public int delayNumber = 1;

        bool delayComplete = false;
        float delayElapsed = 0f;

        ParticleSystem particleSystem;
        public float timeBetweenBursts = 2.5f;
        float elapsed = 0f;
        bool isBursting = false;
        FireAnimatorSystem fireAnimatorSystem;

        AudioSource audioSource;

        public override void Start()
        {
            base.Start();
            particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
            particleSystem.Stop();
            fireAnimatorSystem = gameObject.AddComponent<FireAnimatorSystem>();
            fireAnimatorSystem.Init();
        }

        private void Update()
        {
            if(startDelay && !delayComplete)
            {
                if(delayElapsed < (delayAmount * delayNumber))
                {
                   delayElapsed += Time.deltaTime;
                   return;
                }
                else
                {
                    delayComplete = true;
                }
            }

            if(elapsed >= timeBetweenBursts && !isBursting)
            {
                fireAnimatorSystem.ChangeAnimationState(fireAnimatorSystem.shoot);
                isBursting = true;
            }

            elapsed += Time.deltaTime;
        }

        bool WithinDistanceToPlayer()
        {
            return (transform.position - FindObjectOfType<PlayerManager>().transform.position).magnitude <= 45.0f;
        }

        void Burst()
        {
            particleSystem.Play();
            Invoke("BurstEnded", particleSystem.main.duration);

            if (FindObjectOfType<CameraShake>() && GetComponent<SpriteRenderer>().isVisible)
                audioSource.Play();
        }

        void BurstEnded()
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

