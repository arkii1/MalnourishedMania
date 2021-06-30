using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class EndWaypoint : RaycastController
    {
        [HideInInspector]
        public bool hitPlayer = false;

        [SerializeField] LayerMask affectedMask;
        [SerializeField] GameObject fx;
        [SerializeField] float timeUntilTransition = 3;


        EndWaypointAnimtorSystem endWaypointAnimtorSystem;
        float rayLength = 0.02f + skinWidth;

        public override void Start()
        {
            base.Start();
            endWaypointAnimtorSystem = gameObject.AddComponent<EndWaypointAnimtorSystem>();
            endWaypointAnimtorSystem.Init();
        }

        private void FixedUpdate()
        {
            if (hitPlayer)
                return;
                
            if(HitPlayer(GetHitList()))
            {
                FindObjectOfType<MonoLevelManager>().BankFruit();
                FindObjectOfType<MonoLevelManager>().AddFruitToWallet();
                FindObjectOfType<MonoLevelManager>().UnlockNextLevel();
                TriggerCelebrations();
                hitPlayer = true;
                if (FindObjectOfType<CameraShake>())
                    FindObjectOfType<CameraShake>().Trauma += .75f;
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

        void TriggerCelebrations()
        {
            endWaypointAnimtorSystem.ChangeAnimationState(endWaypointAnimtorSystem.triggered);
            fx.SetActive(true);
            StartCoroutine(DelayThenTransitionOut());
        }

        IEnumerator DelayThenTransitionOut()
        {
            yield return new WaitForSeconds(timeUntilTransition);
            FindObjectOfType<MonoLevelManager>().TransitionOutOfLevel();
        }

    }
}

