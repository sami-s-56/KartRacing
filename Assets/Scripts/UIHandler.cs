using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Start Countdown")]
    [SerializeField] List<Sprite> numbers = new List<Sprite>();
    [SerializeField] List<AudioClip> beeps = new List<AudioClip>();
    [SerializeField] Image countdownDisplay;
    [SerializeField] AudioSource beepSource;

    [SerializeField] RaceHandler raceHandler;

    void Start()
    {
        StartCoroutine("StartCountDown");
    }

    IEnumerator StartCountDown()
    {
        for (int c = 0; c < numbers.Count; c++) 
        {
            //Play Audio
            beepSource.clip = beeps[c];
            beepSource.Play();

            //Play Animation
            countdownDisplay.sprite = numbers[c];
            

            yield return new WaitForSeconds(1);
        }
        countdownDisplay.enabled = false;
        raceHandler.isRaceOn = true;
    }

}
