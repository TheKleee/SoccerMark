using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

[RequireComponent(typeof(GameController))]
public class SkinsManager : MonoBehaviour
{
    #region Private stuff >:)
    [Space]
    [SerializeField] private List<int> skinNum = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    private int price;
    #endregion

    [Header("Skins Layout:")]
    public GameObject skinsLayout;  //Set active to true/false to view/hide skins...

    [Header("Buy Price:")]
    public Text buyTxt;
    [SerializeField] private Button buyBtn;

    [Header("Selected And Unlocked:")]
    public GameObject[] Selected = new GameObject[9];
    public GameObject[] Unlocked = new GameObject[9];
    public Button[] Skins = new Button[9];

    
    public void ShowSkinsLayout()
    {
        UIController.instance.Skins();

        //Set the skins:
        for (int i = 0; i < SaveData.instance.bodyUnlocked.Count; i++)
        {
            skinNum.Remove(SaveData.instance.bodyUnlocked[i]);
        }

        CheckSkins();
        SetPrice();
    }
    /// <summary>
    /// Call this when you need to see available skins...
    /// </summary>
    public void CheckSkins()
    {
        //Activate the selected marker
        foreach (GameObject s in Selected)
            s.SetActive(false);
        Selected[SaveData.instance.bodyId].SetActive(true);

        //Turn On The Unlocked Skin Buttons...
        for (int i = 0; i < Skins.Length; i++)
        {
            if (SaveData.instance.bodyUnlocked.Contains(i))
            {
                Skins[i].interactable = true;
            }
        }
    }
    /// <summary>
    /// Call this whenever you need to change the price...
    /// </summary>
    public void SetPrice()
    {
        //Disable the buy button if there are no more skins to buy!!!
        if (skinNum.Count == 0)
            buyTxt.transform.parent.gameObject.SetActive(false);

        //Set the price:
        price = 300 * SaveData.instance.bodyUnlocked.Count;
        buyTxt.text = price.ToString();
    }
    /// <summary>
    /// Call this when you purchase a skin...
    /// </summary>
    public void SaveChanges()
    {
        //Save Price:
        SaveData.instance.SaveGame();
    }

    //Call this from another script >:O
    public void SelectSkin(int skinId)
    {
        foreach (GameObject s in Selected)
            s.SetActive(false);

        Selected[skinId].SetActive(true);

        //Save the selected skin id... :|
        SaveData.instance.bodyId = skinId;
        SaveData.instance.SaveGame();
    }

    #region Buying:
    public void BuySkin()
    {
        //Get a random skin...
        if (SaveData.instance.points > price)
            Timing.RunCoroutine(_SkinShuffle().CancelWith(gameObject));
    }
    IEnumerator<float> _SkinShuffle()
    {
        buyBtn.interactable = false;
        float dur = 2f;
        do
        {
            dur -= .2f;
            //Shuffle (Special xD)
            int shuffleId = Random.Range(0, skinNum.Count);
            int shuffle = skinNum[shuffleId];
            //Turn one on...
            Unlocked[shuffle].SetActive(true);
            yield return Timing.WaitForSeconds(.2f);
            //Turn it off...
            Unlocked[shuffle].SetActive(false);
            yield return Timing.WaitForSeconds(.2f);
            //Delay... do nothing :|
        } while (dur > 0);

        //Unlock the selected skin...
        int unlockedId = Random.Range(0, skinNum.Count);
        int unlockedSkin = skinNum[unlockedId];
        Unlocked[unlockedSkin].SetActive(true);
        yield return Timing.WaitForSeconds(.2f);
        Unlocked[unlockedSkin].SetActive(false);
        //Other Stuff xD
        skinNum.Remove(unlockedSkin);
        SaveData.instance.bodyUnlocked.Add(unlockedSkin);   //TestO ... like restO! >:\

        //Check if you have any more skins to unlock:
        if (skinNum.Count == 0)
            buyTxt.transform.parent.gameObject.SetActive(false);

        //Save:
        UIController.instance.BuySkin(price);
        SaveChanges();

        //Set a new Price...
        SetPrice();
        CheckSkins();

        buyBtn.interactable = true;
    }
    #endregion
}