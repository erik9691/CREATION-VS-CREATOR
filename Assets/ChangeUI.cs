using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class ChangeUI : MonoBehaviour
{
    [SerializeField] GameObject Overlord, mainCamera, Lobby, Join, Play;
    [SerializeField] Canvas UI;

    private void Awake()
    {
        if (XRSettings.enabled == true)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
            {
                UI.renderMode = RenderMode.WorldSpace;
                UI.GetComponent<RectTransform>().localScale = new Vector3(0.014f, 0.014f, 0.014f);
                UI.GetComponent<RectTransform>().position = new Vector3(0, 5.23f, 23.85f);
            }
            else
            {
                mainCamera.SetActive(false);
                Overlord.SetActive(true);
                UI.renderMode = RenderMode.WorldSpace;
                UI.GetComponent<RectTransform>().localScale = new Vector3(0.06f, 0.06f, 0.06f);
                UI.GetComponent<RectTransform>().position = new Vector3(0, 4.5f, 0);
                Lobby.SetActive(false);
                Join.SetActive(false);
                Play.SetActive(true);
            }
        }
    }
}
