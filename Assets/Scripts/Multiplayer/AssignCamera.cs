using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class AssignCamera : NetworkBehaviour
{
    [SerializeField] GameObject _camerasPrefab;
    public GameObject cameras;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameras = Instantiate(_camerasPrefab, Vector3.zero, Quaternion.identity);
        GetComponent<PlayerMovement>().cameraTransform = cameras.transform.GetChild(0);
        GetComponent<PlayerGun>().SpawnPoint = cameras.transform.GetChild(0).transform.GetChild(0);
        cameras.GetComponentInChildren<CinemachineVirtualCamera>().Follow = transform;
        cameras.GetComponentInChildren<CinemachineVirtualCamera>().LookAt = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().LookAt = transform;
        cameras.transform.GetChild(2).GetComponent<SwitchVCCam>().playerInput = GetComponent<PlayerMovement>().playerInput;


        SpawnCamerasServerRpc();
    }


    [ServerRpc]
    private void SpawnCamerasServerRpc()
    {
        cameras.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}
