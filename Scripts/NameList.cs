using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameList : MonoBehaviour
{
    public static NameList instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }

    [Header("Name List:")]
    public string[] nameList = new string[100];

    [Header("Cup Names:")]
    public string[] cupNames;
}
