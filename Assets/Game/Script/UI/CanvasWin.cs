using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class CanvasWin : UICanvas
{
    [SerializeField] Image featureImg;
    [SerializeField] Image fillImg;
    [SerializeField] TMP_Text txtProgress;
    [SerializeField] Transform progressBarTf;
    [SerializeField] Button continueBtn;
    Tweener doTween;
    private Vector3 scaleItem = new Vector3(0.15f, 0.15f, 1);

    public void OnEnable()
    {
        continueBtn.gameObject.SetActive(false);
        RunProgressAnim();
        UIManager.Ins.CloseUI<CanvasPopupBooster>();
        UIManager.Ins.CloseUI<CanvasSetting>();

        numberOfCoin = DataManager.Ins.GameData.user.gold;
        txtCoin.text = numberOfCoin.ToString();
    }

    public async void ClickContinue()
    {
        CollectCoins();
        await Task.Delay(2100);
        if (DataManager.Ins.NormalLevelIndex + 1 == GetCurTarget())
        {
            UIManager.Ins.OpenUI<CanvasUnlock>().SetData();
            Close();
        }
        else
        {
            SceneTransition.Ins.ChangeScene(CONSTANTS.SCENE_HOME, () => UIManager.Ins.OpenUI<CanvasHome>());
            GameManager.Ins.ChangeState(GameState.MainMenu);
        }
    }

    private void RunProgressAnim()
    {
        DataManager data = DataManager.Ins;
        int curTarget = GetCurTarget();
        float initFill = (data.NormalLevelIndex - 1) / (float)(curTarget - 1);
        float fill = (data.NormalLevelIndex) / (float)(curTarget - 1);
        fillImg.fillAmount = initFill;
        txtProgress.text = (data.NormalLevelIndex - 1) + "/" + (curTarget - 1);
        doTween = DOVirtual.Float(initFill, fill, 1f, (v) =>
        {
            fillImg.fillAmount = v;
        }).SetEase(Ease.InOutSine).OnComplete(() => 
        {
            txtProgress.text = (data.NormalLevelIndex) + "/" + (curTarget - 1);
            progressBarTf.DOPunchScale(scaleItem, 0.5f);
            continueBtn.gameObject.SetActive(true);
        });
    }

    private int GetCurTarget()
    {
        DataManager data = DataManager.Ins;
        int curTarget = 0;
        for (int i = 1; i < data.TutorialData.unlockDatas.Count; i++)
        {
            if (!data.TutorialData.unlockDatas[i].unlock)
            {
                curTarget = data.TutorialData.unlockDatas[i].level;
                break;
            }
        }
        return curTarget;
    }

    //Collect Coin anim
    [SerializeField] private RectTransform[] coinPos;
    [SerializeField] private RectTransform coinTarget, coinStart;
    [SerializeField] private TextMeshProUGUI txtCoin;

    private int numberOfCoin = 0;
    private void CollectCoins()
    {
        float delay = 0f;
        for(int i = 0; i < coinPos.Length; i++) 
        {
            coinPos[i].localScale = Vector3.zero;
            coinPos[i].anchoredPosition = RandomRectPosAround(coinStart.anchoredPosition);
            coinPos[i].eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

            coinPos[i].DOScale(1, .5f).SetDelay(delay).SetEase(Ease.OutBack);
            coinPos[i].DOMove(coinTarget.transform.position, .5f).SetDelay(delay+.5f).SetEase(Ease.InBack);
            coinPos[i].DORotate(Vector3.zero, .2f).SetDelay(delay + .1f).SetEase(Ease.Flash);

            delay += 0.1f;
        }

        DOVirtual.Float(numberOfCoin, numberOfCoin + 100, 1f, (c) =>
        {
            txtCoin.text = (Mathf.RoundToInt(c)).ToString();
        })
        .SetDelay(1f)
        .OnComplete(() =>
        {
            numberOfCoin += 100;
            ResourceManager.Ins.AddResource(RESOURCES.GOLD, 100);
        });
    }
    private Vector2 RandomRectPosAround(Vector2 rectPos)
    {
        float x = Random.Range(-150f, 150f);
        float y = Random.Range(-100f, 100f);

        return new Vector2(x, y) + rectPos;   
    }
}
