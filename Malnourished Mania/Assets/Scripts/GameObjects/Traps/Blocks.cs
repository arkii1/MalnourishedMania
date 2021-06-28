using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Blocks : RaycastController
    {
        public bool triggered = false;
        public LayerMask characterMask;
        public float rayLength = 0.02f;
        public float timeUntilRespawn = 5;
        public float timeDelayInExternalTrigger = 0.5f;

        BlockAnimatorSystem blockAnimatorSystem;

        float timer = 0;


        public override void Start()
        {
            base.Start();
            blockAnimatorSystem = gameObject.AddComponent<BlockAnimatorSystem>();
            blockAnimatorSystem.Init();
        }

        private void FixedUpdate()
        {
            List<RaycastHit2D> hitList = GetAllCollisions(characterMask, rayLength + skinWidth);

            if (hitList.Count > 0)
            {
                if(!triggered)
                {
                    triggered = true;
                    Trigger();
                }
            }
            else if(!triggered)
                blockAnimatorSystem.ChangeAnimationState(blockAnimatorSystem.idle);

            if (triggered)
            {
                if(timer > timeUntilRespawn)
                {
                    timer = 0;
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    gameObject.GetComponent<BoxCollider2D>().enabled = true;
                    triggered = false;
                }
                else
                    timer += Time.deltaTime;
            }
        }

        void Trigger()
        {
            blockAnimatorSystem.ChangeAnimationState(blockAnimatorSystem.triggered);
            TriggerAdjactentBlocks();
        }

        public void TriggeredExternally()
        {
            if (triggered)
                return;

            triggered = true;
            Invoke("Trigger", timeDelayInExternalTrigger);
        }

        void TriggerAdjactentBlocks()
        {
            gameObject.layer = 2; //ignore raycast
            List<RaycastHit2D> hitList = GetAllCollisions(collisionMask, 0.1f + skinWidth);
            gameObject.layer = 9; //obstacle
            for (int i = 0; i < hitList.Count; i++)
            {
                if(hitList[i].transform.GetComponent<Blocks>() != null && hitList[i].transform != this.transform)
                {
                    hitList[i].transform.GetComponent<Blocks>().TriggeredExternally();
                }
            }
        }

        public void FinishedAnimation()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
