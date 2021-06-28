using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MalnourishedMania
{
    public class PauseMenu : MonoBehaviour
    {
        public Image banana, kiwi, melon, pineapple, strawberry;

        public Image inGameBananaImage, inGameKiwiImage, inGameMelonImage, inGamePineappleImage, inGameStrawberryImage;

        private void OnEnable()
        {
            banana.sprite = inGameBananaImage.sprite;
            kiwi.sprite = inGameKiwiImage.sprite;
            melon.sprite = inGameMelonImage.sprite;
            pineapple.sprite = inGamePineappleImage.sprite;
            strawberry.sprite = inGameStrawberryImage.sprite;
        }
    }
}

