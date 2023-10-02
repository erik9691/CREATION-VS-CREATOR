using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button _hostBtn;
    [SerializeField] Button _clientBtn;
    bool hostStarted = false;
    [SerializeField] Transform _overlordSpawn;
    private void Awake()
    {
        _hostBtn.onClick.AddListener(() =>
        {
            Host();
            Destroy(this.gameObject);
        });
        _clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            UIManager.Instance.ActivateMinionUI();
            Destroy(this.gameObject);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Host();
            Destroy(this.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
            UIManager.Instance.ActivateMinionUI();
            Destroy(this.gameObject);
        }
    }

    private void Host()
    {
        //Spawnea con setting para Overlord y lo cambia para que los demas spawneen como Minions
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        if (hostStarted)
        {
            // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
            response.PlayerPrefabHash = 969;
            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;
            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;
        }
        else
        {
            response.PlayerPrefabHash = null;
            response.Position = _overlordSpawn.position;
            response.Rotation = _overlordSpawn.rotation;
            hostStarted = true;
        }

        

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
}
