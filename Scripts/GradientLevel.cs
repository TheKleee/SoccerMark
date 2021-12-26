using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class GradientLevel : MonoBehaviour
{
    SpriteRenderer gradCol;
    Image gradImg;

    [Header("Check Cur Level:")]
    public bool repeat;

    [HideInInspector] public Color col;

    [Header("Is Image:")]
    public bool isImage;
    [Range(0.0f, 1.0f)] public float a = 1.0f;
    

    private void Start()
    {
        if (!isImage)
        {
            gradCol = GetComponent<SpriteRenderer>();
            Timing.RunCoroutine(_SetGradient().CancelWith(gameObject));
        }
        else
        {
            gradImg = GetComponent<Image>();
            Timing.RunCoroutine(_SetImgGrad().CancelWith(gameObject));
        }

    }
    IEnumerator<float> _SetImgGrad()
    {
        yield return Timing.WaitForSeconds(.15f);
        SetImgCol();
    }

    void SetImgCol()
    {
        col.r = SaveData.instance.gradR;
        col.g = SaveData.instance.gradG;
        col.b = SaveData.instance.gradB;
        col.a = a;
        gradImg.color = col;
    }


    IEnumerator<float> _SetGradient()
    {
        yield return Timing.WaitForSeconds(.1f);
        repeat = SaveData.instance.repeatLvl ? true : false;

        SetGradCol(repeat);
    }
    
    void SetGradCol(bool repeatCur)
    {
        if (!repeatCur)
        {
            if (SaveData.instance.lvl % 10 == 1 || SaveData.instance.lvl % 10 == 7)
            {
                col.r = Random.Range(0.0f, 1.0f);
                col.g = Random.Range(0.0f, 1.0f);
                col.b = Random.Range(0.0f, 1.0f);
                col.a = 1.0f;
                gradCol.color = col;

                SaveData.instance.gradR = gradCol.color.r;
                SaveData.instance.gradG = gradCol.color.g;
                SaveData.instance.gradB = gradCol.color.b;
                SaveData.instance.SaveGame();
            } else {
                col.r = SaveData.instance.gradR;
                col.g = SaveData.instance.gradG;
                col.b = SaveData.instance.gradB;
                col.a = 1.0f;
                gradCol.color = col;
            }
        } else {
            col.r = SaveData.instance.gradR;
            col.g = SaveData.instance.gradG;
            col.b = SaveData.instance.gradB;
            col.a = 1.0f;
            gradCol.color = col;
        }        
    }
}
