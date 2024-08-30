using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLose : UICanvas
{
    public void OnClickHome()
    {
        UIManager.Ins.CloseAll();
        SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_HOME, () => UIManager.Ins.OpenUI<CanvasHome>());
        GameManager.Ins.ChangeState(GameState.MainMenu);
    }

    public void OnClickTryAgain()
    {
        LevelManager.Ins.LoadLevel();
        Close();
    }
}
