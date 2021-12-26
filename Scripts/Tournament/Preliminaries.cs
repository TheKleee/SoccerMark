using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

[RequireComponent(typeof(UIAnim))]
public class Preliminaries : MonoBehaviour
{
    [Header("Crown:")]
    public GameObject crown;

    [Header("Rounds Left:")]
    public Text rounds;

    int[] premPts = new int[6];
    string[] npcNames = new string[6];
    int playerPremPts;
    string playerName;

    [Header("Check Cur Level:")]
    public bool repeat;

    float delay;
    private void Start()
    {
        delay = GetComponent<UIAnim>().delay;
        Timing.RunCoroutine(_SetPts().CancelWith(gameObject));
    }
    IEnumerator<float> _SetPts()
    {
        yield return Timing.WaitForSeconds(.1f);
        repeat = SaveData.instance.repeatLvl ? true : false;

        SetPts(repeat);  //Test...
    }
    int randName;

    [Header("UI Elements:")]
    public Text[] npcN = new Text[6];
    public Text[] npcP = new Text[6];
    [Space]
    public Text playerN;
    public Text playerP;

    private List<int> ptsParent = new List<int>();

    void SetPts(bool repeatLvl)
    {
        LoadSaveData();

        rounds.text = $"Rounds:\n{7 - (SaveData.instance.lvl % 10)}";   //Testing :C

        crown.SetActive(false);
        crown.transform.localScale = Vector3.zero;
        if (!repeatLvl)
        {
            playerPremPts += Random.Range(5, 9) + 3 * (SaveData.instance.lvl % 10);
            SaveData.instance.playerPremPts = playerPremPts;
            playerN.text = SaveData.instance.playerName = playerName;
            playerP.text = playerPremPts.ToString();

            ptsParent.Add(playerPremPts);

            for (int i = 0; i < premPts.Length; i++)
            {
                premPts[i] += Random.Range(7, 16);
                randName = Random.Range(0, NameList.instance.nameList.Length);

                if(SaveData.instance.lvl % 10 == 1)
                    npcNames[i] = NameList.instance.nameList[randName];

                npcN[i].text = npcNames[i];
                npcP[i].text = premPts[i].ToString();
                ptsParent.Add(premPts[i]);
            }
            SaveData.instance.premPts1 = premPts[0];
            SaveData.instance.premPts2 = premPts[1];
            SaveData.instance.premPts3 = premPts[2];
            SaveData.instance.premPts4 = premPts[3];
            SaveData.instance.premPts5 = premPts[4];
            SaveData.instance.premPts6 = premPts[5];
            
            SaveData.instance.npcN1 = npcNames[0];
            SaveData.instance.npcN2 = npcNames[1];
            SaveData.instance.npcN3 = npcNames[2];
            SaveData.instance.npcN4 = npcNames[3];
            SaveData.instance.npcN5 = npcNames[4];
            SaveData.instance.npcN6 = npcNames[5];

            SaveData.instance.SaveGame();
        }
        else
        {
            ptsParent.Add(playerPremPts);
            for (int i = 0; i < premPts.Length; i++)
            {
                npcN[i].text = npcNames[i];
                npcP[i].text = premPts[i].ToString();
                ptsParent.Add(premPts[i]);
            }
            playerN.text = SaveData.instance.playerName;
            playerP.text = playerPremPts.ToString();
        }
        ShuffleLeaderboard();
    }

    void LoadSaveData()
    {
        premPts[0] = SaveData.instance.premPts1;
        premPts[1] = SaveData.instance.premPts2;
        premPts[2] = SaveData.instance.premPts3;
        premPts[3] = SaveData.instance.premPts4;
        premPts[4] = SaveData.instance.premPts5;
        premPts[5] = SaveData.instance.premPts6;

        npcNames[0] = SaveData.instance.npcN1;
        npcNames[1] = SaveData.instance.npcN2;
        npcNames[2] = SaveData.instance.npcN3;
        npcNames[3] = SaveData.instance.npcN4;
        npcNames[4] = SaveData.instance.npcN5;
        npcNames[5] = SaveData.instance.npcN6;

        playerPremPts = SaveData.instance.playerPremPts;
        playerName = SaveData.instance.playerName;
    }

    #region Shuffle:
    [Header("Shuffle:")]
    public List<ShuffleItem> shuffleItem = new List<ShuffleItem>();
    [SerializeField] Vector2[] shufflePos = new Vector2[7]; //Only Y value; from 700 to -500; by 200 range.
    public void ShuffleLeaderboard()
    {

        SaveData.instance.npcEnemy = npcNames[(SaveData.instance.lvl % 10) - 1];
        Timing.RunCoroutine(_Shuffle().CancelWith(gameObject));
    }
    IEnumerator<float> _Shuffle()
    {
        yield return Timing.WaitForSeconds(delay + .5f);
        //Start Shuffling...
        for (int i = 0; i < shuffleItem.Count; i++)
        {
            shuffleItem[i].shuffleID = ptsParent[i];
        }
        shuffleItem.Sort((p1, p2) => p1.shuffleID.CompareTo(p2.shuffleID));
        for (int i = 0; i < shufflePos.Length; i++)
        {
            LeanTween.moveLocal(shuffleItem[i].gameObject, shufflePos[i], 0.75f);
            
        }
        crown.SetActive(true);
        Timing.RunCoroutine(_Hide().CancelWith(gameObject));
    }
    IEnumerator<float> _Crown()
    {
        yield return Timing.WaitForSeconds(.75f);
        LeanTween.scale(crown, Vector3.one, 1.25f).setEaseOutElastic();
    }
    #endregion

    IEnumerator<float> _Hide()
    {
        Timing.RunCoroutine(_Crown().CancelWith(gameObject));
        yield return Timing.WaitForSeconds(7.35f);
        LeanTween.scale(gameObject, Vector3.zero, .5f).setEaseInBack();
        yield return Timing.WaitForSeconds(.65f);
        gameObject.SetActive(false);
    }
}
