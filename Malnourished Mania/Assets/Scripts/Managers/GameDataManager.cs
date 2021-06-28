using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class GameDataManager : MonoBehaviour
    {
        public PlayerData playerData;

        private void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
#if !UNITY_EDITOR
            if (PlayerPrefs.HasKey("firstLoad"))
            {
                Debug.Log("Isn't players first load");
                playerData = SaveSystem.LoadData();
            }
            else
            {
                Debug.Log("Is players first load");
                playerData = new PlayerData();
                PlayerPrefs.SetInt("firstLoad", 0);
            }
            Debug.Log("Load bananas = " + playerData.bananas);
            Debug.Log("Load kiwis = " + playerData.kiwis);
            Debug.Log("Load melons = " + playerData.melons);
            Debug.Log("Load pineapples = " + playerData.pineapples);
            Debug.Log("Load strawberries = " + playerData.strawberries);
#endif

#if UNITY_EDITOR
            playerData = new PlayerData();
#endif
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveSystem.SaveData(playerData);
        }

        public void UnlockLevel(int levelInt)
        {
            switch (levelInt)
            {
                case 2:
                    playerData.unlockedLevelTwo = true;
                    break;
                case 3:
                    playerData.unlockedLevelThree = true;
                    break;
                case 4:
                    playerData.unlockedLevelFour = true;
                    break;
                case 5:
                    playerData.unlockedLevelFive = true;
                    break;
                case 6:
                    playerData.unlockedLevelSix = true;
                    break;
                case 7:
                    playerData.unlockedLevelSeven = true;
                    break;
                case 8:
                    playerData.unlockedLevelEight = true;
                    break;
                case 9:
                    playerData.unlockedLevelNine = true;
                    break;
                case 10:
                    playerData.unlockedLevelTen = true;
                    break;
                default:
                    break;
            }
        }

        public void AddFruit(FruitType fruit)
        {
            switch (fruit)
            {
                case FruitType.banana:
                    playerData.bananas++;
                    break;
                case FruitType.kiwi:
                    playerData.kiwis++;
                    break;
                case FruitType.melon:
                    playerData.melons++;
                    break;
                case FruitType.pineapple:
                    playerData.pineapples++;
                    break;
                case FruitType.strawberry:
                    playerData.strawberries++;
                    break;
                default:
                    break;
            }
        }

        public void UnlockedGroundTile(GroundTiles tile)
        {
            switch (tile)
            {
                case GroundTiles.green:
                    break;
                case GroundTiles.orange:
                    playerData.unlockedOrangeGround = true;
                    break;
                case GroundTiles.pink:
                    playerData.unlockedPinkGround = true;
                    break;
                default:
                    break;
            }
        }

        public void UnlockedBarTile(BarTiles tile)
        {
            switch (tile)
            {
                case BarTiles.bronze:
                    break;
                case BarTiles.silver:
                    playerData.unlockedSilverBar = true;
                    break;
                case BarTiles.gold:
                    playerData.unlockedGoldBar = true;
                    break;
                default:
                    break;
            }
        }

        public void UnlockedBoundaryTile(BoundaryTiles tile)
        {
            switch (tile)
            {
                case BoundaryTiles.wood:
                    break;
                case BoundaryTiles.stone:
                    playerData.unlockedStoneBoundary = true;
                    break;
                case BoundaryTiles.leaf:
                    playerData.unlockedLeafBoundary = true;
                    break;
                default:
                    break;
            }
        }

        public void UnlockedBackground(Backgrounds bg)
        {
            switch (bg)
            {
                case Backgrounds.blue:
                    break;
                case Backgrounds.brown:
                    playerData.unlockedBrownBG = true;
                    break;
                case Backgrounds.gray:
                    playerData.unlockedGrayBG = true;
                    break;
                case Backgrounds.green:
                    playerData.unlockedGreenBG = true;
                    break;
                case Backgrounds.pink:
                    playerData.unlockedPinkBG = true;
                    break;
                case Backgrounds.purple:
                    playerData.unlockedPurpleBG = true;
                    break;
                case Backgrounds.yellow:
                    playerData.unlockedYellowBG = true;
                    break;
                default:
                    break;
            }
        }

        public void UnlockedCharacter(Characters character)
        {
            switch (character)
            {
                case Characters.maskedDude:
                    break;
                case Characters.ninjaFrog:
                    playerData.unlockedNinjaFrog = true;
                    break;
                case Characters.pinkMan:
                    playerData.unlockedPinkMan = true;
                    break;
                case Characters.virtualGuy:
                    playerData.unlockedVirtualGuy = true;
                    break;
                default:
                    break;
            }
        }

        public void SetCosmeticPreferences(GroundTiles ground, BarTiles bar, BoundaryTiles boundary, Backgrounds bg, Characters character)
        {
            playerData.groundTile = ground;
            playerData.barTile = bar;
            playerData.boundaryTile = boundary;
            playerData.backGround = bg;
            playerData.character = character;
        }

        public bool IsLevelUnlocked(int levelInt)
        {
            switch (levelInt)
            {
                case 1:
                    return true;
                case 2:
                    if (playerData.unlockedLevelTwo) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 3:
                    if (playerData.unlockedLevelThree) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 4:
                    if (playerData.unlockedLevelFour) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 5:
                    if (playerData.unlockedLevelFive) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 6:
                    if (playerData.unlockedLevelSix) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 7:
                    if (playerData.unlockedLevelSeven) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 8:
                    if (playerData.unlockedLevelEight) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 9:
                    if (playerData.unlockedLevelNine) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                case 10:
                    if (playerData.unlockedLevelTen) return true;
                    Debug.Log("Player has not unlocked level " + levelInt);
                    return false;
                default:
                    break;
            }

            Debug.LogError("Level " + levelInt + " has not been set up!");
            return false;
        }
    }
}
