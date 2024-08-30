using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CanvasPopupTutorial : UICanvas
{
    [SerializeField] Image boosterIcon;
    [SerializeField] TMP_Text actionTxt;
    [SerializeField] TMP_Text desTxt;
    [SerializeField] VideoPlayer vidPlayer;
    [SerializeField] BaseButton actionBtn;

    public void OnInit(Sprite boosterIconSpr, FEATURE_ACTION featureAct, string desStr, VideoClip clip, FEATURE feature)
    {
        actionBtn.action.RemoveAllListeners();
        boosterIcon.sprite = boosterIconSpr;
        actionTxt.text = (featureAct == FEATURE_ACTION.CLAIM) ? "Claim" : "Continue";
        desTxt.text = desStr;
        vidPlayer.clip = clip;
        switch (feature)
        {
            case FEATURE.BOOSTER_UNDO:
                actionBtn.action.AddListener(() => ClickClaim(ref DataManager.Ins.GameData.user.boosterUndo));
                break;
            case FEATURE.BOOSTER_EXTRA_SPACE:
                actionBtn.action.AddListener(() => ClickClaim(ref DataManager.Ins.GameData.user.boosterAdd));
                break;
            case FEATURE.BOOSTER_SHUFFLE:
                actionBtn.action.AddListener(() => ClickClaim(ref DataManager.Ins.GameData.user.boosterShuffle));
                break;
            case FEATURE.BOOSTER_MAGNET:
                actionBtn.action.AddListener(() => ClickClaim(ref DataManager.Ins.GameData.user.boosterMagnet));
                break;
            default:
                actionBtn.action.AddListener(ClickContinue);
                break;
        }
        GameManager.Ins.ChangeState(GameState.Pause);
    }

    public void ClickClaim(ref int boosterNum, int amount = 1)
    {
        boosterNum = 1;
        DataManager.Ins.Save();
        LevelManager.Ins.canvasGame.UpdateUIButton();
        Close();
    }

    public void ClickContinue()
    {
        GameManager.Ins.ChangeState(GameState.InGame);
        Close();
    }
}
