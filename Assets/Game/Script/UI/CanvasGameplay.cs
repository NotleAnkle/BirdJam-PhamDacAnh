using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CanvasGameplay : UICanvas
{
    public Transform mainContent;
    public List<Slot> initSlots = new List<Slot>();
    public List<Slot> slots = new List<Slot>();
    public RectTransform finalPointRight, finalPointLeft;
    [SerializeField] Slot bonusSlot;
    [SerializeField] UIBoosterButton uiBoosterUndo;
    [SerializeField] UIBoosterButton uiBoosterAdd;
    [SerializeField] UIBoosterButton uiBoosterShuffle;
    [SerializeField] UIBoosterButton uiBoosterMagnet;
    GameData.UserData userData;

    private void Start()
    {
        userData = DataManager.Ins.GameData.user;
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UpdateUIButton();
    }

    public void UpdateUIButton()
    {
        if (!GameManager.Ins.IsState(GameState.InGame)) return;
        List<FeatureUnlockData> unlockData = DataManager.Ins.TutorialData.unlockDatas;
        GameData.UserData userData = DataManager.Ins.GameData.user;
        uiBoosterUndo.SetBoosterUI(LevelManager.Ins.CheckUndo(), !unlockData[(int)FEATURE.BOOSTER_UNDO].unlock, userData.boosterUndo > 0, userData.boosterUndo);
        uiBoosterAdd.SetBoosterUI(LevelManager.Ins.CheckAddSlot(), !unlockData[(int)FEATURE.BOOSTER_EXTRA_SPACE].unlock, userData.boosterAdd > 0, userData.boosterAdd);
        uiBoosterShuffle.SetBoosterUI(LevelManager.Ins.CheckShuffle(), !unlockData[(int)FEATURE.BOOSTER_SHUFFLE].unlock, userData.boosterShuffle > 0, userData.boosterShuffle);
        uiBoosterMagnet.SetBoosterUI(LevelManager.Ins.CheckMagnet(), !unlockData[(int)FEATURE.BOOSTER_MAGNET].unlock, userData.boosterMagnet > 0, userData.boosterMagnet);
    }

    public void OnClickReset()
    {
        DOTween.KillAll();
        SimplePool.CollectAll();
        LevelManager.Ins.InitLevel();
    }

    public void OnClickSetting()
    {
        UIManager.Ins.OpenUI<CanvasSetting>();
    }

    public void OnClickUndo()
    {
        if (userData.boosterUndo > 0)
        {
            LevelManager.Ins.Undo(false);
            DataManager.Ins.Save();
            UpdateUIButton();
        }
        else
        {
            CanvasPopupBooster cvBooster = UIManager.Ins.OpenUI<CanvasPopupBooster>();
            cvBooster.SetData(RESOURCES.UNDO);
        }
    }

    public void ClickAddSlot()
    {
        if (userData.boosterAdd > 0)
        {
            LevelManager.Ins.AddSlot(false);
            DataManager.Ins.Save();
            UpdateUIButton();
        }
        else
        {
            CanvasPopupBooster cvBooster = UIManager.Ins.OpenUI<CanvasPopupBooster>();
            cvBooster.SetData(RESOURCES.ADDSLOT);
        }
    }

    public void OnAddSlot()
    {
        uiBoosterAdd.boosterBtn.interactable = false;
        bonusSlot.gameObject.SetActive(true);
        slots.Add(bonusSlot);
        TimerManager.Inst.WaitForFrame(10, () => 
        {
            foreach (Slot slot in slots) slot.SetBirdToSlot();
        });
        
    }

    public void ResetSlots()
    {
        slots.Clear();
        for(int i = 0; i < initSlots.Count; i++)
        {
            slots.Add(initSlots[i]);
        }
        bonusSlot.bird = null;
        bonusSlot.gameObject.SetActive(false);
        uiBoosterAdd.boosterBtn.interactable = true;
    }

    public void OnClickMagnet()
    {
        if (userData.boosterMagnet > 0)
        {
            LevelManager.Ins.MagnetBooster(false);
            DataManager.Ins.Save();
            UpdateUIButton();
        }
        else
        {
            CanvasPopupBooster cvBooster = UIManager.Ins.OpenUI<CanvasPopupBooster>();
            cvBooster.SetData(RESOURCES.MAGNET);
        }

    }

    public void OnClickShuffle()
    {
        if (userData.boosterShuffle > 0)
        {
            LevelManager.Ins.Shuffle(false);
            DataManager.Ins.Save();
            UpdateUIButton();
        }
        else
        {
            CanvasPopupBooster cvBooster = UIManager.Ins.OpenUI<CanvasPopupBooster>();
            cvBooster.SetData(RESOURCES.SHUFFLE);
        }
    }
}
