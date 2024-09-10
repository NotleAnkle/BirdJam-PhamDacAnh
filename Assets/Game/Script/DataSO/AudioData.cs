using System;
using System.Collections.Generic;
using AudioEnum;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData", order = 1)]
public class AudioData : SerializedScriptableObject
{
    [Title("BGM")]
    [SerializeField] private readonly Dictionary<BgmType, Audio> _bgmAudioDict;

    [Title("SFX")]
    [SerializeField] private readonly Dictionary<SfxType, Audio> _sfxAudioDict;

    [Title("Environment")]
    [SerializeField] private readonly Dictionary<EnvironmentType, Audio> _environmentAudioDict;

    public Dictionary<BgmType, Audio> BgmAudioDict => _bgmAudioDict;

    public Dictionary<SfxType, Audio> SfxAudioDict => _sfxAudioDict;

    public Dictionary<EnvironmentType, Audio> EnvironmentAudioDict => _environmentAudioDict;
}

[Serializable]
public class Audio
{
    public AudioClip clip;
    [Range(0, 1)]
    public float multiplier = 1;
}

namespace AudioEnum
{
    public enum SfxType
    {
        None = -1,
        BirdHit_0 = 0,
        BirdHit_1 = 1,
        BirdHit_2 = 2,
        BirdHit_3 = 3,
        BirdHit_4 = 4,
    }

    public enum BgmType
    {
        None = -1,
        MainMenu = 0,
        InGame = 1,
    }

    public enum EnvironmentType
    {
        None = -1,
        Ocean = 0,
    }
}
