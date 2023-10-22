using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.InputSystem;

public class AssignCamera : NetworkBehaviour
{
    [SerializeField] GameObject _camerasPrefab;
    public GameObject cameras;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameras = Instantiate(_camerasPrefab, Vector3.zero, Quaternion.identity);

        cameras.GetComponentInChildren<CinemachineVirtualCamera>().Follow = transform;
        cameras.GetComponentInChildren<CinemachineVirtualCamera>().LookAt = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = transform;
        cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().LookAt = transform;
        
        //SpawnCamerasServerRpc();
    }


    //[ServerRpc]
    //private void SpawnCamerasServerRpc()
    //{
    //    cameras.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    //}
}
