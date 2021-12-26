using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class League : MonoBehaviour
{

    [Header("Competitor Names:")]
    public Text pName;  //Player name : D
    public Text[] cNames = new Text[6];

    [Header("Competitor List:")]
    public RectTransform[] competitors;

    [Header("Positions:")]
    public Vector2[] winPos = new Vector2[3];
    public Vector2[] winPosNpc = new Vector2[2];
    public RectTransform player;
    [Space]
    [Header("Leaderboard Data:")]
    [SerializeField] Vector2[] mapPos = new Vector2[4];
    public RectTransform leaderboard;

    [SerializeField] Text cupName;


    private void Start()
    {
        Timing.RunCoroutine(_SetNames().CancelWith(gameObject));
    }
    IEnumerator<float> _SetNames()
    {
        yield return Timing.WaitForSeconds(1.5f);
        SetNames();
    }
    void SetNames()
    {
        pName.text = SaveData.instance.playerName;

        if (!SaveData.instance.repeatLvl && SaveData.instance.lvl % 10 == 7)
        {
            int cup = ((SaveData.instance.lvl - 7) / 10) > NameList.instance.cupNames.Length
                ?
                Random.Range(0, NameList.instance.cupNames.Length) : SaveData.instance.lvl / 10;
            cupName.text = NameList.instance.cupNames[cup];

            for (int i = 0; i < cNames.Length; i++)
            {
                int nameId = Random.Range(0, NameList.instance.nameList.Length);
                cNames[i].text = NameList.instance.nameList[nameId];
            }
            SaveData.instance.npcN1 = cNames[0].text;
            SaveData.instance.npcN2 = cNames[1].text;
            SaveData.instance.npcN3 = cNames[2].text;
            SaveData.instance.npcN4 = cNames[3].text;
            SaveData.instance.npcN5 = cNames[4].text;
            SaveData.instance.npcN6 = cNames[5].text;

            SaveData.instance.cupName = cupName.text;
            SaveData.instance.SaveGame();
        }
        else
        {
            cNames[0].text = SaveData.instance.npcN1;
            cNames[1].text = SaveData.instance.npcN2;
            cNames[2].text = SaveData.instance.npcN3;
            cNames[3].text = SaveData.instance.npcN4;
            cNames[4].text = SaveData.instance.npcN5;
            cNames[5].text = SaveData.instance.npcN6;

            cupName.text = SaveData.instance.cupName;
        }
        SetRankings();
    }

    void SetRankings()
    {
        //Eh... man... here we go again... -.-
        switch (SaveData.instance.lvl % 10)
        {
            case 7:
                SaveData.instance.npcEnemy = cNames[0].text;
                break;

            case 8:
                player.anchoredPosition = winPos[0];

                int randWin = Random.Range(0, 2);
                if(randWin == 0)
                {
                    SaveData.instance.winL1 = true;
                    competitors[0].anchoredPosition = winPosNpc[0];
                    SaveData.instance.npcEnemy = cNames[1].text;
                } else {
                    SaveData.instance.winL2 = true;
                    competitors[1].anchoredPosition = winPosNpc[0];
                    SaveData.instance.npcEnemy = cNames[2].text;
                }
                LeanTween.move(leaderboard, mapPos[1], .75f);
                break;

            case 9:
                player.anchoredPosition = winPos[1];

                if (SaveData.instance.winL1) competitors[0].anchoredPosition = winPosNpc[0];
                else if (SaveData.instance.winL2) competitors[1].anchoredPosition = winPosNpc[0];

                int randWin2 = Random.Range(0, 2);
                if (randWin2 == 0)
                {
                    SaveData.instance.winL3 = true;
                    competitors[2].anchoredPosition = winPosNpc[1];
                    SaveData.instance.npcEnemy = cNames[3].text;
                } else {
                    SaveData.instance.winL4 = true;
                    competitors[3].anchoredPosition = winPosNpc[1];
                    SaveData.instance.npcEnemy = cNames[4].text;
                }

                LeanTween.move(leaderboard, mapPos[2], .75f);
                break;

            case 0:
                player.anchoredPosition = winPos[2];
                if (SaveData.instance.winL1) competitors[0].anchoredPosition = winPosNpc[0];
                else if (SaveData.instance.winL2) competitors[1].anchoredPosition = winPosNpc[0];

                if (SaveData.instance.winL3) competitors[2].anchoredPosition = winPosNpc[1];
                else if (SaveData.instance.winL4) competitors[3].anchoredPosition = winPosNpc[1];

                SaveData.instance.npcEnemy = cNames[5].text;
                LeanTween.move(leaderboard, mapPos[3], .75f);
                break;
        }
        SaveData.instance.SaveGame();
        Timing.RunCoroutine(_HideLeague().CancelWith(gameObject));
    }

    IEnumerator<float> _HideLeague()
    {
        yield return Timing.WaitForSeconds(7.35f);
        LeanTween.scale(gameObject, Vector3.zero, .5f).setEaseInBack();
        yield return Timing.WaitForSeconds(.65f);
        gameObject.SetActive(false);
    }
}