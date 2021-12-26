using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusIconChange : MonoBehaviour
{
    [Header("Icon:")]
    public Sprite icon;

    [Header("Bonus")]
    public Bonus bonus;

    public void ChangeIcon()
    {
        if(bonus.openThree > 0)
            GetComponent<Image>().sprite = icon;
    }
}
