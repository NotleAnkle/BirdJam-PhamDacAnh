using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUnlock : UICanvas
{
    [SerializeField] TMP_Text txtItemUnlock;
    [SerializeField] Image imgItemUnlock;

    public void SetData()
    {
        TutorialData tutData = DataManager.Ins.TutorialData;
        txtItemUnlock.text = tutData.unlockDatas[DataManager.Ins.GameData.user.featuresUnlock].featureName;
        imgItemUnlock.sprite = tutData.unlockDatas[DataManager.Ins.GameData.user.featuresUnlock].featureSprite;
    }

    public void ClickContinue()
    {
        SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_HOME, () => UIManager.Ins.OpenUI<CanvasHome>());
        GameManager.Ins.ChangeState(GameState.MainMenu);
    }
}
