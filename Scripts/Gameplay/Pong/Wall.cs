using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using EZCameraShake;

public class Wall : MonoBehaviour
{
    private Ball ball;
    //private Transform trail;

    [Header("Wall Owner:")]
    public bool playerWall;
    public bool endWall;


    [Header("VFX:")]
    public GameObject vfx;

    AudioSource aSource;
    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision Ball)
    {
        if(Ball.transform.GetComponent<Ball>() != null)
        {
            ball = Ball.transform.GetComponent<Ball>();
            if (endWall)
            {
                Handheld.Vibrate();
                //Ball:
                UIController.instance.SetPlayerPts(!playerWall, 8);
                Instantiate(vfx, ball.transform.position, Quaternion.identity);
                CameraShaker.Instance.ShakeOnce(7, 5, .2f, 1.25f);

                //Add bonus points to the playerWall opponent!!! +10

                //Add points to XO board based on who's wall it was...
                GameController.instance.RandomField(!playerWall, ball);

                Cheering.instance.Goal();

                if (!GameController.instance.win && SoundController.instance.soundOn && !aSource.isPlaying)
                    aSource.Play();
            }
            else
            {
                Vector3 point = Ball.contacts[0].point;
                Instantiate(vfx, point, vfx.transform.rotation);
            }

        }
    }
}
