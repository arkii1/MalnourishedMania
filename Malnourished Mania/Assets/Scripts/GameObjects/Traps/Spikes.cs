﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Spikes : RaycastController
    {
        public LayerMask affectedMask;
        public float rayLength = 0.02f;

        public override void Start()
        {
            base.Start();

            CalculateStaticTrapDirCase();
        }

        private void FixedUpdate()
        {
            HitCollider(GetHitList());
        }


        List<RaycastHit2D> GetHitList()
        {
            return GetAllCollisions(affectedMask, rayLength + skinWidth);
        }

        void HitCollider(List<RaycastHit2D> hitList)
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                hitList[i].transform.GetComponent<CreatureManager>().Hit();
            }
        }
    }
}
