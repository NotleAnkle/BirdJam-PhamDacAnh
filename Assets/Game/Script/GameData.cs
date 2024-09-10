using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public SettingData setting = new();
    public UserData user = new();

    [Serializable]
    public class UserData
    {
        // Level Progress Data
        public int normalLevelIndex;
        public int featuresUnlock;

        //Resources Data
        public int gold;
        public int health;
        public int boosterUndo;
        public int boosterAdd;
        public int boosterShuffle;
        public int boosterMagnet;

        public float remainingTime;
        public DateTime lastPlayDate;
    }

    [Serializable]
    public class SettingData
    {
        public bool isHapticOff;
        public bool isBgmMute;
        public bool isSfxMute;
    }
}

public static class Database
{
    private const string DATA_KEY = "GameData";

    public static void SaveData(GameData data)
    {
        string dataString = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(DATA_KEY, dataString);
        PlayerPrefs.Save();
    }

    public static GameData LoadData()
    {

        if (PlayerPrefs.HasKey(DATA_KEY))
        {
            return JsonConvert.DeserializeObject<GameData>(PlayerPrefs.GetString(DATA_KEY));
        }
        GameData gameData = new();
        SaveData(gameData);
        return gameData;
    }
}
