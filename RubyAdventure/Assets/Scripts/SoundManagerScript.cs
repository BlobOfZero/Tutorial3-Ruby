using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip winSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        // Draws sound effect from resources folder. (NAME FOLDER "Resources"!!)
        winSound = Resources.Load<AudioClip>("WinSound");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "WinSound":
                audioSrc.PlayOneShot(winSound);
                break;
        }
    }
}