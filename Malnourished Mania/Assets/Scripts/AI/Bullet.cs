using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Bullet : RaycastController
    {
        [HideInInspector] public bool right = true;

        [SerializeField] LayerMask playerMask;
        [SerializeField] GameObject bulletPiece1, bulletPiece2;

        [SerializeField] float speed = 3f;

        private void Update()
        {
            transform.Translate((right? transform.right : -transform.right) * speed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();

            if (right)
            {
                List<RaycastHit2D> colList = GetCollisionsToTheRight(playerMask, speed * Time.deltaTime - skinWidth * 2);
                if (colList.Count > 0)
                {
                    for (int i = 0; i < colList.Count; i++)
                    {
                        if (colList[i].transform.CompareTag("Player"))
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                            BreakIntoBulletShards();
                        }
                    }
                }

                if (GetCollisionsToTheRight(collisionMask, speed * Time.deltaTime - skinWidth * 2).Count > 0)
                {
                    BreakIntoBulletShards();
                }
            }
            else
            {
                List<RaycastHit2D> cols = GetCollisionsToTheLeft(playerMask, speed * Time.deltaTime - skinWidth * 2);
                if (cols.Count > 0)
                {
                    for (int i = 0; i < cols.Count; i++)
                    {
                        if (cols[i].transform.CompareTag("Player"))
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                            BreakIntoBulletShards();
                        }
                    }
                }

                if (GetCollisionsToTheLeft(collisionMask, speed * Time.deltaTime - skinWidth * 2).Count > 0)
                {
                    BreakIntoBulletShards();
                }
            }
            
        }

        private void BreakIntoBulletShards()
        {
            GameObject piece1 = Instantiate(bulletPiece1, transform.position, Quaternion.identity);
            GameObject piece2 = Instantiate(bulletPiece2, transform.position, Quaternion.identity);

            Destroy(piece1, 5f);
            Destroy(piece2, 5f);

            Destroy(gameObject);
        }
    }
}

