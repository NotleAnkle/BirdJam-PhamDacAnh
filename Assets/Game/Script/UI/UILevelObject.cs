using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UILevelObject : MonoBehaviour
{
    public int levelId;
    public TMP_Text levelTxt;
    public Image fillImg;
    public Image lineImg;
    public Image frameImg;
    [SerializeField] Sprite inactiveLine, activeLine;
    [SerializeField] Sprite inActiveFrame, activeFrame, completeFrame;
    //[SerializeField] GameObject glowImg;

    public void SetObjData(int lvlId, int levelTxt, bool haveLine = true, bool filled = false, bool active = false, bool complete = false)
    {
        frameImg.sprite = inActiveFrame;
        lineImg.sprite = inactiveLine;
        levelId = lvlId;
        this.levelTxt.text = levelTxt.ToString();
        lineImg.gameObject.SetActive(haveLine);
        fillImg.gameObject.SetActive(filled);
        //glowImg.SetActive(active);
        //if (active)
        //{
        //    frameImg.sprite = activeFrame;
        //    lineImg.sprite = activeLine;
        //}
        if (complete)
        {
            frameImg.sprite = completeFrame;
            lineImg.sprite = activeLine;
        }

        imgCurrentLevel.fillAmount = 0;
    }

    //Mark Current Level Frame
    [SerializeField] private Image imgCurrentLevel;
    public void MarkCurrentLevelAnim()
    {
        fillImg.gameObject.SetActive(true);
        imgCurrentLevel.fillAmount = 0;
        fillImg.fillAmount = 0;

        fillImg.DOFillAmount(1, .5f).OnComplete(() =>
        {
            imgCurrentLevel.DOFillAmount(1, .75f);
        });
    }
    public void MarkCurrentLevel()
    {
        fillImg.gameObject.SetActive(true);
        imgCurrentLevel.fillAmount = 1;
        fillImg.fillAmount = 1;
    }

}
