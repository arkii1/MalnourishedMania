using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MalnourishedMania
{
    public class PostLevelUI : MonoBehaviour
    {
        public Image bananaImage, kiwiImage, melonImage, pineappleImage, strawberryImage;

        public Image inGameBananaImage, inGameKiwiImage, inGameMelonImage, inGamePineappleImage, inGameStrawberryImage;

        public Sprite bananaOL, kiwiOL, melonOL, pineappleOL, strawberryOL;

        public Sprite banana, kiwi, melon, pineapple, strawberry;

        public GameObject levelComplete, playNextLevelButton;

        private void OnEnable()
        {
            bananaImage.sprite = inGameBananaImage.sprite == banana ? banana : bananaOL;
            kiwiImage.sprite = inGameKiwiImage.sprite == kiwi ? kiwi : kiwiOL;
            melonImage.sprite = inGameMelonImage.sprite == melon ? melon : melonOL;
            pineappleImage.sprite = inGamePineappleImage.sprite == pineapple ? pineapple : pineappleOL;
            strawberryImage.sprite = inGameStrawberryImage.sprite == strawberry ? strawberry : strawberryOL;

            int level = SceneManager.GetActiveScene().buildIndex - 1;
            string levelCompleteText =  "You Completeled Level " + level + "/5!";

            if (level == 5)
            {
                levelCompleteText = "You Have Completed The Game!";
                playNextLevelButton.SetActive(false);
            }
            else
                playNextLevelButton.SetActive(true);

            levelComplete.GetComponent<TextMeshProUGUI>().text = levelCompleteText;

        }

        public void OpenFruitShop()
        {
            GameManager.LoadFruitShop();
        }

    }
}

