using System.Collections;
using System.Collections.Generic;   
using Unity.Netcode;
using UnityEngine;

public class NetworkVRCamera : NetworkBehaviour
{
    [SerializeField] GameObject _playerCamera;

    public override void OnNetworkSpawn()
    {
        _playerCamera.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }
}
