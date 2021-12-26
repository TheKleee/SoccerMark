using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedSkin : MonoBehaviour
{
    [Header("Skin Id:")]
    public int skinId;

    [Header("Skins Manager:")]
    public SkinsManager sManager;

    public void SelectSkin()
    {
        sManager.SelectSkin(skinId);
        sManager.SaveChanges();
    }
}