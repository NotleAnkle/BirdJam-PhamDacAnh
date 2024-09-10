using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    Button button;
    float timeCd = 0.25f;
    public UnityEvent action;
    private bool cooling;
    STimer cdTimer;

    private void OnEnable()
    {
        cooling = false;
    }

    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        cdTimer = TimerManager.Inst.PopSTimer();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
        TimerManager.Inst.PushSTimer(cdTimer);
    }

    public void OnClick()
    {
        if (cooling) return;
        cooling = true;
        action?.Invoke();
        cdTimer.Start(timeCd, () => cooling = false);
    }
}
