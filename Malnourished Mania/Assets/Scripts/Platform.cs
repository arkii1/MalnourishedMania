using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class Platform : RaycastController
    {
        public LayerMask playerMask;

        public Sprite bronzeSprite, silverSprite, goldSprite;

        public override void Start()
        {
            base.Start();
            if(FindObjectOfType<GameDataManager>())
            {
                switch (FindObjectOfType<GameDataManager>().playerData.barTile)
                {
                    case BarTiles.bronze:
                        GetComponent<SpriteRenderer>().sprite = bronzeSprite;
                        break;
                    case BarTiles.silver:
                        GetComponent<SpriteRenderer>().sprite = silverSprite;
                        break;
                    case BarTiles.gold:
                        GetComponent<SpriteRenderer>().sprite = silverSprite;
                        break;
                    default:
                        break;
                }
            }
        }


        private void FixedUpdate()
        {
            if (FindObjectOfType<PlayerManager>().hit)
                gameObject.layer = 2;

            if(GetCollisionsAbove(playerMask, 0.5f + skinWidth).Count > 0)
            {
                if (FindObjectOfType<PlayerManager>().PlayerWantsToDropThroughPlatform())
                {
                    gameObject.layer = 2; //ignore raycast
                }
                else
                {
                    gameObject.layer = 9; //obstacles
                }
            }
            
            if(GetCollisionsBelow(playerMask, 0.2f + skinWidth).Count > 0 || GetCollisionsToTheLeft(playerMask, 0.2f + skinWidth).Count > 0 || GetCollisionsToTheRight(playerMask, 0.2f + skinWidth).Count > 0)
            {
                gameObject.layer = 2; //ignore raycast
            }

            
        }
    }
}

