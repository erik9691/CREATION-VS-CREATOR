using System.Collections;
using System.Collections.Generic;   
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] GameObject playerCamera;

    public override void OnStartAuthority()
    {
            playerCamera.SetActive(true);
    }

    private void Update()
    {
        gameObject.transform.position = transform.position + offset;
    }
}
