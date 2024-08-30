using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
