using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    public Transform canvasGameplay;
    public Transform canvasUI;

    //dict for UI active
    private readonly Dictionary<Type, UICanvas> uiCanvas = new();

    //dict for quick query UI prefab
    private readonly Dictionary<Type, UICanvas> uiCanvasPrefab = new();

    //list from resource
    private UICanvas[] uiResources;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //GameManager.Ins.RegisterListenerEvent(EventID.OnUpdateUIs, UpdateUIs);
        //GameManager.Ins.RegisterListenerEvent(EventID.OnChangeGameState, OnShowIndicator);
    }
    public int ConvertPixelToUnitHeight(float pixel)
    {
        float unitHeight = ((RectTransform)canvasGameplay).rect.height;
        float pixelHeight = Screen.height;
        return (int)(pixel / pixelHeight * unitHeight);
    }
    #region Canvas

    public T OpenUI<T>() where T : UICanvas
    {
        UICanvas canvas = GetUI<T>();
        canvas.Setup();
        canvas.Open();

        return (T)canvas;
    }

    public T OpenUIGameplay<T>() where T : UICanvas
    {
        UICanvas canvas = GetUIGameplay<T>();
        canvas.Setup();
        canvas.Open();

        return (T)canvas;
    }

    public T OpenUI<T>(object param) where T : UICanvas
    {
        UICanvas canvas = GetUI<T>();
        canvas.Setup(param);
        canvas.Open(param);
        return (T)canvas;
    }

    public UICanvas OpenUIDirectly(UICanvas ui)
    {
        UICanvas canvas = Instantiate(ui, canvasGameplay);
        canvas.Setup();
        canvas.Open();
        return canvas;
    }

    public UICanvas OpenUIDirectly(UICanvas ui, object param)
    {
        UICanvas canvas = Instantiate(ui, canvasGameplay);
        canvas.Setup(param);
        canvas.Open(param);
        return canvas;
    }

    public void HideUI<T>() where T : UICanvas
    {
        if (IsOpened<T>()) GetUI<T>().Hide();
    }

    public void ShowUI<T>() where T : UICanvas
    {
        if (!IsOpened<T>()) GetUI<T>().Show();
    }

    public void CloseUI<T>() where T : UICanvas
    {
        if (IsOpened<T>()) GetUI<T>().Close();
    }

    public void CloseUIDirectly(UICanvas ui)
    {
        if (!ui.gameObject.activeInHierarchy) return;
        ui.CloseDirectly();
    }

    public bool IsOpened<T>() where T : UICanvas
    {
        return IsLoaded<T>() && uiCanvas[typeof(T)].gameObject.activeInHierarchy;
    }

    public bool IsContain(UICanvas ui)
    {
        return uiCanvas.ContainsValue(ui);
    }


    public bool IsLoaded<T>() where T : UICanvas
    {
        Type type = typeof(T);
        return uiCanvas.ContainsKey(type) && uiCanvas[type] is not null;
    }

    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            UICanvas canvas = Instantiate(GetUIPrefab<T>(), canvasUI);
            uiCanvas[typeof(T)] = canvas;
        }

        return uiCanvas[typeof(T)] as T;
    }

    public T GetUIGameplay<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            UICanvas canvas = Instantiate(GetUIPrefab<T>(), canvasGameplay);
            uiCanvas[typeof(T)] = canvas;
        }

        return uiCanvas[typeof(T)] as T;
    }


    private T GetUIPrefab<T>() where T : UICanvas
    {
        if (uiCanvasPrefab.ContainsKey(typeof(T))) return uiCanvasPrefab[typeof(T)] as T;
        uiResources ??= Resources.LoadAll<UICanvas>(CONSTANTS.UI_PATH);

        for (int i = 0; i < uiResources.Length; i++)
            if (uiResources[i] is T)
            {
                uiCanvasPrefab[typeof(T)] = uiResources[i];
                break;
            }

        return uiCanvasPrefab[typeof(T)] as T;
    }

    private void UpdateUIs()
    {
        for (int i = 0; i < backCanvas.Count; i++)
        {
            backCanvas[i].UpdateUI();
        }
    }
    #endregion

    #region Back Button

    private readonly Dictionary<UICanvas, UnityAction> backActionEvents = new();
    private readonly List<UICanvas> backCanvas = new();

    private UICanvas BackTopUI
    {
        get
        {
            UICanvas canvas = null;
            if (backCanvas.Count > 0) canvas = backCanvas[^1];

            return canvas;
        }
    }


    // private void LateUpdate()
    // {
    //     if (Input.GetKey(KeyCode.Escape) && BackTopUI != null)
    //     {
    //         BackActionEvents[BackTopUI]?.Invoke();
    //     }
    // }

    public void PushBackAction(UICanvas canvas, UnityAction action)
    {
        backActionEvents.TryAdd(canvas, action);
    }

    public void AddBackUI(UICanvas canvas)
    {
        if (!backCanvas.Contains(canvas)) backCanvas.Add(canvas);
    }

    public void RemoveBackUI(UICanvas canvas)
    {
        backCanvas.Remove(canvas);
    }

    public void CloseAll()
    {
        foreach (KeyValuePair<Type, UICanvas> item in uiCanvas.Where(item =>
                     item.Value != null && item.Value.gameObject.activeInHierarchy))
            item.Value.Close();
    }


    /// <summary>
    ///     CLear back key when comeback index UI canvas
    /// </summary>
    public void ClearBackKey()
    {
        backCanvas.Clear();
    }

    #endregion
}
