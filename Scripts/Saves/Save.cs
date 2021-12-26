using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class Save: MonoBehaviour
{
    #region Saving:
    public static void SaveUser(SaveData player)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream($"{Application.persistentDataPath}/user.dat", FileMode.Create);

        UserValuesData data = new UserValuesData(player);

        bf.Serialize(stream, data);
        stream.Close();
    }
    #endregion

    #region Loading: 
    public static UserValuesData LoadUser()
    {
        if (File.Exists(Application.persistentDataPath + "/user.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream($"{Application.persistentDataPath}/user.dat", FileMode.Open);

            UserValuesData data = (UserValuesData)bf.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            Debug.LogError("Your file has not yet been created!!!");
            return null;
        }
    }
    #endregion
}

[System.Serializable]
public class UserValuesData
{
    public int lvl;
    public int points;

    public float gradR;
    public float gradG;
    public float gradB;

    public bool repeatLvl;

    public string npcN1;
    public string npcN2;
    public string npcN3;
    public string npcN4;
    public string npcN5;
    public string npcN6;
    public string npcEnemy;

    public int premPts1;
    public int premPts2;
    public int premPts3;
    public int premPts4;
    public int premPts5;
    public int premPts6;

    public int playerPremPts;
    public string playerName;


    public bool inLeague;

    public string cupName;
    public bool winL1;
    public bool winL2;
    public bool winL3;
    public bool winL4;

    public float kickPower;
    public float moveSpeed;
    public float bonusSize;

    public int speedLvl;
    public int sizeLvl;
    public int kickLvl;

    public bool soundOn;
    public bool bonus;
    public int timeYear;
    public int timeMonth;
    public int timeDay;
    public int timeHour;
    public int timeMinute;
    public int timeSecond;

    public bool bonusFix;

    public List<int> bodyUnlocked = new List<int>();
    public int bodyId;
    public UserValuesData(SaveData player)
    {
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
