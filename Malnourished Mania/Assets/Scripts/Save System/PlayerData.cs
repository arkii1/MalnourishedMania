using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    [System.Serializable]
    public class PlayerData
    {
        #region Data
        //Levels
        public bool unlockedLevelOne = true;
        public bool unlockedLevelTwo = false;
        public bool unlockedLevelThree = false;
        public bool unlockedLevelFour = false;
        public bool unlockedLevelFive = false;
        public bool unlockedLevelSix = false;
        public bool unlockedLevelSeven = false;
        public bool unlockedLevelEight= false;
        public bool unlockedLevelNine = false;
        public bool unlockedLevelTen = false;

        //Fruit
        public int bananas = 0;
        public int kiwis = 0;
        public int pineapples = 0;
        public int melons = 0;
        public int strawberries = 0;

        //Cosmetics
        public bool unlockedGreenGround = true; //set true
        public bool unlockedOrangeGround = false;
        public bool unlockedPinkGround = false;

        public bool unlockedBronzeBar = true; //set true
        public bool unlockedSilverBar = false;
        public bool unlockedGoldBar = false;

        public bool unlockedWoodBoundary = true; //set true
        public bool unlockedStoneBoundary = false;
        public bool unlockedLeafBoundary = false;

        public bool unlockedBlueBG = true; //set true
        public bool unlockedBrownBG = false; 
        public bool unlockedGrayBG = false; 
        public bool unlockedGreenBG = false; 
        public bool unlockedPinkBG = false; 
        public bool unlockedPurpleBG = false; 
        public bool unlockedYellowBG = false;

        public bool unlockedMaskedMan = true; //set true
        public bool unlockedNinjaFrog = false;
        public bool unlockedPinkMan = false;
        public bool unlockedVirtualGuy = false;

        // Preferences
        public GroundTiles groundTile = GroundTiles.green;
        public BarTiles barTile = BarTiles.bronze;
        public BoundaryTiles boundaryTile = BoundaryTiles.wood;
        public Backgrounds backGround = Backgrounds.blue;
        public Characters character = Characters.maskedDude;
        #endregion

    }

    public enum GroundTiles
    {
        green, orange, pink
    }

    public enum BarTiles
    {
        bronze, silver, gold
    }

    public enum BoundaryTiles
    {
        wood, stone, leaf
    }

    public enum Backgrounds
    {
        blue, brown, gray, green, pink, purple, yellow
    }
    
    public enum Characters
    {
        maskedDude, ninjaFrog, pinkMan, virtualGuy
    }

}

