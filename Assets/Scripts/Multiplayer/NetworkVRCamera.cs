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
        //transform.rotation = new Quaternion(0, -90, 0, 0);
    }
}
