using System.Collections;
using System.Collections.Generic;   
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] GameObject playerCamera;

    public override void OnNetworkSpawn()
    {
        playerCamera.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }
}
