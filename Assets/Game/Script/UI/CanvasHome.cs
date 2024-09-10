using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHome : UICanvas
{
    public List<UILevelObject> uiLevelObjects;
    public List<UILevelObject> uiLevelCompleteObjects;
    [SerializeField] private GameObject uilevelObj;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;
    bool init = true;
    Tweener doTween;
    [SerializeField] TMP_Text txtGold;

    private void Awake()
    {
        OnInit();
    }

    private void OnEnable()
    {
        UpdateCell();
        ScrollAnim();
    }

    void OnInit()
    {
        uiLevelObjects = InstantiateObjects(15);
        uiLevelCompleteObjects = InstantiateObjects(6, false);
        UpdateCell();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    int numberCompleteObejctEnable;
    float spacing = 285f;
    int currentIndex;
    void ScrollAnim()
    {
        if (init)
        {
            int totalObjectDisplay = uiLevelObjects.Count + (numberCompleteObejctEnable);
            content.anchoredPosition = new Vector2(0, spacing * totalObjectDisplay);
            currentIndex = 0;
            init = false;
        }
        if(currentIndex != DataManager.Ins.NormalLevelIndex)
        {
            int totalSpace = uiLevelObjects.Count;
            float targetY = (totalSpace) * spacing - 185f;

            if(currentIndex != 0)
            {
                content.anchoredPosition = new Vector2 (0, (totalSpace + 1) * spacing - 185);
            }

            content.DOAnchorPosY(targetY, 1.5f).OnComplete(() =>
            {
                uiLevelObjects[uiLevelObjects.Count - 1].MarkCurrentLevelAnim();
            })
            .SetEase(Ease.InOutSine);

            currentIndex = DataManager.Ins.NormalLevelIndex;
        }
        else
        {
            uiLevelObjects[uiLevelObjects.Count - 1].MarkCurrentLevel();
        }

    }

    private List<UILevelObject> InstantiateObjects(int count, bool isActive = true)
    {
        List<UILevelObject> objects = new List<UILevelObject>();
        for (int i = 0; i < count; i++)
        {
            Transform lvlObjTf = Instantiate(uilevelObj).transform;
            lvlObjTf.SetParent(content);
            lvlObjTf.position = Vector3.zero;
            lvlObjTf.localScale = Vector3.one;
            UILevelObject uilvlObj = lvlObjTf.GetComponent<UILevelObject>();
            objects.Add(uilvlObj);
            lvlObjTf.gameObject.SetActive(isActive);
        }
        return objects;
    }

    public void UpdateCell()
    {
        if (uiLevelObjects.Count < 1) return;
        int curLvIndex = DataManager.Ins.NormalLevelIndex;
        for (int i = 0; i < uiLevelObjects.Count; i++)
        {
            bool firstObjLine = false;
            bool firstObj = i == uiLevelObjects.Count - 1;
            if (curLvIndex == 0) firstObjLine = i == uiLevelObjects.Count - 1;
            uiLevelObjects[i].SetObjData(curLvIndex, curLvIndex + uiLevelObjects.Count - i, !firstObjLine, false, firstObj, false);
        }
        if (curLvIndex < 1) return;
        for (int i = 0; i < curLvIndex; i++)
        {
            bool breakLoop = i > uiLevelCompleteObjects.Count - 2 || curLvIndex - i == 1;
            uiLevelCompleteObjects[i].SetObjData(curLvIndex - i, curLvIndex - i, !breakLoop, !breakLoop, false, true);
            uiLevelCompleteObjects[i].gameObject.SetActive(true);

            numberCompleteObejctEnable = i + 1;

            if (breakLoop) break;
        }
    }

    public void ClickPlay()
    {
        SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_GAME, () =>
        {
            UIManager.Ins.OpenUIGameplay<CanvasGameplay>();
            LevelManager.Ins.LoadLevel();
            GameManager.Ins.ChangeState(GameState.InGame);
            LevelManager.Ins.canvasGame.UpdateUI();
        });
    }

    public void ClickSetting()
    {
        UIManager.Ins.OpenUI<CanvasSetting>();
    }

    public void ClickHome()
    {

    }

    public void ClickShop()
    {

    }

    public void ClickCollection()
    {

    }
}
