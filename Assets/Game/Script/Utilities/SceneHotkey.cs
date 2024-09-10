using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

public class SceneHotkey : MonoBehaviour
{
    public static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Game/Scenes/" + sceneName + ".unity");
        }
    }

    [MenuItem("Open Scene/Gameplay")]
    public static void OpenSceneGameDemo()
    {
        OpenScene("Gameplay");
    }

    [MenuItem("Open Scene/Home")]
    public static void OpenSceneHome()
    {
        OpenScene("Home");
    }

    [MenuItem("Open Scene/LoadStart")]
    public static void OpenSceneLoadStart()
    {
        OpenScene("LoadStart");
    }

    [MenuItem("Open Scene/CryptoLoader")]
    public static void OpenSceneCryptoLoader()
    {
        OpenScene("CryptoLoader");
    }

    [MenuItem("Open Scene/LevelEditor")]
    public static void OpenSceneLevelEditor()
    {
        OpenScene("LevelEditor");
    }

}

#endif

