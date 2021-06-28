using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Plant : RaycastController
    {
        public LayerMask playerMask;
        public float timeBetweenShots;
        public GameObject bulletPrefab;

        PlantAnimatorSystem plantAnimatorSystem;
        SpriteRenderer sr;
        AudioSource audioSource;

        public AudioClip shootClip;
        public AudioClip deathClip;


        float elapsed = 0;
        bool isShooting = false;
        bool hit = false;

        public float jumpForceOnKill = 5; 

        public override void Start()
        {
            base.Start();

            sr = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();

            plantAnimatorSystem = gameObject.AddComponent<PlantAnimatorSystem>();
            plantAnimatorSystem.Init();
            plantAnimatorSystem.ChangeAnimationState(plantAnimatorSystem.idle, sr.flipX);
        }

        private void Update()
        {
            if (hit)
                return;

            List<RaycastHit2D> aboveCollisions = GetCollisionsAbove(playerMask, 0.1f);
            if (aboveCollisions.Count > 0)
            {
                for (int i = 0; i < aboveCollisions.Count; i++)
                {
                    if (aboveCollisions[i].transform.CompareTag("Player"))
                    {
                        Hit();
                        hit = true;
                        return;
                    }
                }
            }

            if (isShooting)
                return;

            if(elapsed >= timeBetweenShots)
            {
                Shoot();
                isShooting = true;
                elapsed = 0f;
            }
            else
            {
                plantAnimatorSystem.ChangeAnimationState(plantAnimatorSystem.idle, sr.flipX);
                elapsed += Time.deltaTime;
            }
        }

        private void Hit()
        {
            plantAnimatorSystem.ChangeAnimationState(plantAnimatorSystem.hit, sr.flipX);
            FindObjectOfType<PlayerManager>().SetVerticalVelocity(jumpForceOnKill);
            PlayDeathAudio();
        }

        private void Shoot()
        {
            plantAnimatorSystem.ChangeAnimationState(plantAnimatorSystem.shoot, sr.flipX);

            if (FindObjectOfType<CameraShake>() && GetComponent<SpriteRenderer>().isVisible)
                PlayShootAudio();
        }


        public void InstantiateBullet()
        {
            GameObject bullet = Instantiate(bulletPrefab, FindObjectOfType<MonoLevelManager>().transform);
            bullet.GetComponent<Bullet>().right = sr.flipX;
            bullet.transform.position = sr.flipX ? new Vector2(0.384f, 0.169f) : new Vector2(-0.384f, 0.169f);
            bullet.transform.position += transform.position;
        }

        void FinishShot()
        {
            isShooting = false;
        }

        public void DisableGameObject()
        {
            gameObject.SetActive(false);
            FindObjectOfType<MonoLevelManager>().AddCreature(gameObject);
            isShooting = false;
            hit = false;
        }

        void PlayShootAudio()
        {
            if (audioSource.clip != shootClip)
                audioSource.clip = shootClip;

            audioSource.Play();
        }

        void PlayDeathAudio()
        {
            if (audioSource.clip != deathClip)
                audioSource.clip = deathClip;

            audioSource.Play();
        }
    }
}

