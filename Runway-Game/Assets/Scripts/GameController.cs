﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{

    public Transform planeTransform;
    public TextMeshProUGUI heightText;
    public Text countdownText;
    private int height;
    private bool gameStarted = false;
    private bool countdown = true;
    public GameObject countdownPage;
    public Rigidbody2D planeRigid;
    private int secondsToGo = 3, timeToGo = 3;
    public GameObject background;
    private AudioSource menuAudio, gameAudio;
    public AudioSource musiclvl1, musiclvl2, musiclvl3;
    public Text muteText;
    private int loops = 0;
    private int level;

    // Update is called once per frame

    private void Start()
    {
        level = PlayerPrefs.GetInt("level", 1);
        InitialiseMusic();
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            InitialiseGame();
            DisplayHeight();
        }
        else if (countdown)
        {
            countdownPage.SetActive(true);
            // Hardcoded variable to stop plane moving during countdown
            planeRigid.simulated = false;
            StartCoroutine("CountdownTimer");
            countdown = false;
        }
    }

    private void InitialiseMusic()
    {
        if (GameObject.FindWithTag("music") != null)
            menuAudio = GameObject.FindWithTag("music").GetComponent<AudioSource>();
        if (menuAudio != null)
            menuAudio.Stop();
        switch (level)
        {
            default:
            case 1:
                gameAudio = musiclvl1;
                break;

            case 2:
                gameAudio = musiclvl2;
                break;

            case 3:
                gameAudio = musiclvl3;
                break;

        }
        if (PlayerPrefs.GetInt("music", 1) == 1)
        {
            muteText.text = "Mute";
            gameAudio.volume = 1;
        }
        else
        {
            muteText.text = "Unmute";
            gameAudio.volume = 0;
        }
    }

    private void InitialiseGame()
    {
        if (gameAudio != null && gameAudio.isPlaying == false && loops == 0)
            gameAudio.Play();
        if (countdownPage.activeSelf)
        {
            countdownPage.SetActive(false);
            planeRigid.simulated = true;
        }
    }

    private void DisplayHeight()
    {
        height = (int)planeTransform.position.z / 10;
        height = height * 10;
        if (height > 0)
        {
            height = 0;
        }
        height = height * 32;
        heightText.text = "Altitude: " + (-height).ToString() + " ft";
    }

    public void MuteMusic()
    {
        if (gameAudio != null && gameAudio.volume == 1)
        {
            gameAudio.volume = 0;
            muteText.text = "Unmute";
            PlayerPrefs.SetInt("music", 0);
        }
        else if (gameAudio != null)
        {
            gameAudio.volume = 1;
            muteText.text = "Mute";
            PlayerPrefs.SetInt("music", 1);
        }
        PlayerPrefs.Save();
    }

    public void EndMusic()
    {
        loops = 1;
        StartCoroutine("MusicTimer");
    }

    // Timer to stop the music looping after a landing
    private IEnumerator MusicTimer()
    {
        for (;;)
        {
            if (timeToGo > 0)
            {
                countdownText.text = secondsToGo.ToString();
                timeToGo -= 1;
                yield return new WaitForSeconds(1);
            }
            else
            {
                gameAudio.Stop();
                yield return null;
            }
        }
    }

    // Countdown before the game starts after user presses play
    private IEnumerator CountdownTimer()
    {
        for (;;)
        {
            if (secondsToGo > 0)
            {
                countdownText.text = secondsToGo.ToString();
                secondsToGo -= 1;
                yield return new WaitForSeconds(1);
            }
            else
            {
                gameStarted = true;
                yield return null;
            }
        }
    }
}
