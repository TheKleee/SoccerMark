using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Glove : MonoBehaviour
{
    [Header("Rect Transform:")]
    [SerializeField] private RectTransform rect;

    [Header("X Local Destination")]
    public float xLocal = 300;

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        LeanTween.moveLocalX(gameObject, xLocal, 1.25f).setLoopPingPong();  //Test :S
    }
}
