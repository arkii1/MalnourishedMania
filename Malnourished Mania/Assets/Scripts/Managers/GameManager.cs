using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MalnourishedMania
{
    public static class GameManager
    {
        static ObjectPooler objectPooler;
        public static ObjectPooler GetObjectPooler()
        {
            if (objectPooler == null)
            {
                objectPooler = Resources.Load("Managers/Object Pooler") as ObjectPooler;
                objectPooler.Init();
            }

            return objectPooler;
        }

        public static void LoadNextLevel()
        {
            //Reset
            lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            GetObjectPooler().Init();
        }

        public static void LoadFirstLevel() //0 is menu and 1 is shop
        {
            lastSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(2);
            GetObjectPooler().Init();

            Save();
        }

        public static void ExitGame()
        {
            Application.Quit();
        }

        public static int lastSceneIndex;

        public static void LoadFruitShop()
        {
            GameManager.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(1);
            Save();
        }

        public static void LoadNextLevelFromFruitShop()
        {
            if (lastSceneIndex != 0 && lastSceneIndex + 1 != SceneManager.sceneCount)
                SceneManager.LoadScene(lastSceneIndex + 1);
            else
                SceneManager.LoadScene(0);

            Save();
        }

        public static void Save()
        {
            GameObject.FindObjectOfType<GameDataManager>().Save();
        }

        public static void LoadScene(int scene)
        {
            SceneManager.LoadScene(scene);
        }

    }
}
