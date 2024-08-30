using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
	Ray ray;
	RaycastHit hit;

    private void Awake()
    {
		DontDestroyOnLoad(gameObject);
    }

    void Update()
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			if (Input.GetMouseButtonDown(0))
            {
				if (hit.transform.CompareTag(CONSTANTS.TAG_BIRD))
				{
					hit.transform.GetComponent<Bird>().OnClick();
				}
			}

		}
	}
}
