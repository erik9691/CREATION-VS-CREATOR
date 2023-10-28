using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ChangeUI : MonoBehaviour
{
    [SerializeField] GameObject Overlord, mainCamera;
    [SerializeField] Canvas UI;

    private void Awake()
    {
        if (XRSettings.enabled == true)
        {
            mainCamera.SetActive(false);
            Overlord.SetActive(true);
            UI.renderMode = RenderMode.WorldSpace;
            UI.GetComponent<RectTransform>().localScale = new Vector3(0.06f, 0.06f, 0.06f);
            UI.GetComponent<RectTransform>().position = Vector3.zero;
        }
    }
}
