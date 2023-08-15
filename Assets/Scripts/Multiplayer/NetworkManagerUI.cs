using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button _spawnBtn;
    [SerializeField] Button _hostBtn;
    [SerializeField] Button _clientBtn;

    bool hostStarted = false;
    [SerializeField] GameObject _interactablePrefab;

    private void Awake()
    {
        _spawnBtn.onClick.AddListener(() =>
        {
            SpawnInteractableServerRpc();
        });
        _hostBtn.onClick.AddListener(() =>
        {
            Host();
            //Destroy(this.gameObject);
        });
        _clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            //Destroy(this.gameObject);
        });
    }

    private void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    [ServerRpc]
    private void SpawnInteractableServerRpc()
    {
        GameObject interactable;

        interactable = Instantiate(_interactablePrefab, new Vector3(0, 2, 0), Quaternion.identity);
        interactable.GetComponent<NetworkObject>().Spawn();
        interactable.GetComponent<NetworkObject>().ChangeOwnership(1739009040);
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
            response.PlayerPrefabHash = 1094301109;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;
        }
        else
        {
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = new Vector3(0, -20, 0);

            hostStarted = true;
        }

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
}
