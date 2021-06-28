using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MalnourishedMania
{
    public class MonoLevelManager : MonoBehaviour
    {
        #region Waypoint variables
        public List<Waypoint> waypoints = new List<Waypoint>(30);
        Waypoint spawnWaypoint = null;
        #endregion

        #region Fruit variables
        public List<Fruit> fruitBankedThisLevel = new List<Fruit>();
        public List<Fruit> carryingFruit = new List<Fruit>();
        #endregion

        #region Cosemetic References
        [Header("Cosemtics")]
        public GameObject greenGroundTiles;
        public GameObject orangeGroundTiles;
        public GameObject pinkGroundTiles;

        public GameObject bronzeBarTiles;
        public GameObject silverBarTiles;
        public GameObject goldBarTiles;

        public GameObject woodBoundaryTiles;
        public GameObject stoneBoundaryTiles;
        public GameObject leafBoundaryTiles;

        public GameObject blueBackGround;
        public GameObject brownBackGround;
        public GameObject grayBackGround;
        public GameObject greenBackGround;
        public GameObject pinkBackGround;
        public GameObject purpleBackGround;
        public GameObject yellowBackGround;

        public GameObject maskedDude;
        public GameObject virtualGuy;
        public GameObject ninjaFrog;
        public GameObject pinkMan;

        public GameObject saveDataPrefab;
        #endregion

        #region Creature Variables
        List<GameObject> creaturesKilledSinceLastWaypoint = new List<GameObject>();

        #endregion

        public GameDataManager gameDataManager = null;

        private void Awake()
        {
            if (FindObjectOfType<GameDataManager>() != null)
            {
                gameDataManager = FindObjectOfType<GameDataManager>();
            }
            else //this is for when i am in scene in unity level editor.
            {
                GameObject obj = Instantiate(saveDataPrefab);
                gameDataManager = GameObject.FindObjectOfType<GameDataManager>();
            }

            for (int i = 0; i < 30; i++)
            {
                waypoints.Add(null);
            }

            //Activate cosmetics
            switch (gameDataManager.playerData.groundTile)
            {
                case GroundTiles.green:
                    greenGroundTiles.SetActive(true);
                    break;
                case GroundTiles.orange:
                    orangeGroundTiles.SetActive(true);
                    break;
                case GroundTiles.pink:
                    pinkGroundTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (gameDataManager.playerData.barTile)
            {
                case BarTiles.bronze:
                    bronzeBarTiles.SetActive(true);
                    break;
                case BarTiles.silver:
                    silverBarTiles.SetActive(true);
                    break;
                case BarTiles.gold:
                    goldBarTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (gameDataManager.playerData.boundaryTile)
            {
                case BoundaryTiles.wood:
                    woodBoundaryTiles.SetActive(true);
                    break;
                case BoundaryTiles.stone:
                    stoneBoundaryTiles.SetActive(true);
                    break;
                case BoundaryTiles.leaf:
                    leafBoundaryTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (gameDataManager.playerData.backGround)
            {
                case Backgrounds.blue:
                    blueBackGround.SetActive(true);
                    break;
                case Backgrounds.brown:
                    brownBackGround.SetActive(true);
                    break;
                case Backgrounds.gray:
                    grayBackGround.SetActive(true);
                    break;
                case Backgrounds.green:
                    greenBackGround.SetActive(true);
                    break;
                case Backgrounds.pink:
                    pinkBackGround.SetActive(true);
                    break;
                case Backgrounds.purple:
                    purpleBackGround.SetActive(true);
                    break;
                case Backgrounds.yellow:
                    yellowBackGround.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (gameDataManager.playerData.character)
            {
                case Characters.maskedDude:
                    maskedDude.SetActive(true);
                    break;
                case Characters.ninjaFrog:
                    ninjaFrog.SetActive(true);
                    break;
                case Characters.pinkMan:
                    pinkMan.SetActive(true);
                    break;
                case Characters.virtualGuy:
                    virtualGuy.SetActive(true);
                    break;
                default:
                    break;
            }


        }

        private void Start()
        {
            FindObjectOfType<CameraFollow>().Init(gameDataManager.playerData.character);
        }

        #region Waypoint methods

        public void AddWaypoint(Waypoint waypoint)
        {
            if (waypoints[waypoint.index] == null)
            {
                waypoints[waypoint.index] = waypoint;
            }
            else
            {
                Debug.LogError("Waypoint at index " + waypoint.index + " already is filled!");
            }
        }

        public void TriggerWaypoint(Waypoint triggeredWaypoint)
        {
            if (!waypoints.Contains(triggeredWaypoint))
            {
                Debug.LogError("List doesn't contain waypoint!");
                return;
            }

            if (spawnWaypoint == null)
            {
                spawnWaypoint = triggeredWaypoint;
            }
            else
            {
                spawnWaypoint = waypoints.IndexOf(spawnWaypoint) > waypoints.IndexOf(triggeredWaypoint) ? spawnWaypoint : triggeredWaypoint;
            }

            creaturesKilledSinceLastWaypoint.Clear();
        }

        public void TransitionToWaypoint()
        {
            Vector3 spawnPos = GetSpawnPoint();

            GameObject.FindObjectOfType<PlayerManager>().transform.position = spawnPos + Vector3.up * 0.52f;
        }

        Vector3 GetSpawnPoint()
        {
            return spawnWaypoint != null ? spawnWaypoint.transform.position : waypoints[0].transform.position;
        }
        #endregion

        #region Fruit Methods
        public void AddCarryingFruit(Fruit fruit)
        {
            carryingFruit.Add(fruit);
        }

        public void BankFruit()
        {
            foreach (var fruit in carryingFruit)
                fruitBankedThisLevel.Add(fruit);

            carryingFruit = new List<Fruit>();
        }

        public void DropFruit()
        {
            foreach (Fruit fruit in carryingFruit)
            {
                fruit.EnableSpriteRenderer();
                FindObjectOfType<InGameUI>().DroppedFruit(fruit.fruitType);
            }

            carryingFruit = new List<Fruit>();
        }

        public void AddFruitToWallet()
        {
            foreach (var fruit in fruitBankedThisLevel)
            {
                FindObjectOfType<GameDataManager>().AddFruit(fruit.fruitType);
            }
        }

        #endregion

        public void TransitionOutOfLevel()
        {
            FindObjectOfType<Transition>().TransitionOut();
        }

        public void UnlockNextLevel()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex; //not plus 1 because plus one as scene one is taken up by fruit shop so all now level n = scene n
            FindObjectOfType<GameDataManager>().UnlockLevel(nextLevel);
        }

        public void CompleteLevel()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex; //not plus 1 because plus one as scene one is taken up by fruit shop so all now level n = scene n

            if (FindObjectOfType<GameDataManager>().IsLevelUnlocked(nextLevel))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                GameManager.GetObjectPooler().Init();
            }

        }

        public void AddCreature(GameObject creature)
        {
            creaturesKilledSinceLastWaypoint.Add(creature);
        }

        public void ReenableCreatures()
        {
            foreach (GameObject obj in creaturesKilledSinceLastWaypoint)
            {
                obj.SetActive(true);

                if (obj.GetComponent<Turtle>())
                    obj.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
