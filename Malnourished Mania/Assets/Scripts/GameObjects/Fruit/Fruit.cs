using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Fruit : RaycastController
    {
        public FruitType fruitType;

        public LayerMask affectedMask;

        float rayLength = 0.02f + skinWidth;

        public bool collected = false;

        FruitAnimatorSystem fruitAnimatorSystem;
        AudioSource audioSource;

        public int index;

        public override void Start()
        {
            base.Start();
            InitAnimSystem();
            audioSource = GetComponent<AudioSource>();
        }

        void InitAnimSystem()
        {
            fruitAnimatorSystem = gameObject.AddComponent<FruitAnimatorSystem>();
            fruitAnimatorSystem.Init();

            fruitAnimatorSystem.ChangeAnimationState(fruitAnimatorSystem.idle);
        }

        private void FixedUpdate()
        {
            if (collected)
                return;

            if(HitPlayer(GetHitList()))
            {
                AddFruitToPlayer();
                
                fruitAnimatorSystem.ChangeAnimationState(fruitAnimatorSystem.collected);
                collected = true;
            }
        }

        bool HitPlayer(List<RaycastHit2D> hitList)
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                if (hitList[i].transform.CompareTag("Player"))
                    return true;
            }

            return false;
        }

        List<RaycastHit2D> GetHitList()
        {
            return GetAllCollisions(affectedMask, rayLength);
        }

        void AddFruitToPlayer()
        {
            FindObjectOfType<MonoLevelManager>().AddCarryingFruit(this);
            FindObjectOfType<InGameUI>().CollectedFruit(fruitType);
            audioSource.Play();
        }

        public void DisableSpriteRenderer()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        public void EnableSpriteRenderer()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            collected = false;
            fruitAnimatorSystem.ChangeAnimationState(fruitAnimatorSystem.idle);
        }
    }

    public enum FruitType
    {
        banana, kiwi, melon, pineapple, strawberry
    }
}

