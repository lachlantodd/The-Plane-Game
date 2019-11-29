﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rigidBody;
    private float planeRotationInput;
    private const float rotationSpeedButton = 4f;
    private const float rotationSpeedTilt = 8f;
    private const float fallSpeed = 2;
    private const float acceleration = 0.0003f;
    private const float deceleration = 0.0025f;
    private float moveSpeed = -0.3f;
    private float scaleX;
    private float scaleY;
    private float scaleZ;
    private float shrinkScale = 1;
    private bool planeLanded;
    private bool pressedLeft;
    private bool pressedRight;
    public GameObject GameOverPage;
    public GameObject inputButtons;
    public Text scoreText;
    public Text highscoreText;
    private static int score;
    private static int highscore;
    private int planeType;
    private Sprite planeSprite;
    public Sprite planeWhite;
    public Sprite planeBlack;
    public Sprite shuttle;
    private bool onRunway;
    public ParticleSystem explosion;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        highscore = PlayerPrefs.GetInt("highscore", -1);
        if (highscore >= 0)
        {
            highscoreText.text = "Best: " + highscore.ToString();
        }
        planeType = PlayerPrefs.GetInt("planeType", 0);
        if (planeType == 1)
        {
            planeSprite = planeBlack;
        } else if (planeType == 2)
        {
            planeSprite = shuttle;
        }
        else
        {
            planeSprite = planeWhite;
        }
        GetComponent<SpriteRenderer>().sprite = planeSprite;
        inputButtons.SetActive(true);
        //explosion.Stop();
        //explosion.Clear();
    }

    private void FixedUpdate()
    {
        if (rigidBody.simulated)
        {
            CheckPlane();
        }
    }

    private void CheckPlane()
    {
        planeRotationInput = Input.GetAxisRaw("Horizontal");
        if (transform.position.z >= 0 & planeLanded == false)
        {
            LandPlane();
        }
        else if (transform.position.z < -1 & planeLanded == false)
        {
            MovePlane(planeRotationInput);
            ShrinkPlane();
            RotatePlane();
        }
    }

    public void ButtonPressedLeft()
    {
        pressedLeft = true;
    }

    public void ButtonReleasedLeft()
    {
        pressedLeft = false;
    }

    public void ButtonPressedRight()
    {
        pressedRight = true;
    }

    public void ButtonReleasedRight()
    {
        pressedRight = false;
    }

    private void RotatePlane()
    {
        if (pressedLeft)
        {
            transform.Rotate(0, 0, 1 * rotationSpeedButton);
        }
        else if (pressedRight)
        {
            transform.Rotate(0, 0, -1 * rotationSpeedButton);
        }
        else
        {
            transform.Rotate(0.0f, 0.0f, -Input.acceleration.x * rotationSpeedTilt);
        }
    }

    private void MovePlane(float planeRotationInput)
    {
        transform.Rotate(0, 0, -planeRotationInput * rotationSpeedButton);
        transform.Translate(moveSpeed, 0, fallSpeed);
        moveSpeed += acceleration;
        
    } 
    private void ShrinkPlane()
    {
        if (shrinkScale > 0.2)
        {
            shrinkScale = -(transform.position.z) / 800 - 0.25f;
        }
        else
        {
            shrinkScale = shrinkScale - 0.0009f;
        }
        scaleX = shrinkScale;
        scaleY = shrinkScale;
        scaleZ = transform.localScale.z;
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }

    private void CheckPosition()
    {
        if ((transform.position.y <= 0.25 && transform.position.y >= -0.25) && (transform.position.x <= 2.5 && transform.position.x >= -2.5))
        {
            onRunway = true;
        }
        else
        {
            onRunway = false;
        }
    }

    private void Explode()
    {
        explosion.Stop();
        explosion.Clear();
        explosion.Play();
        GetComponent<SpriteRenderer>().sprite = null;
    }

    private void LandPlane()
    {
        moveSpeed += deceleration;
        CheckPosition();
        if (!onRunway)
        {
            Explode();
            moveSpeed = 0;
        }
        if (moveSpeed < 0)
        {
            transform.Translate(moveSpeed, 0, 0);
        }
        else
        {
            planeLanded = true;
            GameOverPage.SetActive(true);
            inputButtons.SetActive(false);
            ShowScore();
        }
    }

    private void ShowScore()
    {
        if (transform.position.y >= -0.25 & transform.position.y <= 0.25)
        {
            if (transform.position.x <= 2.5 & transform.position.x > 2)
            {
                score = 1;
            }
            else if (transform.position.x <= 2 & transform.position.x > 1.5)
            {
                score = 2;
            }
            else if (transform.position.x <= 1.5 & transform.position.x > 1)
            {
                score = 3;
            }
            else if (transform.position.x <= 1 & transform.position.x > 0.5)
            {
                score = 4;
            }
            else if (transform.position.x <= 0.5 & transform.position.x > 0)
            {
                score = 5;
            }
             else if (transform.position.x <= 0 & transform.position.x > -0.5)
            {
                score = 6;
            }
            else if (transform.position.x <= -0.5 & transform.position.x > -1)
            {
                score = 7;
            }
            else if (transform.position.x <= -1 & transform.position.x > -1.5)
            {
                score = 8;
            }
            else if (transform.position.x <= -1.5 & transform.position.x > -2)
            {
                score = 9;
            }
            else if (transform.position.x <= -2 & transform.position.x >= -2.5)
            {
                score = 10;
            }
            else
            {
                score = 0;
            }
        }
        else
        {
            score = 0;
        }
        scoreText.text = "Score: " + score.ToString();
        if (score > highscore | highscore < 0)
        {
            highscore = score;
            highscoreText.text = "Best: " + highscore.ToString();
            PlayerPrefs.SetInt("highscore", highscore);
        }
    }
}