using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtGold, txtHealth;
    [SerializeField] private TextMeshProUGUI txtTime;

    private GameData.UserData userData => DataManager.Ins.GameData.user;

    private bool isHeathNotFull = false;
    private float remainingTime;
    private DateTime date;

    private void OnEnable()
    {
        date = userData.lastPlayDate;
        DateTime currentDate = DateTime.UtcNow;
        TimeSpan timeSpan = currentDate - date;

        int healthRecovery = (int)(timeSpan.TotalSeconds / 1800f);
        userData.health = Mathf.Min(5, healthRecovery + userData.health);
        userData.remainingTime -= (float)timeSpan.TotalSeconds % 1800f;
        remainingTime = userData.remainingTime;

        UpdateResourceData();
    }
    public void UpdateResourceData()
    {  
        txtGold.text = userData.gold.ToString();
        txtHealth.text = userData.health.ToString();

        isHeathNotFull = userData.health < 5;
        if(isHeathNotFull)
        {
            UpdateTimer();
        }
        else
        {
            txtTime.text = "Full";
            userData.remainingTime = 1800f;
        }
    }
    private void Update()
    {
        if (!isHeathNotFull) return;
        remainingTime -= Time.deltaTime;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        txtTime.text = string.Format("{0:00} : {1:00}", minutes, seconds);

        if (remainingTime <= 0)
        {
            remainingTime = 1800f;
            userData.health += 1;
            UpdateResourceData();
        }
    }

    private void OnDisable()
    {
        userData.remainingTime = remainingTime;
        userData.lastPlayDate = DateTime.UtcNow;
        DataManager.Ins.Save();
    }
}
