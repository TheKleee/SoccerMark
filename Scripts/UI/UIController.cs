using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using System;
using Yodo1.MAS;

public class UIController : MonoBehaviour
{
    #region Instance:
    public static UIController instance;
    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    [Header("Menu PTS:")]
    public Text totalPts;   //The menu one... :|
    [Space]
    public Text winTotalPts;

    [Header("UI:")]
    public GameObject menu;
    public GameObject gameplay;
    public GameObject wld;  //Win, Lose, Draw
    [Space]
    public GameObject joysticMenuParent;

    [Header("Tournament:")]
    public GameObject[] tour = new GameObject[2]; //Preliminaries & League
    private void Start()
    {
        CheckUpgLvls();

        joysticMenuParent.SetActive(true);
        menu.SetActive(true);

        if (SaveData.instance.lvl > 0)
        {
            if (SaveData.instance.lvl % 3 == 0)
            {
                Yodo1U3dMas.SetInterstitialAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
                {
                    Debug.Log("[Yodo1 Mas] InterstitialAdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
                    switch (adEvent)
                    {
                        case Yodo1U3dAdEvent.AdClosed:
                            Debug.Log("[Yodo1 Mas] Interstital ad has been closed.");
                            break;
                        case Yodo1U3dAdEvent.AdOpened:
                            Debug.Log("[Yodo1 Mas] Interstital ad has been shown.");
                            break;
                        case Yodo1U3dAdEvent.AdError:
                            Debug.Log("[Yodo1 Mas] Interstital ad error, " + error.ToString());
                            break;
                    }
                });
                bool isLoaded = Yodo1U3dMas.IsInterstitialAdLoaded();
                Yodo1U3dMas.ShowInterstitialAd();
            }
            if (!SaveData.instance.inLeague) tour[0].SetActive(true);
            else tour[1].SetActive(true);
        } else
        {
            SaveData.instance.npcEnemy = "Tutorial";
        }

        ReadPlayerName();

        if (SaveData.instance.timeYear > 0)
            timeNow = new DateTime(SaveData.instance.timeYear,
                SaveData.instance.timeMonth,
                SaveData.instance.timeDay,
                SaveData.instance.timeHour,
                SaveData.instance.timeMinute,
                SaveData.instance.timeSecond);
        else
            ConstructTimeNow();

        WaitBonus();
    }

    #region Main Stuff:
    public void OnValueChangedCheck()
    {
        GameController.instance.StartMatch();
        menu.SetActive(false);
        gameplay.SetActive(true);
        SetLevel();
    }

    public void GameEnded()
    {
        gameplay.SetActive(false);
        joysticMenuParent.SetActive(false);
        wld.SetActive(true);

        //Add things to wld...
    }
    #endregion

    [Header("Level:")]
    public Text[] lvl = new Text[3];
    [Header("Points:")]
    public Text[] pts;              //0 - player, 1 - npc
    public int[] totalValue;
    public GameObject[] ptsParent;  //Same as above xD
    [Header("Scale Size:")]
    public Vector2 scale;

    #region Gameplay:

    public void SetLevel()
    {
        if (SaveData.instance.lvl > 0) lvl[0].text = SaveData.instance.lvl.ToString();
        else lvl[0].text = "T";

        playerName.text = SaveData.instance.playerName;
        npcName.text = SaveData.instance.npcEnemy;
    }

    public void SetPlayerPts(bool isPlayer, int value)
    {
        int P = isPlayer ? 0 : 1;
        totalValue[P] += value;
        pts[P].text = totalValue[P].ToString();
        LeanTween.scale(ptsParent[P], scale, .25f).setEaseOutBack();
        Timing.RunCoroutine(_ScaleBack(P).CancelWith(gameObject));
    }
    IEnumerator<float> _ScaleBack(int p)
    {
        yield return Timing.WaitForSeconds(.35f);
        LeanTween.scale(ptsParent[p], Vector2.one, .25f).setEaseOutBack();
    }
    #endregion

    #region Win:
    [Header("Who Won:")]
    public GameObject[] win = new GameObject[3];    //0 - player, 1 - npc, 2 - draw

    [Header("Player Win Points:")]
    public Text playerWonPts;
    public Text npcWonPts;

    [Header("Names:")]
    public Text playerName;
    public Text npcName;

