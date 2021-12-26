using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    #region Singleton
    public static SoundController instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }
    #endregion

    [HideInInspector] public bool soundOn = true;

    [Header("Sound Icons:")]
    public Sprite[] icons = new Sprite[2];  //For on and off...
    [Space]
    public Image sfxImg;    //Change sfxImg.sprite based on the soundOn bool => icons[0/1]
    
    #region Sound Funs:
    private void Start()
    {
        soundOn = SaveData.instance.soundOn;
        CheckSound();
    }
    public void SetSound()
    {
        soundOn = !soundOn;
        CheckSound();
    }
    private void CheckSound()
    {
        sfxImg.sprite = soundOn ? icons[0] : icons[1];
        SaveData.instance.soundOn = soundOn;
        SaveData.instance.SaveGame();
    }
    #endregion
}
