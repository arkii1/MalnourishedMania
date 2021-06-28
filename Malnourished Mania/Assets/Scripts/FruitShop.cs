using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MalnourishedMania
{
    public class FruitShop : MonoBehaviour
    {
        #region Cosmetic references
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
        #endregion

        #region Costmetic Cost
        [Header("Costmetic Costs")]

        public int orangeGroundCost = 3, pinkGroundCost = 5;
        public int silverBarCost = 3, goldBarCost = 5;
        public int stoneBoundaryCost = 3, leafBoundaryCost = 5;
        public int brownBGCost = 1, grayBGCost = 1, greenBGCost = 1, pinkBGCost = 2, purpleBGCost = 2, yellowBGCost = 3;
        public int virtualGuyCost = 3, pinkManCost = 2, ninjaFrogCost = 4;
        #endregion

        #region UI Gameobjects
        public GameObject orangeCostUI, pinkCostUI;
        public GameObject silverCostUI, goldCostUI;
        public GameObject stoneCostUI, leafCostUI;
        public GameObject brownBGCostUI, grayBGCostUI, greenBGCostUI, pinkBGCostUI, purpleBGCostUI, yellowBGCostUI;
        public GameObject virtualGuyCostUI, pinkManCostUI, ninjaFrogCostUI;

        public GameObject noOfBananasUI, noOfKiwisUI, noOfMelonsUI, noOfPineapplesUI, noOfStrawberriesUI;
        #endregion

        public PlayerData playerData;

        private void Start()
        {
            if (FindObjectOfType<GameDataManager>() != null)
                playerData = FindObjectOfType<GameDataManager>().playerData;
            else playerData = new PlayerData();

            UpdateCosmetics();
            UpdateUI();
        }

        void UpdateCosmetics()
        {
            switch (playerData.groundTile)
            {
                case GroundTiles.green:
                    greenGroundTiles.SetActive(true);
                    orangeGroundTiles.SetActive(false);
                    pinkGroundTiles.SetActive(false);
                    break;
                case GroundTiles.orange:
                    greenGroundTiles.SetActive(false);
                    orangeGroundTiles.SetActive(true);
                    pinkGroundTiles.SetActive(false);
                    break;
                case GroundTiles.pink:
                    greenGroundTiles.SetActive(false);
                    orangeGroundTiles.SetActive(false);
                    pinkGroundTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (playerData.barTile)
            {
                case BarTiles.bronze:
                    bronzeBarTiles.SetActive(true);
                    silverBarTiles.SetActive(false);
                    goldBarTiles.SetActive(false);
                    break;
                case BarTiles.silver:
                    bronzeBarTiles.SetActive(false);
                    silverBarTiles.SetActive(true);
                    goldBarTiles.SetActive(false);
                    break;
                case BarTiles.gold:
                    bronzeBarTiles.SetActive(false);
                    silverBarTiles.SetActive(false);
                    goldBarTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (playerData.boundaryTile)
            {
                case BoundaryTiles.wood:
                    woodBoundaryTiles.SetActive(true);
                    stoneBoundaryTiles.SetActive(false);
                    leafBoundaryTiles.SetActive(false);
                    break;
                case BoundaryTiles.stone:
                    woodBoundaryTiles.SetActive(false);
                    stoneBoundaryTiles.SetActive(false);
                    leafBoundaryTiles.SetActive(true);
                    break;
                case BoundaryTiles.leaf:
                    woodBoundaryTiles.SetActive(false);
                    stoneBoundaryTiles.SetActive(false);
                    leafBoundaryTiles.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (playerData.backGround)
            {
                case Backgrounds.blue:
                    blueBackGround.SetActive(true);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.brown:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(true);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.gray:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(true);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.green:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(true);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.pink:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(true);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.purple:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(true);
                    yellowBackGround.SetActive(false);
                    break;
                case Backgrounds.yellow:
                    blueBackGround.SetActive(false);
                    brownBackGround.SetActive(false);
                    grayBackGround.SetActive(false);
                    greenBackGround.SetActive(false);
                    pinkBackGround.SetActive(false);
                    purpleBackGround.SetActive(false);
                    yellowBackGround.SetActive(true);
                    break;
                default:
                    break;
            }
            switch (playerData.character)
            {
                case Characters.maskedDude:
                    maskedDude.SetActive(true);
                    ninjaFrog.SetActive(false);
                    virtualGuy.SetActive(false);
                    pinkMan.SetActive(false);
                    break;
                case Characters.ninjaFrog:
                    maskedDude.SetActive(false);
                    ninjaFrog.SetActive(true);
                    virtualGuy.SetActive(false);
                    pinkMan.SetActive(false);
                    break;
                case Characters.pinkMan:
                    maskedDude.SetActive(false);
                    ninjaFrog.SetActive(false);
                    virtualGuy.SetActive(false);
                    pinkMan.SetActive(true);
                    break;
                case Characters.virtualGuy:
                    maskedDude.SetActive(false);
                    ninjaFrog.SetActive(false);
                    virtualGuy.SetActive(true);
                    pinkMan.SetActive(false);
                    break;
                default:
                    break;
            }
            FindObjectOfType<CameraFollow>().Init(playerData.character);
        }

        void UpdateUI()
        {
            orangeCostUI.SetActive(!playerData.unlockedOrangeGround);
            orangeCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + orangeGroundCost; 
            pinkCostUI.SetActive(!playerData.unlockedPinkGround);
            pinkCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + pinkGroundCost; 

            silverCostUI.SetActive(!playerData.unlockedSilverBar);
            silverCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + silverBarCost; 
            goldCostUI.SetActive(!playerData.unlockedGoldBar);
            goldCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + goldBarCost; 

            stoneCostUI.SetActive(!playerData.unlockedStoneBoundary);
            stoneCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + stoneBoundaryCost; 
            leafCostUI.SetActive(!playerData.unlockedLeafBoundary);
            leafCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + leafBoundaryCost; 

            brownBGCostUI.SetActive(!playerData.unlockedBrownBG);
            brownBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + brownBGCost; 
            grayBGCostUI.SetActive(!playerData.unlockedGrayBG);
            grayBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + grayBGCost; 
            greenBGCostUI.SetActive(!playerData.unlockedGreenBG);
            greenBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + greenBGCost; 
            pinkBGCostUI.SetActive(!playerData.unlockedPinkBG);
            pinkBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + pinkBGCost; 
            purpleBGCostUI.SetActive(!playerData.unlockedPurpleBG);
            purpleBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + purpleBGCost; 
            yellowBGCostUI.SetActive(!playerData.unlockedYellowBG);
            yellowBGCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + yellowBGCost; 

            pinkManCostUI.SetActive(!playerData.unlockedPinkMan);
            pinkManCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + pinkManCost; 
            virtualGuyCostUI.SetActive(!playerData.unlockedVirtualGuy);
            virtualGuyCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + virtualGuyCost; 
            ninjaFrogCostUI.SetActive(!playerData.unlockedNinjaFrog);
            ninjaFrogCostUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + ninjaFrogCost;

            noOfBananasUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + playerData.bananas;
            noOfKiwisUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + playerData.kiwis;
            noOfMelonsUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + playerData.melons;
            noOfPineapplesUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + playerData.pineapples;
            noOfStrawberriesUI.GetComponentInChildren<TextMeshProUGUI>().text = "x " + playerData.strawberries;

        }

        public void PlayNextLevel()
        {
            GameManager.LoadNextLevelFromFruitShop();
        }

        public void GoToMainMenu()
        {
            GameManager.LoadScene(0);
        }

        #region Buy Methods. Will also equip if already owned
        public void BuyGroundTile(int tile) //BANANAS
        {
            switch (tile)
            {
                case 0:
                    playerData.groundTile = GroundTiles.green;
                    break;
                case 1:
                    if (playerData.unlockedOrangeGround)
                    {
                        playerData.groundTile = GroundTiles.orange;
                    }
                    else if (playerData.bananas >= orangeGroundCost)// got enough fruit
                    {
                        playerData.bananas -= orangeGroundCost;
                        playerData.unlockedOrangeGround = true;
                        playerData.groundTile = GroundTiles.orange;
                    }
                    break;
                case 2:
                    if (playerData.unlockedPinkGround)
                    {
                        playerData.groundTile = GroundTiles.pink;
                    }
                    else if (playerData.bananas >= pinkGroundCost)// got enough fruit
                    {
                        playerData.bananas -= pinkGroundCost;
                        playerData.unlockedPinkGround = true;
                        playerData.groundTile = GroundTiles.pink;
                    }
                    break;
                default:
                    break;
            }

            UpdateCosmetics();
            UpdateUI();
        }

        public void BuyBarTile(int tile)
        {
            switch (tile)
            {
                case 0:
                    playerData.barTile = BarTiles.bronze;
                    break;
                case 1:
                    if (playerData.unlockedSilverBar)
                    {
                        playerData.barTile = BarTiles.silver;
                    }
                    else if (playerData.kiwis >= silverBarCost)// got enough fruit
                    {
                        playerData.kiwis -= silverBarCost;
                        playerData.unlockedSilverBar = true;
                        playerData.barTile = BarTiles.silver;
                    }
                    break;
                case 2:
                    if (playerData.unlockedGoldBar)
                    {
                        playerData.barTile = BarTiles.gold;
                    }
                    else if (playerData.kiwis >= goldBarCost)// got enough fruit
                    {
                        playerData.kiwis -= goldBarCost;
                        playerData.unlockedGoldBar = true;
                        playerData.barTile = BarTiles.gold;
                    }
                    break;
                default:
                    break;
            }

            UpdateCosmetics();
            UpdateUI();
        }

        public void BuyBoundaryTile(int tile)
        {
            switch (tile)
            {
                case 0:
                    playerData.boundaryTile = BoundaryTiles.wood;
                    break;
                case 1:
                    if (playerData.unlockedStoneBoundary)
                    {
                        playerData.boundaryTile = BoundaryTiles.stone;
                    }
                    else if (playerData.melons >= stoneBoundaryCost)// got enough fruit
                    {
                        playerData.melons -= stoneBoundaryCost;
                        playerData.unlockedStoneBoundary = true;
                        playerData.boundaryTile = BoundaryTiles.stone;
                    }
                    break;
                case 2:
                    if (playerData.unlockedStoneBoundary)
                    {
                        playerData.boundaryTile = BoundaryTiles.leaf;
                    }
                    else if (playerData.melons >= leafBoundaryCost)// got enough fruit
                    {
                        playerData.melons -= leafBoundaryCost;
                        playerData.unlockedLeafBoundary = true;
                        playerData.boundaryTile = BoundaryTiles.leaf;
                    }
                    break;
                default:
                    break;
            }

            UpdateCosmetics();
            UpdateUI();
        }

        public void BuyBackground(int bg)
        {
            switch (bg)
            {
                case 0:
                    playerData.backGround = Backgrounds.blue;
                    break;
                case 1:
                    if (playerData.unlockedBrownBG)
                    {
                        playerData.backGround = Backgrounds.brown;
                    }
                    else if(playerData.pineapples >= brownBGCost)
                    {
                        playerData.pineapples -= brownBGCost;
                        playerData.unlockedBrownBG = true;
                        playerData.backGround = Backgrounds.brown;
                    }
                    break;
                case 2:
                    if (playerData.unlockedGrayBG)
                    {
                        playerData.backGround = Backgrounds.gray;
                    }
                    else if (playerData.pineapples >= grayBGCost)
                    {
                        playerData.pineapples -= grayBGCost;
                        playerData.unlockedGrayBG = true;
                        playerData.backGround = Backgrounds.gray;
                    }
                    break;
                case 3:
                    if (playerData.unlockedGreenBG)
                    {
                        playerData.backGround = Backgrounds.green;
                    }
                    else if (playerData.pineapples >= greenBGCost)
                    {
                        playerData.pineapples -= greenBGCost;
                        playerData.unlockedGreenBG = true;
                        playerData.backGround = Backgrounds.green;
                    }
                    break;
                case 4:
                    if (playerData.unlockedPinkBG)
                    {
                        playerData.backGround = Backgrounds.pink;
                    }
                    else if (playerData.pineapples >= pinkBGCost)
                    {
                        playerData.pineapples -= pinkBGCost;
                        playerData.unlockedPinkBG = true;
                        playerData.backGround = Backgrounds.pink;
                    }
                    break;
                case 5:
                    if (playerData.unlockedPurpleBG)
                    {
                        playerData.backGround = Backgrounds.purple;
                    }
                    else if (playerData.pineapples >= purpleBGCost)
                    {
                        playerData.pineapples -= purpleBGCost;
                        playerData.unlockedPurpleBG = true;
                        playerData.backGround = Backgrounds.purple;
                    }
                    break;
                case 6:
                    if (playerData.unlockedYellowBG)
                    {
                        playerData.backGround = Backgrounds.yellow;
                    }
                    else if (playerData.pineapples >= yellowBGCost)
                    {
                        playerData.pineapples -= yellowBGCost;
                        playerData.unlockedYellowBG = true;
                        playerData.backGround = Backgrounds.yellow;
                    }
                    break;
                default:
                    break;
            }

            UpdateCosmetics();
            UpdateUI();
        }

        public void BuyCharacter(int character)
        {
            switch (character)
            {
                case 0:
                    maskedDude.SetActive(true);
                    FindObjectOfType<CameraFollow>().Init(Characters.maskedDude);
                    playerData.character = Characters.maskedDude;
                    ninjaFrog.SetActive(false);
                    virtualGuy.SetActive(false);
                    pinkMan.SetActive(false);
                    break;
                case 1:
                    if (playerData.unlockedNinjaFrog)
                    {
                        ninjaFrog.SetActive(true);
                        FindObjectOfType<CameraFollow>().Init(Characters.ninjaFrog);
                        playerData.character = Characters.ninjaFrog;
                        maskedDude.SetActive(false);
                        virtualGuy.SetActive(false);
                        pinkMan.SetActive(false);
                    }
                    else if (playerData.strawberries >= ninjaFrogCost)
                    {
                        ninjaFrog.SetActive(true);
                        playerData.strawberries -= ninjaFrogCost;
                        playerData.unlockedNinjaFrog = true;
                        FindObjectOfType<CameraFollow>().Init(Characters.ninjaFrog);
                        playerData.character = Characters.ninjaFrog;
                        maskedDude.SetActive(false);
                        virtualGuy.SetActive(false);
                        pinkMan.SetActive(false);
                    }
                    break;
                case 2:
                    if (playerData.unlockedPinkMan)
                    {
                        pinkMan.SetActive(true);
                        FindObjectOfType<CameraFollow>().Init(Characters.pinkMan);
                        playerData.character = Characters.pinkMan;
                        maskedDude.SetActive(false);
                        ninjaFrog.SetActive(false);
                        virtualGuy.SetActive(false);
                    }
                    else if (playerData.strawberries >= pinkManCost)
                    {
                        pinkMan.SetActive(true);
                        playerData.strawberries -= pinkManCost;
                        playerData.unlockedPinkMan = true;
                        FindObjectOfType<CameraFollow>().Init(Characters.pinkMan);
                        playerData.character = Characters.pinkMan;
                        maskedDude.SetActive(false);
                        ninjaFrog.SetActive(false);
                        virtualGuy.SetActive(false);
                    }
                    break;
                case 3:
                    if (playerData.unlockedVirtualGuy)
                    {
                        virtualGuy.SetActive(true);
                        FindObjectOfType<CameraFollow>().Init(Characters.virtualGuy);
                        playerData.character = Characters.virtualGuy;
                        maskedDude.SetActive(false);
                        ninjaFrog.SetActive(false);
                        pinkMan.SetActive(false);
                    }
                    else if (playerData.strawberries >= virtualGuyCost)
                    {
                        virtualGuy.SetActive(true);
                        playerData.strawberries -= virtualGuyCost;
                        playerData.unlockedVirtualGuy = true;
                        FindObjectOfType<CameraFollow>().Init(Characters.virtualGuy);
                        playerData.character = Characters.virtualGuy;
                        maskedDude.SetActive(false);
                        ninjaFrog.SetActive(false);
                        pinkMan.SetActive(false);
                    }
                    break;
                default:
                    break;
            }

            UpdateCosmetics();
            UpdateUI();
        }

        #endregion
    }
}