    public void PlayerWon()
    {
        WLD();
        win[0].SetActive(true);
        if (SaveData.instance.lvl > 0) lvl[1].text = SaveData.instance.lvl.ToString();
        else lvl[0].text = "T";

        playerWonPts.GetComponent<RectTransform>().localScale = Vector2.zero;
        npcWonPts.GetComponent<RectTransform>().localScale = Vector2.zero;
        winTotalPts.GetComponent<RectTransform>().localScale = Vector2.zero;

        playerWonPts.text = totalValue[0].ToString();
        npcWonPts.text = $"+{totalValue[1]}";
        winTotalPts.text = (totalValue[0] + totalValue[1]).ToString();

        SaveData.instance.points += (totalValue[0] + totalValue[1]);
        SaveData.instance.repeatLvl = false;
        SaveData.instance.lvl++;

        if (SaveData.instance.lvl % 10 < 7 && SaveData.instance.lvl % 10 > 0) SaveData.instance.inLeague = false;
        else
        {
            SaveData.instance.inLeague = true;
            SaveData.instance.premPts1 =
            SaveData.instance.premPts2 =
            SaveData.instance.premPts3 =
            SaveData.instance.premPts4 =
            SaveData.instance.premPts5 =
            SaveData.instance.premPts6 =
            SaveData.instance.playerPremPts = 0;
        }

        SaveData.instance.SaveGame();

        Timing.RunCoroutine(_PlayerWin().CancelWith(gameObject));
    }
    private Vector2 ptsScale = new Vector2(0.5714285f, 0.5714285f);
    IEnumerator<float> _PlayerWin()
    {
        yield return Timing.WaitForSeconds(.75f);
        LeanTween.scale(playerWonPts.gameObject, ptsScale, .5f).setEaseOutBack();
        yield return Timing.WaitForSeconds(.35f);
        LeanTween.scale(npcWonPts.gameObject, ptsScale, .5f).setEaseOutBack();
        yield return Timing.WaitForSeconds(.35f);
        LeanTween.scale(winTotalPts.gameObject, Vector2.one, .5f).setEaseOutBack();
    }

    public void NpcWon()
    {
        WLD();
        win[1].SetActive(true);
        if (SaveData.instance.lvl > 0) lvl[2].text = SaveData.instance.lvl.ToString();
        else lvl[0].text = "T";

        SaveData.instance.repeatLvl = true;
        SaveData.instance.SaveGame();
    }

    public void Draw()
    {
        WLD();
        win[2].SetActive(true);
        if (SaveData.instance.lvl > 0) lvl[3].text = SaveData.instance.lvl.ToString();
        else lvl[0].text = "T";

        SaveData.instance.repeatLvl = false;
        SaveData.instance.points += totalValue[0];
        SaveData.instance.lvl++;
        SaveData.instance.SaveGame();
    }

    public void WLD()
    {
        wld.SetActive(true);
    }

    #endregion

    #region Menu:
    public void ReadMenuData()
    {
        totalPts.text = SaveData.instance.points.ToString();
    }
    #endregion

    #region Upgrades:

    [Header("Player:")]
    public Player player;
    List<Player> p = new List<Player>();
    [Space]
    public Text speedCost;
    public Text sizeCost;
    public Text kickCost;

    [Header("Upgrade Info:")]
    public Text speedTxt;
    public Text sizeTxt;
    public Text kickTxt;
    [Header("Color Max:")]
    public Image colSpeedMax;
    public Image colSizeMax;
    public Image colKickMax;
    [Space]
    public Color maxCol;

    public void CheckUpgLvls()
    {
        p.AddRange(FindObjectsOfType<Player>());
        foreach (var pl in p)
        {
            if (pl.type == Type.player)
            {
                player = pl;
                break;
            }
        }
        speedCost.text = (SaveData.instance.speedLvl * 20).ToString();
        sizeCost.text = (SaveData.instance.sizeLvl * 20).ToString();
        kickCost.text = (SaveData.instance.kickLvl * 20).ToString();

        speedTxt.text = SaveData.instance.speedLvl.ToString();
        sizeTxt.text = SaveData.instance.sizeLvl.ToString();
        kickTxt.text = SaveData.instance.kickLvl.ToString();

        totalPts.text = SaveData.instance.points.ToString();

        if (SaveData.instance.speedLvl == 10)
        {
            colSpeedMax.color = maxCol;
            speedCost.text = "MAX";
        }
        if (SaveData.instance.sizeLvl == 10)
        {
            colSizeMax.color = maxCol;
            sizeCost.text = "MAX";
        }
        if (SaveData.instance.kickLvl == 10)
        {
            colKickMax.color = maxCol;
            kickCost.text = "MAX";
        }
        player.Upgrades();
    }
    public void SetSpeed()
    {
        if (SaveData.instance.speedLvl < 10)
        {
            player.SetMoveSpeed(SaveData.instance.speedLvl * 20);
        }
    }
    public void SetSize()
    {
        if (SaveData.instance.sizeLvl < 10)
        {
            player.SetBonusSize(SaveData.instance.sizeLvl * 20);
        }
    }
    public void SetKick()
    {
        if (SaveData.instance.kickLvl < 10)
        {
            player.SetKickPower(SaveData.instance.kickLvl * 20);
        }
    }

