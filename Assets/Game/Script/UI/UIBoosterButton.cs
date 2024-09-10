using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterButton : MonoBehaviour
{
    public Button boosterBtn;
    [SerializeField] GameObject lockObj;
    [SerializeField] GameObject amountObj;
    [SerializeField] TMP_Text amountTxt;
    
    public void SetBoosterUI(bool buttonCheck, bool lockCheck, bool amountCheck, int amount)
    {
        boosterBtn.interactable = buttonCheck;
        lockObj.SetActive(lockCheck);
        amountObj.SetActive(amountCheck);
        amountTxt.text = amount.ToString();
    }
}
