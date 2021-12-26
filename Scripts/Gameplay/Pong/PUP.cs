using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PUP : MonoBehaviour
{
    [Header("Power Type:")]
    public PowerType pType;

    [Header("Vfx:")]
    public GameObject vfx;

    float lifeSpan;
    Vector3 scale = new Vector3(1.25f, 1.25f, 1.25f);
    private void Start()
    {
        Instantiate(vfx, transform.position, Quaternion.identity);
        LeanTween.scale(gameObject, scale, .75f).setLoopPingPong();
        lifeSpan = Random.Range(2.75f, 5.25f);
        Timing.RunCoroutine(_LifeSpan().CancelWith(gameObject));
    }
    IEnumerator<float> _LifeSpan()
    {
        yield return Timing.WaitForSeconds(lifeSpan);
        Instantiate(vfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider ball)
    {
        if(ball.GetComponent<Ball>() != null)
        {
            Ball b = ball.GetComponent<Ball>();
            switch (pType)
            {
                case PowerType.twice:
                    GameController.instance.Twice();
                    break;

                case PowerType.speed:
                    b.Speed();
                    break;

                case PowerType.large:
                    b.Large();
                    break;
            }
            Destroy(gameObject);
        }
    }
}
