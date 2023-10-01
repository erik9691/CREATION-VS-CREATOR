using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class TurretController : NetworkBehaviour
{
    [SerializeField] float rotateSpeed = 0.5f;

    public NetworkVariable<bool> n_isMounted = new NetworkVariable<bool>();
    public bool isMounted;

    [SerializeField] GameObject cannon;
    PlayerInput playerInput;
    CinemachineVirtualCamera cam;
    Vector2 moveInput;


    private void Start()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void FixedUpdate()
    {
        if (isMounted)
        {
            cam.Priority = 30;

            moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

            if (moveInput.y != 0)
            {
                UpdateRotationServerRpc(moveInput.y);
            }
        }
        else
        {
            cam.Priority = 1;
        }
    }

    private void Update()
    {
        if (isMounted)
        {
            if (playerInput.actions["Shoot"].WasPressedThisFrame())
            {
                GetComponent<TurretShoot>().SpawnTurretBulletServerRpc();
            }
        }
    }

    public void Mount(bool mount, PlayerInput pi = null)
    {
        UpdateMountServerRpc(mount, GetComponent<NetworkObject>());
        isMounted = mount;
        playerInput = pi;
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdateRotationServerRpc(float direction)
    {
        cannon.transform.Rotate(-rotateSpeed*direction, 0, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMountServerRpc(bool mounted, NetworkObjectReference turretReference)
    {
        n_isMounted.Value = mounted;
        if (turretReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.GetChild(0).transform.GetChild(1).GetChild(1).gameObject.SetActive(mounted);
            UpdateMountClientRpc(mounted, turretReference);
        }
    }

    [ClientRpc]
    public void UpdateMountClientRpc(bool mounted, NetworkObjectReference turretReference)
    {
        if (turretReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.GetChild(0).transform.GetChild(1).GetChild(1).gameObject.SetActive(mounted);
        }
    }
}
