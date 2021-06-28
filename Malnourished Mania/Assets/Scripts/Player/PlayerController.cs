using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class PlayerController : RaycastController
    {
        public CollisionInfo collisions;

        public override void Start()
        {
            base.Start();
            collisions.faceDir = 1;
        }

        public void Move(Vector3 velocity, bool standingOnPlatform = false)
        {
            SetCollisionsFaceDir(velocity);

            collisions.Reset();

            UpdateRayCastOrigins();

            CheckCollisions(ref velocity);

            LimitVerticalVelocity(ref velocity);

            transform.Translate(velocity);
            SetBelowCollisionsToBool(standingOnPlatform);
        }

        private void SetCollisionsFaceDir(Vector3 velocity)
        {
            if (velocity.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(velocity.x);
            }
        }

        public void SetBelowCollisionsToBool(bool standingOnPlatform)
        {
            if (standingOnPlatform)
            {
                collisions.below = true;
            }
        }

        void LimitVerticalVelocity(ref Vector3 velocity)
        {
            if (Mathf.Abs(velocity.y) < Mathf.Pow(10, -3))
                velocity.y = 0;
        }

        void CheckCollisions(ref Vector3 velocity)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            rayLength = Mathf.Max(rayLength, 0.02f);
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();


            if (directionX == 1)
            {
                hitList = GetCollisionsToTheRight(collisionMask, rayLength);
            }
            else if(directionX == -1)
            {
                hitList = GetCollisionsToTheLeft(collisionMask, rayLength);
            }

            for (int i = 0; i < hitList.Count; i++)
            {
                if (hitList[i])
                {
                    velocity.x = (hitList[i].distance - skinWidth) * directionX;
                    rayLength = hitList[i].distance;

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }

            if (velocity.y != 0)
            {
                float directionY = Mathf.Sign(velocity.y);
                rayLength = Mathf.Abs(velocity.y + skinWidth);
                rayLength = Mathf.Max(rayLength, 0.02f);

                if (directionY == 1)
                {
                    hitList = GetCollisionsAbove(collisionMask, rayLength);
                }
                else if (directionY == -1)
                {
                    hitList = GetCollisionsBelow(collisionMask, rayLength);
                }

                for (int i = 0; i < hitList.Count; i++)
                {
                    if (hitList[i] && hitList[i].distance > 0)
                    {
                        velocity.y = (hitList[i].distance - skinWidth - 0.001f) * directionY; //0.001f is to make clipping less common

                        rayLength = hitList[i].distance;

                        collisions.below = directionY == -1;
                        collisions.above = directionY == 1;
                    }
                }
            }
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public int faceDir; // 1 means right, -1 means left
            
            public void Reset()
            {
                above = below = left = right = false;
            }
        }
    }
}

