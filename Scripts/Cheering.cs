using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheering : MonoBehaviour
{
    #region Singleton
    public static Cheering instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }
    #endregion

    [Header("Cheer:")]
    [SerializeField] Animator[] anims;

    public void Goal()
    {
        foreach(var a in anims)
        {
            a.Play("goal");
        }
    }
}
