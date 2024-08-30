using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCheat : UICanvas
{
    public GameObject cheatGroup;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ClickCheatGold()
    {
        DataManager.Ins.GameData.user.gold += 1000;
        DataManager.Ins.Save();
        DataManager.Ins.updateGoldAction?.Invoke();
    }

    public void ClickSetActive()
    {
        cheatGroup.SetActive(!cheatGroup.activeSelf);
    }
}
