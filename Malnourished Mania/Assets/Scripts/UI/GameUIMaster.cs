using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MalnourishedMania
{
    public class GameUIMaster : MonoBehaviour
    {
        [SerializeField] GameObject ingameUI;
        [SerializeField] GameObject pauseUI;
        [SerializeField] GameObject postLevelUI;

        public GameUIState state;

        private void Start()
        {
            ingameUI.SetActive(true);
            pauseUI.SetActive(false);
            postLevelUI.SetActive(false);

            state = GameUIState.ingame;
            Time.timeScale = 1;
        }

        public void PauseGame()
        {
            if (postLevelUI.activeInHierarchy)
                return;

            ingameUI.SetActive(false);
            pauseUI.SetActive(true);

            Time.timeScale = 0;

            state = GameUIState.paused;

        }

        public void ResumeGame()
        {
            if (postLevelUI.activeInHierarchy)
                return;

            ingameUI.SetActive(true);
            pauseUI.SetActive(false);

            Time.timeScale = 1;

            state = GameUIState.ingame;

        }

        public void RestartLevel()
        {
            Time.timeScale = 1;
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }

        public void ActivePostLevelScreen()
        {
            ingameUI.SetActive(false);
            pauseUI.SetActive(false);
            postLevelUI.SetActive(true);

            state = GameUIState.postLevel;
        }

        public void PlayNextLevel()
        {
            FindObjectOfType<MonoLevelManager>().CompleteLevel();
        }
    }

    public enum GameUIState
    {
        ingame, paused, postLevel
    }
}

