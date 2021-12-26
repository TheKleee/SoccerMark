using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class UIAnim : MonoBehaviour
{
    [Header("Scale:")]
    public Vector2 scale = Vector2.one;

    [Header("Loop:")]
    public bool loop;

    [Space]
    public bool isLeague;
    public GameObject leagueBackground;
    public GameObject fakeBackground;

    [Header("Loop Duration:")]
    [Range(0.25f, 1.5f)] [SerializeField] private float dur = 1.25f;

    [Header("Start Delay:")]
    [Range(0.0f, 2.0f)] public float delay = 0;
    void OnEnable()
    {
        if (isLeague)
        {
            fakeBackground.SetActive(true);
            leagueBackground.SetActive(false);
            Timing.RunCoroutine(_League().CancelWith(gameObject));
        }

        if (!loop) GetComponent<RectTransform>().localScale = Vector2.zero;
        Timing.RunCoroutine(_Delay().CancelWith(gameObject));
    }

    IEnumerator<float> _Delay()
    {
        yield return Timing.WaitForSeconds(delay);
        if (loop)
            LeanTween.scale(gameObject, scale, dur).setLoopPingPong();
        else
        {
            GetComponent<RectTransform>().localScale = Vector2.zero;
            LeanTween.scale(gameObject, scale, dur).setEaseOutBounce();
        }
    }

    IEnumerator<float> _League()
    {
        yield return Timing.WaitForSeconds(delay + dur);
        fakeBackground.SetActive(false);
        leagueBackground.SetActive(true);
    }
}
