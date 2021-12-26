using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonSfx : MonoBehaviour
{
    private AudioSource aSource;
    private void Awake()
    {
        aSource = GetComponent<AudioSource>();

        if (SoundController.instance.soundOn)
            aSource.Play();
    }
}
