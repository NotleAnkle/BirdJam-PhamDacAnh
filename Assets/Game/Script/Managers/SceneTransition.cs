using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : Singleton<SceneTransition>
{
    float FADE_TIME = 0.2f;
    float TRANSITION_TIME = .5f;
    IEnumerator changeSceneCor;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName, UnityAction action)
    {
        changeSceneCor = ChangeSceneCor(sceneName, action);
        PoolController.Ins.gameObject.SetActive(sceneName == CONSTANTS.SCENE_GAME);
        StartCoroutine(changeSceneCor);
    }

    IEnumerator ChangeSceneCor(string sceneName, UnityAction action)
    {
        CanvasLoadingScreen canvas = UIManager.Ins.OpenUI<CanvasLoadingScreen>();
        yield return new WaitForSeconds(FADE_TIME);
        SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForSeconds(TRANSITION_TIME);
        UIManager.Ins.CloseAll();
        action?.Invoke();
    }
}
