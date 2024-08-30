using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitCanvas : UICanvas
{
    public event Action<int, bool> _OnToggleValueChange;
    public event Action _OnStartGame;
    [SerializeField]
    UIToggle fpsDebugToggle;
    [SerializeField]
    UIToggle logToggle;
    [SerializeField]
    UIToggle showAdsToggle;
    [SerializeField]
    Button startButton;
    [SerializeField]
    TMP_InputField levelInputField;
    [SerializeField]
    public int StartLevel => int.Parse(levelInputField.text);
    void Start()
    {
        levelInputField.text = Database.LoadData().user.normalLevelIndex.ToString();
        fpsDebugToggle._OnValueChange += OnToggleValueChange;
        logToggle._OnValueChange += OnToggleValueChange;        
        showAdsToggle._OnValueChange += OnToggleValueChange;
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    public void SetData(bool value2, bool value3, bool value4)
    {
        fpsDebugToggle.SetValue(value2);
        logToggle.SetValue(value3);
        showAdsToggle.SetValue(value4);
    }

    private void OnToggleValueChange(int id, bool value)
    {
        _OnToggleValueChange?.Invoke(id, value);
    }

    private void OnStartButtonClick()
    {
        _OnStartGame?.Invoke();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartButtonClick);
    }
}
