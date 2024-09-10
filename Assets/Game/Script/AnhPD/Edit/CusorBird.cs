using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CusorBird : MonoBehaviour
{
    [SerializeField] private Material material;

    public bool isReady = true;

    private void OnTriggerEnter(Collider other)
    {
        material.SetColor("_EmissionColor", Color.red);
        isReady = false;
    }
    private void OnTriggerExit(Collider other)
    {
        material.SetColor("_EmissionColor", Color.green);
        isReady = true;
    }
}
