using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSetting : UICanvas
{
    public GameObject musicActive, musicInactive, soundActive, soundInactive, hapticActive, hapticInactive;
    public GameObject homeBtns, gameplayBtns;
    [SerializeField] private GameObject heartbreak;
    [SerializeField] private UIResourceDisplay resourceDisplay;
    GameData data;
    bool isMusicMute, isSfxMute, isHapticOff;

    private Vector2 sizeMainMenu = new Vector2(650, 700);
    private Vector2 sizePause = new Vector2(650, 800);

    private void OnEnable()
    {
        OnInit();
    }

    public void OnInit()
    {
        data = DataManager.Ins.GameData;
        isMusicMute = data.setting.isBgmMute;
        isSfxMute = data.setting.isSfxMute;
        isHapticOff = data.setting.isHapticOff;
        SetActiveState(musicActive, musicInactive, isMusicMute);
        SetActiveState(soundActive, soundInactive, isSfxMute);
        SetActiveState(hapticActive, hapticInactive, isHapticOff);
        if (GameManager.Ins.IsState(GameState.MainMenu))
        {
            gameplayBtns.SetActive(false);
            homeBtns.SetActive(true);
            resourceDisplay.gameObject.SetActive(false);
            rectTFContent.sizeDelta = sizeMainMenu;
        }
        else if (GameManager.Ins.IsState(GameState.InGame))
        {
            GameManager.Ins.ChangeState(GameState.Pause);
            gameplayBtns.SetActive(true);
            homeBtns.SetActive(false);
            resourceDisplay.gameObject.SetActive(true);

            rectTFContent.sizeDelta = sizePause;
        }
    }

    private void SetActiveState(GameObject activeObject, GameObject deactiveObject, bool isMute)
    {
        activeObject.SetActive(!isMute);
        deactiveObject.SetActive(isMute);
    }

    public void ClickMusic()
    {
        data.setting.isBgmMute = !isMusicMute;
        isMusicMute = !isMusicMute;
        SetActiveState(musicActive, musicInactive, isMusicMute);
        DataManager.Ins.Save();
    }

    public void ClickSound()
    {
        data.setting.isSfxMute = !isSfxMute;
        isSfxMute = !isSfxMute;
        SetActiveState(soundActive, soundInactive, isSfxMute);
        DataManager.Ins.Save();
    }

    public void ClickHaptic()
    {
        data.setting.isHapticOff = !isHapticOff;
        isHapticOff = !isHapticOff;
        SetActiveState(hapticActive, hapticInactive, isHapticOff);
        DataManager.Ins.Save();
    }

    public void ClickContinue()
    {
        Close();
    }

    public void ClickGiveUp()
    {
        if (DataManager.Ins.GameData.user.health < 1) return;

        ResourceManager.Ins.AddResource(RESOURCES.HEALTH, -1);
        resourceDisplay.UpdateResourceData();
        heartbreak.SetActive(true);
        StartCoroutine(DelayGiveUp());
    }
    private IEnumerator DelayGiveUp()
    {
        yield return new WaitForSeconds(1);
        heartbreak.SetActive(false);
        SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_HOME, () => UIManager.Ins.OpenUI<CanvasHome>());
        GameManager.Ins.ChangeState(GameState.MainMenu);
        Close();
    }

    public void ClickPrivacy()
    {

    }

    public void ClickContact()
    {

    }

    public override void Close()
    {
        base.Close();

        if (GameManager.Ins.IsState(GameState.Pause))
        {
            GameManager.Ins.ChangeState(GameState.InGame);
        }
    }
}
