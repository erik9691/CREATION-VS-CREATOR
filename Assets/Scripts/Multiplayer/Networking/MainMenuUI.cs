using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject _connectingPanel;
    [SerializeField] TMP_InputField _joinCodeInput;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        _connectingPanel.SetActive(false);
    }

    public void ClickHost()
    {
        HostManager.Instance.Host();
    }

    public async void ClickClient()
    {
        Debug.Log(_joinCodeInput.text);
        await ClientManager.Instance.Client(_joinCodeInput.text);
    }

}
