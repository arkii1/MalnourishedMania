using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class CharmeleonTongue : RaycastController
    {
        [SerializeField] LayerMask playerMask;

        private void FixedUpdate()
        {
            if (GetAllCollisions(playerMask, 2 * skinWidth).Count > 0)
            {
                FindObjectOfType<PlayerManager>().Hit();
            }
        }

        private void Update()
        {
            Collider2D[] results = new Collider2D[5];
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = playerMask;
            int i = boxCollider.OverlapCollider(filter, results);
            if(i > 0)
            {
                foreach (Collider2D r in results)
                {
                    if (r)
                    {
                        if (r.gameObject.layer == playerMask)
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                        }
                        else if(r.gameObject.GetComponent<PlayerManager>() != null)
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                        }
                    }
                }
            }

            if (GetAllCollisions(playerMask, 2 * skinWidth).Count > 0)
            {
                FindObjectOfType<PlayerManager>().Hit();
            }
        }
    }
}

