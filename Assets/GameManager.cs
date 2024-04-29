using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;
    
    public GameObject statsMenu;
    private RectTransform statsMenuRect;
    private bool statsMenuExpanded = false;
    public float targetOffset = 1250;

    [Header("UI")]
    public GameObject loadingView;
    public GameObject gameView;
    
    public ProgressBar strengthBar;
    public ProgressBar speedBar;
    public ProgressBar stamianBar;
    public ProgressBar healthBar;

    public TextMeshProUGUI levelText;

    private void Awake()
    {
        targetOffset = 1150;
        
        //Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        strengthBar = GameObject.Find("StrengthBar").GetComponent<ProgressBar>();
        speedBar = GameObject.Find("SpeedBar").GetComponent<ProgressBar>();
        stamianBar = GameObject.Find("StaminaBar").GetComponent<ProgressBar>();
        healthBar = GameObject.Find("HealthBar").GetComponent<ProgressBar>();
        
        statsMenuRect = statsMenu.GetComponent<RectTransform>();
    }

    private void Update()
    {
        statsMenuRect.offsetMin = new Vector2(statsMenuRect.offsetMin.x, Mathf.Lerp(statsMenuRect.offsetMin.y, targetOffset, 25 * Time.deltaTime));
    }

    //UI
    public void clickStatsMenu()
    {
        Bootstrap.instance.AddGold("100");
        if (statsMenuExpanded)
        {
            statsMenuExpanded = false;
            // Change the bottom size of the RectTransform
            targetOffset = 600;
        }
        else
        {
            statsMenuExpanded = true;
            // Change the bottom size of the RectTransform
            targetOffset = 1150;

        }
    }
    
    
    public void switchToGameView()
    {
        loadingView.SetActive(false);
        gameView.SetActive(true);
    }
    
    public void switchToLoadingView()
    {
        loadingView.SetActive(true);
        gameView.SetActive(false);
    }
}
