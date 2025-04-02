using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // needed?
using UnityEngine;

public class SoundEffects : MonoBehaviour
{

    public UIController uiController; // Reference to UIController
    private Animator animator; // Reference to the Animator

    public AudioSource audioSFX;
    public AudioClip BG_NightSFX;
    public AudioClip BG_DaySFX;


    // Start is called before the first frame update
    void Start()
    {
        //play title menu music
    }

    // Update is called once per frame
    void Update()
    {
        //if game is in night mode, run night music
        if (uiController.isNightPhase) {
            audioSFX.clip = BG_NightSFX;
            audioSFX.Play();
        }

        //if day time, play day music

        //customer speaks when arriving

        //customer happy when order done

        //customer grunts when order NOT done

        //watering for watering can

        //spray for flies

        //harvesting 

        //planting
    }
}
