using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MEC;

public class Bonus : MonoBehaviour
{
    [HideInInspector] public int openThree = 3; //Just 3 >:D

    [Header("Bonus Text:")]
    public Text bonusTxt;

    [Header("Bonuses:")]
    public GameObject[] bonuses = new GameObject[3];
    public Text[] ptsTxt = new Text[3];

    public void GetBonus()
    {
        if (openThree > 0)
        {
            openThree--;
            bonusTxt.text = $"Open: {openThree}";

            int amount = Random.Range(25, 251);
            int checkA = amount % 5 == 0 ? amount : Mathf.FloorToInt(amount / 5) * 5;

            SaveData.instance.points += checkA;

            switch (openThree)
            {
                case 2:
                    bonuses[0].SetActive(true);
                    ptsTxt[0].text = $"+{checkA}";
                    break;

                case 1:
                    bonuses[1].SetActive(true);
                    ptsTxt[1].text = $"+{checkA}";
                    break;

                case 0:
                    bonuses[2].SetActive(true);
                    ptsTxt[2].text = $"+{checkA}";
                    break;
            }
            SaveData.instance.SaveGame();
            Timing.RunCoroutine(_CheckLevelChange().CancelWith(gameObject));
        }
    }
    IEnumerator<float> _CheckLevelChange()
    {
        if(openThree == 0)
        {
            yield return Timing.WaitForSeconds(1.25f);
            SceneManager.LoadSceneAsync(0);
        }
    }
}
