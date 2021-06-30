using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] GameObject maskedDude, virtualGuy, pinkMan, ninjaFrog;

        [SerializeField] float verticalOffset;
        [SerializeField] float lookAheadDistanceX;
        [SerializeField] float lookAheadDistanceY;
        [SerializeField] float lookSmoothTimeX;
        [SerializeField] float lookSmoothTimeY;
        [SerializeField] float lookSmoothTimeReturnY;//when camera returns from a look ahead y
        [SerializeField] float verticalSmoothTime;
        [SerializeField] float targetLookAheadY;


        [SerializeField] Vector2 focusAreaSize;
        [SerializeField] Vector3 camOffset;

        public Vector3 CamOffset { set { camOffset = value; } }

        FocusArea focusArea;

        float currentLookAheadX;
        float targetLookAheadX;
        float currentLookAheadY;

        float lookAheadDirX;
        float lookAheadDirY;

        float smoothLookVelocityX;
        float smoothLookVelocityY;
        float smoothVelocityY;

        bool lookAheadStoppedX;
        bool lookAheadStoppedY;

        PlayerController target;
        PlayerInput playerInput;

        public void Init(Characters character)
        {
            switch (character)
            {
                case Characters.maskedDude:
                    target = maskedDude.GetComponent<PlayerController>();
                    break;
                case Characters.ninjaFrog:
                    target = ninjaFrog.GetComponent<PlayerController>();
                    break;
                case Characters.pinkMan:
                    target = pinkMan.GetComponent<PlayerController>();
                    break;
                case Characters.virtualGuy:
                    target = virtualGuy.GetComponent<PlayerController>();
                    break;
                default:
                    break;
            }
            
            focusArea = new FocusArea(target.boxCollider.bounds, focusAreaSize);

        }

        private void LateUpdate()
        {
            if (FindObjectOfType<GameUIMaster>() != null && FindObjectOfType<GameUIMaster>().state != GameUIState.ingame || target == null)
                return;

            focusArea.Update(target.boxCollider.bounds);

            Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

            if(focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if(Mathf.Sign(target.GetComponent<PlayerManager>().playerInput.GetHorizontalInput()) == Mathf.Sign(focusArea.velocity.x) && Mathf.Sign(target.GetComponent<PlayerManager>().playerInput.GetHorizontalInput()) != 0)
                {
                    lookAheadStoppedX = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
                }
                else
                {
                    if (!lookAheadStoppedX)
                    {
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistanceX - currentLookAheadX) / 4;
                        lookAheadStoppedX = true;
                    }
                }
            }

            if(focusArea.velocity.x == 0 && playerInput.GetVerticalInput() != 0)
            {
                lookAheadStoppedY = false;
                lookAheadDirY = Mathf.Sign(playerInput.GetVerticalInput());
                targetLookAheadY = lookAheadDirY * lookAheadDistanceY;
            }
            else
            {
                targetLookAheadY = 0;
                
                if(Mathf.Abs(currentLookAheadY - focusPosition.y) < 0.001f)
                {
                    lookAheadStoppedY = true;
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            float ySmoothTime = lookAheadStoppedY ? lookSmoothTimeReturnY : lookSmoothTimeY;
            currentLookAheadY = Mathf.SmoothDamp(currentLookAheadY, targetLookAheadY, ref smoothLookVelocityY, ySmoothTime);
            
            if(lookAheadStoppedY)
                focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

            focusPosition += new Vector2(currentLookAheadX, currentLookAheadY);
            transform.position = (Vector3)focusPosition + (Vector3.forward * -10) + camOffset;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);

                Gizmos.DrawCube(focusArea.centre + Vector2.up * verticalOffset, focusAreaSize);
        }

        [System.Serializable]
        public struct FocusArea
        {
            public Vector2 centre;
            public Vector2 velocity;
            float left, right;
            float top, bottom;

            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;

                velocity = Vector2.zero;
                centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if(targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if(targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }

                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }

                bottom += shiftY;
                top += shiftY;

                centre = new Vector2((left + right) / 2, (top + bottom) / 2);

                velocity = new Vector2(shiftX, shiftY);
            }
        }

    }
}

