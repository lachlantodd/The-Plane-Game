﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rigidBody;
    public GameObject shadow;
    public GameObject arrow;
    private const float startPositionX = 14;
    private const float startPositionY = 0;
    private const float startRotation = 0;
    private float planeRotationInput;
    private const float rotationSpeedButton = 4f;
    private const float rotationSpeedTilt = 8f;
    private const float fallSpeed = 2f;
    private const float acceleration = 0.0003f;
    private const float deceleration = 0.005f; //old value which was good: 0.0025f
    private float moveSpeed = -0.3f;
    private float scaleX, scaleY, scaleZ;
    private float shrinkScale = 1f;
    private float shadowX, shadowY, shadowZ;
    private float shadowMoveSpeed = 0.035f;
    private float shadowAlpha = 1f;
    private bool planeLanded;
    private bool pressedLeft, pressedRight;
    public GameObject GameOverPage;
    public GameObject inputButtons;
    public Text scoreText, highscoreText;
    private static int score, highscore;
    private int planeType;
    private Sprite planeSprite, shadowSprite;
    public Sprite planeWhite, planeBlack;
    public Sprite shuttle;
    public Sprite planeShadow, shuttleShadow;
    private bool onRunway;
    public ParticleSystem explosion;


    private void Start()
    {
        InitialisePlane();
        InitialiseUI();
    }

    private void FixedUpdate()
    {
        CheckPlane();
    }

    private void InitialisePlane()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        planeType = PlayerPrefs.GetInt("planeType", 0);
        if (planeType == 1)
        {
            planeSprite = planeBlack;
            shadowSprite = planeShadow;
        }
        else if (planeType == 2)
        {
            planeSprite = shuttle;
            shadowSprite = shuttleShadow;
        }
        else
        {
            planeSprite = planeWhite;
            shadowSprite = planeShadow;
        }
        GetComponent<SpriteRenderer>().sprite = planeSprite;
        shadow.GetComponent<SpriteRenderer>().sprite = shadowSprite;
    }

    private void InitialiseUI()
    {
        highscore = PlayerPrefs.GetInt("highscore", -1);
        if (highscore >= 0)
        {
            highscoreText.text = "Best: " + highscore.ToString();
        }
        inputButtons.SetActive(true);
        arrow.SetActive(false);
    }

    private void CheckPlane()
    {
        if (!rigidBody.simulated || planeLanded)
            return;
        planeRotationInput = Input.GetAxisRaw("Horizontal");
        if (transform.position.z >= 0 && planeLanded == false)
        {
            LandPlane();
        }
        else
        {
            MovePlane(planeRotationInput);
            ShrinkPlane();
            RotatePlane();
            UpdateShadow();
            if (transform.position.x <= -20 || transform.position.x >= 20 || transform.position.y <= -13 || transform.position.y >= 13)
            {
                arrow.SetActive(true);
                arrow.transform.position = new Vector2(transform.position.x, transform.position.y);
                if (arrow.transform.position.x < -15)
                {
                    arrow.transform.position = new Vector2(-15, arrow.transform.position.y);
                }
                else if (arrow.transform.position.x > 15)
                {
                    arrow.transform.position = new Vector2(15, arrow.transform.position.y);
                }
                if (arrow.transform.position.y < -6)
                {
                    arrow.transform.position = new Vector2(arrow.transform.position.x, -6);
                }
                else if (arrow.transform.position.y > 6)
                {
                    arrow.transform.position = new Vector2(arrow.transform.position.x, 6);
                }
            }
            else
            {
                arrow.SetActive(false);
            }
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
            shadow.transform.Rotate(0, 0, 1 * rotationSpeedButton);
            arrow.transform.Rotate(0, 0, 1 * rotationSpeedButton);
        }
        else if (pressedRight)
        {
            transform.Rotate(0, 0, -1 * rotationSpeedButton);
            shadow.transform.Rotate(0, 0, -1 * rotationSpeedButton);
            arrow.transform.Rotate(0, 0, -1 * rotationSpeedButton);
        }
        else
        {
            transform.Rotate(0, 0, -Input.acceleration.x * rotationSpeedTilt);
            shadow.transform.Rotate(0, 0, -Input.acceleration.x * rotationSpeedTilt);
            arrow.transform.Rotate(0, 0, -Input.acceleration.x * rotationSpeedTilt);
        }
    }

    private void MovePlane(float planeRotationInput)
    {
        transform.Rotate(0, 0, -planeRotationInput * rotationSpeedButton);
        shadow.transform.Rotate(0, 0, -planeRotationInput * rotationSpeedButton);
        arrow.transform.Rotate(0, 0, -planeRotationInput * rotationSpeedButton);
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
        shadow.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }

    private void UpdateShadow()
    {
        shadowX = transform.position.x + 0.05f + transform.position.z * shadowMoveSpeed / 2;
        shadowY = transform.position.y - 0.05f + transform.position.z * shadowMoveSpeed / 2;
        shadowZ = transform.position.z;
        shadow.transform.position = new Vector3(shadowX, shadowY, shadowZ);
        shadowAlpha = 0.75f / (-transform.position.z / 40);
        shadow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, shadowAlpha);
    }

    private void CheckLandingPosition()
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
        shadow.GetComponent<SpriteRenderer>().sprite = null;
    }

    private void LandPlane()
    {
        moveSpeed += deceleration;
        CheckLandingPosition();
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
            arrow.SetActive(false);
            ShowScore();
        }
    }

    private void ShowScore()
    {
        if (transform.position.y >= -0.25 && transform.position.y <= 0.25)
        {
            if (transform.position.x <= 2.5 && transform.position.x > 2)
            {
                score = 10;
            }
            else if (transform.position.x <= 2 && transform.position.x > 1.5)
            {
                score = 9;
            }
            else if (transform.position.x <= 1.5 && transform.position.x > 1)
            {
                score = 8;
            }
            else if (transform.position.x <= 1 && transform.position.x > 0.5)
            {
                score = 7;
            }
            else if (transform.position.x <= 0.5 && transform.position.x > 0)
            {
                score = 6;
            }
             else if (transform.position.x <= 0 && transform.position.x > -0.5)
            {
                score = 6;
            }
            else if (transform.position.x <= -0.5 && transform.position.x > -1)
            {
                score = 7;
            }
            else if (transform.position.x <= -1 && transform.position.x > -1.5)
            {
                score = 8;
            }
            else if (transform.position.x <= -1.5 && transform.position.x > -2)
            {
                score = 9;
            }
            else if (transform.position.x <= -2 && transform.position.x >= -2.5)
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
        if (score > highscore || highscore < 0)
        {
            highscore = score;
            highscoreText.text = "Best: " + highscore.ToString();
            PlayerPrefs.SetInt("highscore", highscore);
            PlayerPrefs.Save();
        }
    }
}
