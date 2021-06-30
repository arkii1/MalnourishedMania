using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MalnourishedMania
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] Image pineappleImage;
        [SerializeField] Image kiwiImage;
        [SerializeField] Image melonImage;
        [SerializeField] Image strawberryImage;
        [SerializeField] Image bananaImage;

        [SerializeField] Sprite kiwi, pineapple, melon, strawberry, banana;
        [SerializeField] Sprite kiwiOutline, pineappleOutline, melonOutline, strawberryOutline, bananaOutline;

        private void Start()
        {
            bananaImage.sprite = bananaOutline;
            kiwiImage.sprite = kiwiOutline;
            melonImage.sprite = melonOutline;
            pineappleImage.sprite = pineappleOutline;
            strawberryImage.sprite = strawberryOutline;
        }

        public void CollectedFruit(FruitType type)
        {
            switch (type)
            {
                case FruitType.banana:
                    bananaImage.sprite = banana;
                    break;
                case FruitType.kiwi:
                    kiwiImage.sprite = kiwi;
                    break;
                case FruitType.melon:
                    melonImage.sprite = melon;
                    break;
                case FruitType.pineapple:
                    pineappleImage.sprite = pineapple;
                    break;
                case FruitType.strawberry:
                    strawberryImage.sprite = strawberry;
                    break;
                default:
                    break;
            }
        }

        public void DroppedFruit(FruitType type)
        {
            switch (type)
            {
                case FruitType.banana:
                    bananaImage.sprite = bananaOutline;
                    break;
                case FruitType.kiwi:
                    kiwiImage.sprite = kiwiOutline;
                    break;
                case FruitType.melon:
                    melonImage.sprite = melonOutline;
                    break;
                case FruitType.pineapple:
                    pineappleImage.sprite = pineappleOutline;
                    break;
                case FruitType.strawberry:
                    strawberryImage.sprite = strawberryOutline;
                    break;
                default:
                    break;
            }

        }
    }
}

