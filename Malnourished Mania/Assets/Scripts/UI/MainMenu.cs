using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MalnourishedMania
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject mainMenu;
        [SerializeField] GameObject levelSelect;

        [SerializeField] GameObject level2LockImage, level3LockImage, level4LockImage, level5LockImage;

        private void Awake()
        {
            ReturnToMainMenu();

            PlayerData playerData = FindObjectOfType<GameDataManager>().playerData;

            level2LockImage.SetActive(!playerData.unlockedLevelTwo);
            level3LockImage.SetActive(!playerData.unlockedLevelThree);
            level4LockImage.SetActive(!playerData.unlockedLevelFour);
            level5LockImage.SetActive(!playerData.unlockedLevelFive);
        }

        public void OpenLevelSelect()
        {
            mainMenu.SetActive(false);
            levelSelect.SetActive(true);
        }

        public void LoadLevel(int scene)
        {
            if(FindObjectOfType<GameDataManager>().IsLevelUnlocked(scene - 1)) //level 1 = scene 2, level 2 = scene 3..
                GameManager.LoadScene(scene);
        }

        public void ReturnToMainMenu()
        {
            mainMenu.SetActive(true);
            levelSelect.SetActive(false);
        }

        public void ExitGame()
        {
            GameManager.ExitGame();
        }

        public void OpenFruitShop()
        {
            GameManager.LoadFruitShop();
        }
    }
}
