using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yodo1.MAS;

public class SaveData : MonoBehaviour
{
    #region Singleton

    public static SaveData instance;

    private void Awake()
    {
        //Set screen size for Standalone
#if UNITY_STANDALONE
            Screen.SetResolution(800, 1000, false);
            Screen.fullScreen = false;
#endif

        if (instance != null)
            Destroy(gameObject);        //Make sure that there aren't multiple instances of this or we're screwed >xD
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        LoadGame();

        Yodo1U3dMas.InitializeSdk();
    }
    #endregion

    #region Main Save Information:

    [Header("Main Save Data:")]
    public int points;
    public int lvl;

    [Header("Gradient:")]
    public float gradR;
    public float gradG;
    public float gradB;

    [Header("Repeat Lvl")]
    public bool repeatLvl;

    [Header("Tournament:")]
    public string npcN1;
    public string npcN2;
    public string npcN3;
    public string npcN4;
    public string npcN5;
    public string npcN6;
    public string npcEnemy;
    [Space]
    public int premPts1;
    public int premPts2;
    public int premPts3;
    public int premPts4;
    public int premPts5;
    public int premPts6;
    [Space]
    public int playerPremPts;
    public string playerName;

    [Header("In League:")]
    public bool inLeague;

    public string cupName;
    public bool winL1;
    public bool winL2;
    public bool winL3;
    public bool winL4;

    [Header("Upgrades:")]
    public float kickPower;
    public float moveSpeed;
    public float bonusSize;
    [Space]
    public int speedLvl;
    public int sizeLvl;
    public int kickLvl;

    [Header("Sound:")]
    public bool soundOn;

    [Header("Bonus:")]
    public bool bonus;
    public int timeYear;
    public int timeMonth;
    public int timeDay;
    public int timeHour;
    public int timeMinute;
    public int timeSecond;

    [Header("Skins Save Data:")]
    public List<int> bodyUnlocked = new List<int>();
    public int bodyId;

    [Header("Quick Bonus Fix:")]
    public bool bonusFix;

    #endregion

    #region Save and Load:

    public void SaveGame()              //This should be called whenever the game needs to be saved...
    {
        Save.SaveUser(this);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void LoadGame()
    {
        //Load user data...
        if (File.Exists($"{Application.persistentDataPath}/user.dat"))
        {
            UserValuesData player = Save.LoadUser();

            lvl = player.lvl;
            points = player.points;

            gradR = player.gradR;
            gradG = player.gradG;
            gradB = player.gradB;

            repeatLvl = player.repeatLvl;

            npcN1 = player.npcN1;
            npcN2 = player.npcN2;
            npcN3 = player.npcN3;
            npcN4 = player.npcN4;
            npcN5 = player.npcN5;
            npcN6 = player.npcN6;
            npcEnemy = player.npcEnemy;

            premPts1 = player.premPts1;
            premPts2 = player.premPts2;
            premPts3 = player.premPts3;
            premPts4 = player.premPts4;
            premPts5 = player.premPts5;
            premPts6 = player.premPts6;

            playerPremPts = player.playerPremPts;
            playerName = player.playerName;

            inLeague = player.inLeague;
            cupName = player.cupName;

            winL1 = player.winL1;
            winL2 = player.winL2;
            winL3 = player.winL3;
            winL4 = player.winL4;

            kickPower = player.kickPower;
            moveSpeed = player.moveSpeed;
            bonusSize = player.bonusSize;
            
            speedLvl = player.speedLvl;
            sizeLvl = player.sizeLvl;
            kickLvl = player.kickLvl;

            soundOn = player.soundOn;
            bonus = player.bonus;
            timeYear = player.timeYear;
            timeMonth = player.timeMonth;
            timeDay = player.timeDay;
            timeHour = player.timeHour;
            timeMinute = player.timeMinute;
            timeSecond = player.timeSecond;

            bodyUnlocked = player.bodyUnlocked;
            bodyId = player.bodyId;

            bonusFix = player.bonusFix;
        }
    }

    #endregion



}