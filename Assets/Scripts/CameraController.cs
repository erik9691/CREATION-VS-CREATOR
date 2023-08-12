using System.Collections;
using System.Collections.Generic;   
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] GameObject playerCamera;

    public override void OnNetworkSpawn()
    {
        playerCamera.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }
    private void Update()
    {
        gameObject.transform.position = transform.position + offset;
    }
}
