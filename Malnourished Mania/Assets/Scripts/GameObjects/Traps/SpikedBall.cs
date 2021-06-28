using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class SpikedBall : RaycastController
    {
        public LayerMask playerMask;

        public override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            UpdateRayCastOrigins();

            if (GetAllCollisions(playerMask, 0.2f).Count > 0)
                FindObjectOfType<PlayerManager>().Hit();
        }
    }
}

