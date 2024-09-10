using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TutorialData", menuName = "ScriptableObjects/TutorialData")]
public class TutorialData : ScriptableObject
{
    public List<FeatureUnlockData> unlockDatas;
}

[System.Serializable]
public class FeatureUnlockData
{
    public bool unlock;
    public int level;
    public Sprite featureSprite;
    public string featureName;
    public string featureDes;
    public FEATURE_ACTION featureAction;
    public VideoClip clip;
}
