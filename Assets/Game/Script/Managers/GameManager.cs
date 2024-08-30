using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    LoadStart,
    MainMenu,
    InGame,
    Pause,
    WinGame,
    LoseGame,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameState gameState;
    [SerializeField]
    private bool reduceScreenResolution;
    [SerializeField]
    CameraController camControl;

    public float ReduceRatio { get; private set; }
    public bool IsReduce { get; private set; }
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(gameObject);
        // BUGS: Indicator fall when reduce resolution
        if (reduceScreenResolution)
        {
            const int maxScreenHeight = 1280;
            float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
            if (Screen.currentResolution.height > maxScreenHeight)
            {
                IsReduce = true;
                int newScreenWidth = Mathf.RoundToInt(ratio * maxScreenHeight);
                ReduceRatio = Screen.currentResolution.width / (float)newScreenWidth;
                Screen.SetResolution(newScreenWidth, maxScreenHeight, true);
            }
        }
        if(DataManager.Ins.GameData.user.normalLevelIndex != 0)
        {
            SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_HOME, () => UIManager.Ins.OpenUI<CanvasHome>());
            ChangeState(GameState.MainMenu);
        }
        else
        {
            ChangeState(GameState.InGame);
            SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_GAME, () =>
            {
                UIManager.Ins.OpenUIGameplay<CanvasGameplay>();
                LevelManager.Ins.LoadLevel();
            });
        }
    }

    #region Game State Handling
    public void ChangeState(GameState gameStateI)
    {
        //if (gameStateI == GameState.Pause)
        //{
        //    DOTween.PauseAll();
        //}
        //if (gameState == GameState.Pause && gameStateI != GameState.Pause)
        //{
        //    DOTween.PlayAll();
        //}
        gameState = gameStateI;
    }

    public bool IsState(GameState gameStateI)
    {
        return gameState == gameStateI;
    }
    #endregion
}