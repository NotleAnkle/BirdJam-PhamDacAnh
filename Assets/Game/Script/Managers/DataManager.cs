using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DataManager : Singleton<DataManager>
{
    private GameData _gameData;
    [SerializeField]
    private AudioData audioData;
    [SerializeField]
    private TutorialData tutorialData;
    public Action updateGoldAction;

    public GameData GameData => _gameData ?? Load();
    public int NormalLevelIndex => GameData.user.normalLevelIndex;
    public AudioData AudioData => audioData;
    public TutorialData TutorialData => tutorialData;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        ResetTutorialProgress();
        for(int i = 0; i < Load().user.featuresUnlock; i++)
        {
            if(i < tutorialData.unlockDatas.Count)
            tutorialData.unlockDatas[i].unlock = true;
        }
    }

    public void ResetTutorialProgress()
    {
        for (int i = 0; i < tutorialData.unlockDatas.Count; i++)
        {
            tutorialData.unlockDatas[i].unlock = false;
        }
    }

    private GameData Load()
    {
        _gameData = Database.LoadData();
        return _gameData;
    }
    public void Save()
    {
        Database.SaveData(_gameData);
    }
}