    #endregion

    #region Options:

    [Header("Options:")]
    public GameObject options;

    public void Options()
    {
        options.SetActive(!options.activeSelf);
        skins.SetActive(false);
    }

    [Header("Player Name:")]
    [SerializeField] private InputField pName;

    public void SetPlayerName()
    {
        SaveData.instance.playerName = pName.text;
        SaveData.instance.SaveGame();
        ReadPlayerName();
    }

    [Header("Tournament:")]
    public Preliminaries prem;
    public League league;

    void ReadPlayerName()
    {
         league.pName.text = pName.text = prem.playerN.text = playerName.text = SaveData.instance.playerName;
    }
    #endregion

    #region Bonus:
    [Header("Timer:")]
    public GameObject timer;
    public Text timerTxt;
    [Space]
    public GameObject exc;

    [Header("Bonus:")]
    public Button bonusBtn;
    public Text bonusTxt;   //SaveData bonusCash
    [Space]
    [SerializeField] Color bonusCol;
    public Image cashBonus;
    private void BonusCollectable()
    {
        //It's ticking out otherwise...
        SaveData.instance.bonus = true;
        bonusBtn.interactable = true;
        cashBonus.color = Color.white;
        timer.SetActive(false);
        exc.SetActive(true);
        SaveData.instance.SaveGame();
    }

    private void WaitBonus()
    {
        bonusTxt.text = $"{SaveData.instance.lvl * 10 + 200}";
        SaveData.instance.bonus = false;
        cashBonus.color = bonusCol;
        bonusBtn.interactable = false;
        timer.SetActive(true);
        exc.SetActive(false);
        Timing.RunCoroutine(_TimerDetector().CancelWith(gameObject));
    }
    DateTime timeNow;
    TimeSpan lastSession;
    int timeElapsed;
    IEnumerator<float> _TimerDetector()
    {
        lastSession = DateTime.Now - timeNow;
        timeElapsed = 1800 - (int)lastSession.TotalSeconds;
        SaveData.instance.SaveGame();

        MobileNotifs.instance.Norif(timeElapsed);
        if (lastSession.TotalSeconds <= 1800)
        {
            while (timeElapsed > 0)
            {
                if (SaveData.instance.bonus)
                    break;

                lastSession = DateTime.Now - timeNow;
                timeElapsed -= 1;
                lastSession = TimeSpan.FromSeconds(timeElapsed);
                timerTxt.text = $"{lastSession.Minutes:00}:{lastSession.Seconds:00}";
                yield return Timing.WaitForSeconds(1f);
            }
        }
        BonusCollectable();
    }

    void ConstructTimeNow()
    {
        //Collect The cash
        timeNow = DateTime.Now;
        //Deconstruct timeNow:
        SaveData.instance.timeYear = timeNow.Year;
        SaveData.instance.timeMonth = timeNow.Month;
        SaveData.instance.timeDay = timeNow.Day;
        SaveData.instance.timeHour = timeNow.Hour;
        SaveData.instance.timeMinute = timeNow.Minute;
        SaveData.instance.timeSecond = timeNow.Second;
    }

    public void CollectBonus()
    {
        ConstructTimeNow();

        SaveData.instance.points += SaveData.instance.lvl * 10 + 200;
        ReadMenuData();
        WaitBonus();
    }
    #endregion

    #region Skins:
    [Header("Skins:")]
    public GameObject skins;

    public void Skins()
    {
        skins.SetActive(!skins.activeSelf);
        options.SetActive(false);
    }
    #endregion

    #region Skins:
    public void BuySkin(int price)
    {
        SaveData.instance.points -= price;

        totalPts.text = (SaveData.instance.points).ToString();
        SaveData.instance.SaveGame();
    }
    #endregion
    private void OnApplicationQuit()
    {
        SaveData.instance.repeatLvl = true;
    }
}
