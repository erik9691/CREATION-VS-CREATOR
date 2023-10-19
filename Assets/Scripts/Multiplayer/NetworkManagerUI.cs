using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ClickHost();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ClickClient();
        }
    }

    public void ClickHost()
    {
        ServerManager.Instance.Host();
    }

    public void ClickClient()
    {
        ServerManager.Instance.Client();
    }

}
