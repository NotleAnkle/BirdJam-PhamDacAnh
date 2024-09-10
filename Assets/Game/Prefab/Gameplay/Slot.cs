using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public RectTransform slotRect;
    public Bird bird;

    public void SetBirdToSlot()
    {
        if (bird != null)
        {
            bird.rectTf.DOMove(slotRect.position, 0.2f);
        }
    }
}

[System.Serializable]
public class SlotData
{
    public Bird bird;

    public SlotData(Bird bird)
    {
        this.bird = bird;
    }
}
