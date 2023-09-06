using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.InputSystem;

public class AssignCamera : NetworkBehaviour
{
    [SerializeField] GameObject _camerasPrefab;
    GameObject cameras;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameras = Instantiate(_camerasPrefab, Vector3.zero, Quaternion.identity);

        GetComponent<PlayerMovement>().cameraTransform = cameras.transform.GetChild(0);
        GetComponent<PlayerGun>().SpawnPoint = cameras.transform.GetChild(0).transform.GetChild(0);
        GetComponent<PlayerAim>().virtualCamera = cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();

        cameras.GetComponentInChildren<CinemachineVirtualCamera>().Follow = transform;
        cameras.GetComponentInChildren<CinemachineVirtualCamera>().LookAt = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().LookAt = transform;
        


        SpawnCamerasServerRpc();
    }


    [ServerRpc]
    private void SpawnCamerasServerRpc()
    {
        cameras.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}
