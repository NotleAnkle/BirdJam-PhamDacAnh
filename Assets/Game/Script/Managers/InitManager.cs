using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class InitManager : Singleton<InitManager>
{
    #region PROPERTYS
    [SerializeField]
    bool isDebug = false;
    [SerializeField]
    bool isDebugFps = false;
    [SerializeField]
    bool isDebugLog = false;
    [SerializeField]
    bool isShowAds = true;
    [SerializeField]
    GameObject debugObject;
    [SerializeField]
    InitCanvas debugCanvas;
    DebugManager debugManager;
    #endregion
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        debugCanvas._OnToggleValueChange += OnSetDebug;
        debugCanvas._OnStartGame += OnStartGame;
        debugCanvas.SetData(isDebugFps, isDebugLog, isShowAds);
    }

    private void OnSetDebug(int id, bool value)
    {
        switch (id)
        {
            case 0:
                isDebugFps = value;
                break;
            case 1:
                isDebugLog = value;
                break;
            case 2:
                isShowAds = value;
                break;
        }
    }

    private void OnStartGame()
    {
        if (isDebug)
        {
            debugManager = Instantiate(debugObject).GetComponent<DebugManager>();
            debugManager.OnInit(isDebugFps, isDebugLog, isShowAds, debugCanvas.StartLevel);
            DontDestroyOnLoad(debugManager.gameObject);
        }
        SceneManager.LoadScene("LoadStart");
    }
}
