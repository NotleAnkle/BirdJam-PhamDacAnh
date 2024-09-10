using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPopupBooster : UICanvas
{
    RESOURCES boosterType;
    [SerializeField] List<Sprite> boosterImgList;
    [SerializeField] List<string> txtDesList;
    [SerializeField] List<int> boosterPrice;

    [SerializeField] Image boosterImg;
    [SerializeField] TMP_Text txtDes;
    [SerializeField] TMP_Text txtPrice;
    [SerializeField] TMP_Text txtGold;
    [SerializeField] Button goldBtn;
    Action action;

    public void SetData(RESOURCES booster)
    {
        boosterType = booster;
        boosterImg.sprite = boosterImgList[(int)booster];
        txtPrice.text = boosterPrice[(int)booster].ToString();
        txtDes.text = txtDesList[(int)booster];
        txtGold.text = DataManager.Ins.GameData.user.gold.ToString();

        if (DataManager.Ins.GameData.user.gold < boosterPrice[(int)boosterType]) goldBtn.interactable = false;
        else goldBtn.interactable = true;

        switch (booster)
        {
            case RESOURCES.UNDO:
                action = () => LevelManager.Ins.Undo();
                break;
            case RESOURCES.SHUFFLE:
                action = () => LevelManager.Ins.Shuffle();
                break;
            case RESOURCES.MAGNET:
                action = () => LevelManager.Ins.MagnetBooster();
                break;
            case RESOURCES.ADDSLOT:
                action = () => LevelManager.Ins.AddSlot();
                break;
        }
        GameManager.Ins.ChangeState(GameState.Pause);
    }

    public void ClickWatch()
    {
        action?.Invoke();
        DataManager.Ins.Save();
        Close();
    }

    public void ClickGold()
    {
        if (DataManager.Ins.GameData.user.gold < boosterPrice[(int)boosterType]) return;
        DataManager.Ins.GameData.user.gold -= boosterPrice[(int)boosterType];
        action?.Invoke();
        DataManager.Ins.Save();
        Close();
    }

    public override void Close()
    {
        base.Close();
        GameManager.Ins.ChangeState(GameState.InGame);
    }
}
