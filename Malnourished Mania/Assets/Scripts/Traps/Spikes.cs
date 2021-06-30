using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Spikes : RaycastController
    {
        [SerializeField] LayerMask creatureMask;
        [SerializeField] float rayLength = 0.02f;

        public override void Start()
        {
            base.Start();
            CalculateStaticTrapDirCase();
        }

        private void FixedUpdate()
        {
            HitCollider(GetAllCollisions(creatureMask, rayLength + skinWidth));
        }

        void HitCollider(List<RaycastHit2D> hitList)
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                hitList[i].transform.GetComponent<PlayerManager>().Hit();
            }
        }
    }
}
