using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SOLevelData", menuName = "ScriptableObjects/AnhPD/SOLevelData")]

public class SOLevelData : ScriptableObject
{
    public List<ColorBasketData> colorBaskets = new List<ColorBasketData>();
}
[System.Serializable]
public class ColorBasketData
{
    public int numberOfBasket;
    public COLOR color;
}
