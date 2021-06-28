using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class SpikeHead : RaycastController
    {
        public CycleType cycleType;
        public LayerMask playerMask;
        float speed;
        public float speedMult;
        float timeAccelerating = 0;
        SpikeHeadAnimatorSystem spikeHeadAnimatorSystem;
        AudioSource audioSource;

        Direction direction;

        bool alreadyHitWall = false;

        public override void Start()
        {
            base.Start();
            SetInitialDirection();

            spikeHeadAnimatorSystem = gameObject.AddComponent<SpikeHeadAnimatorSystem>();
            spikeHeadAnimatorSystem.Init();
            spikeHeadAnimatorSystem.ChangeAnimationState(spikeHeadAnimatorSystem.idle);

            audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            UpdateRayCastOrigins();
            if (GetAllCollisions(playerMask, 0.05f).Count > 0)
                HitPlayer();

            List<RaycastHit2D> obstacleList = GetObstacleHitList();

            if (obstacleList.Count == 0)
            {
                transform.Translate(CalculateMovement());
                timeAccelerating += Time.deltaTime;
            }
            else if (!alreadyHitWall)
            {
                alreadyHitWall = true;
                PlayHitAnimation();

                float distance = obstacleList[0].distance - (5 * skinWidth);
                transform.Translate(GetVectorFromDirection() * distance);

                timeAccelerating = 0;
                speed = 0;
                ChooseNewDirection();

                if (FindObjectOfType<CameraShake>() && GetComponent<SpriteRenderer>().isVisible && !FindObjectOfType<EndWaypoint>().hitPlayer)
                {
                    PlayHitAudio();
                    if(WithinDistanceToPlayer())
                        FindObjectOfType<CameraShake>().Trauma = 0.5f;
                }
            }
        }

        bool WithinDistanceToPlayer()
        {
            return (transform.position-FindObjectOfType<PlayerManager>().transform.position).magnitude <=  8.0f;
        }

        void PlayHitAudio()
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        Vector3 CalculateMovement()
        {
            Vector3 retVal = Vector3.zero;
            speed = Ease(timeAccelerating) * Time.deltaTime;

            switch (direction)
            {
                case Direction.up:
                    retVal = Vector3.up * speed;
                    break;
                case Direction.down:
                    retVal = Vector3.down * speed;
                    break;
                case Direction.left:
                    retVal = Vector3.left * speed;
                    break;
                case Direction.right:
                    retVal = Vector3.right * speed;
                    break;
                default:
                    break;
            }

            return retVal;
        }

        void SetAlreadyHitWallToFalse()
        {
            Invoke("DelayAlreadyHitWallToFalse", 0.0f);
        }

        void DelayAlreadyHitWallToFalse()
        {
            alreadyHitWall = false;
        }

        List<RaycastHit2D> GetObstacleHitList()
        {
            switch (direction)
            {
                case Direction.up:
                    return GetCollisionsAbove(collisionMask, speed + skinWidth);
                case Direction.down:
                    return GetCollisionsBelow(collisionMask, speed + skinWidth);
                case Direction.left:
                    return GetCollisionsToTheLeft(collisionMask, speed + skinWidth);
                case Direction.right:
                    return GetCollisionsToTheRight(collisionMask, speed + skinWidth);
                default:
                    return new List<RaycastHit2D>();
            }
        }

        void PlayHitAnimation()
        {
            switch (direction)
            {
                case Direction.up:
                    spikeHeadAnimatorSystem.ChangeAnimationState(spikeHeadAnimatorSystem.topHit);
                    break;
                case Direction.down:
                    spikeHeadAnimatorSystem.ChangeAnimationState(spikeHeadAnimatorSystem.bottomHit);
                    break;
                case Direction.left:
                    spikeHeadAnimatorSystem.ChangeAnimationState(spikeHeadAnimatorSystem.leftHit);
                    break;
                case Direction.right:
                    spikeHeadAnimatorSystem.ChangeAnimationState(spikeHeadAnimatorSystem.rightHit);
                    break;
                default:
                    break;
            }
        }

        void HitPlayer()
        {
            FindObjectOfType<PlayerManager>().Hit();
        }

        float Ease(float x)
        {

            return x * x * x * x * speedMult;
        }

        void SetInitialDirection()
        {
            if (cycleType == CycleType.leftRight)
                direction = Direction.left;
            else direction = Direction.up;
        }

        void ChooseNewDirection()
        {
            switch (cycleType)
            {
                case CycleType.leftRight:
                    switch (direction)
                    {
                        case Direction.left:
                            direction = Direction.right;
                            break;
                        case Direction.right:
                            direction = Direction.left;
                            break;
                        default:
                            direction = Direction.left;
                            break;
                    }
                    break;
                case CycleType.upDown:
                    switch (direction)
                    {
                        case Direction.up:
                            direction = Direction.down;
                            break;
                        case Direction.down:
                            direction = Direction.up;
                            break;
                        default:
                            direction = Direction.up;
                            break;
                    }
                    break;
                case CycleType.cycleClockwise:
                    switch (direction)
                    {
                        case Direction.up:
                            direction = Direction.right;
                            break;
                        case Direction.down:
                            direction = Direction.left;
                            break;
                        case Direction.left:
                            direction = Direction.up;
                            break;
                        case Direction.right:
                            direction = Direction.down;
                            break;
                        default:
                            break;
                    }
                    break;
                case CycleType.cycleAntiClockwise:
                    switch (direction)
                    {
                        case Direction.up:
                            direction = Direction.left;
                            break;
                        case Direction.down:
                            direction = Direction.right;
                            break;
                        case Direction.left:
                            direction = Direction.down;
                            break;
                        case Direction.right:
                            direction = Direction.up;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        Vector3 GetVectorFromDirection()
        {
            switch (direction)
            {
                case Direction.up:
                    return Vector3.up;
                case Direction.down:
                    return Vector3.down;
                case Direction.left:
                    return Vector3.left;
                case Direction.right:
                    return Vector3.right;
                default:
                    return Vector3.zero;
            }
        }
    }

    public enum Direction
    {
        up, down, left, right
    }
    public enum CycleType
    {
        leftRight, upDown, cycleClockwise, cycleAntiClockwise
    }
}

