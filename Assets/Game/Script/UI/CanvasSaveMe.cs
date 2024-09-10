using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSaveMe : UICanvas
{
    [SerializeField] Button keepPlayingBtn;
    [SerializeField] UIResourceDisplay resourceDisplay;
    [SerializeField] GameObject heartbreakAnim;

    public void CheckKeepPlay(bool check)
    {
        if (!DataManager.Ins.TutorialData.unlockDatas[(int)FEATURE.BOOSTER_EXTRA_SPACE].unlock && !LevelManager.Ins.useAddSlot)
        {
            keepPlayingBtn.gameObject.SetActive(true);
        }
        else
        {
            keepPlayingBtn.gameObject.SetActive(check);
        }
    }

    public void OnClickKeepPlaying()
    {
        Close();
        if(DataManager.Ins.GameData.user.boosterAdd < 1)
        {
            LevelManager.Ins.AddSlot();
        }
        else
        {
            LevelManager.Ins.AddSlot(false);
        }
        DataManager.Ins.Save();
    }

    public void OnClickGiveUp()
    {
        if (DataManager.Ins.GameData.user.health < 1) return;

        ResourceManager.Ins.AddResource(RESOURCES.HEALTH, -1);
        
        resourceDisplay.UpdateResourceData();
        heartbreakAnim.SetActive(true);
        StartCoroutine(DelayGiveUp());
    }
    private IEnumerator DelayGiveUp()
    {
        yield return new WaitForSeconds(1);
        heartbreakAnim.SetActive(false);
        Close();
        UIManager.Ins.OpenUI<CanvasLose>();
    }
}
