using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Bullet : RaycastController
    {
        public LayerMask playerMask;
        public float speed = 3f;
        public GameObject piece1, piece2;
        public bool right = true;

        private void Update()
        {
            transform.Translate((right? transform.right : -transform.right) * speed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();

            if (right)
            {
                List<RaycastHit2D> cols = GetCollisionsToTheRight(playerMask, speed * Time.deltaTime - skinWidth * 2);
                if (cols.Count > 0)
                {
                    for (int i = 0; i < cols.Count; i++)
                    {
                        Debug.Log(cols[i].transform.name);
                        if (cols[i].transform.CompareTag("Player"))
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                            Break();
                        }
                    }
                }

                if (GetCollisionsToTheRight(collisionMask, speed * Time.deltaTime - skinWidth * 2).Count > 0)
                {
                    Break();
                }
            }
            else
            {
                List<RaycastHit2D> cols = GetCollisionsToTheLeft(playerMask, speed * Time.deltaTime - skinWidth * 2);
                if (cols.Count > 0)
                {
                    for (int i = 0; i < cols.Count; i++)
                    {
                        Debug.Log(cols[i].transform.name);
                        if (cols[i].transform.CompareTag("Player"))
                        {
                            FindObjectOfType<PlayerManager>().Hit();
                            Break();
                        }
                    }
                }

                if (GetCollisionsToTheLeft(collisionMask, speed * Time.deltaTime - skinWidth * 2).Count > 0)
                {
                    Break();
                }
            }
            
        }

        private void Break()
        {
            GameObject p1 = Instantiate(piece1, transform.position, Quaternion.identity);
            GameObject p2 = Instantiate(piece2, transform.position, Quaternion.identity);

            Destroy(p1, 5f);
            Destroy(p2, 5f);

            Destroy(gameObject);
        }
    }
}

