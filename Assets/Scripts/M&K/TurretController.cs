using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class TurretController : NetworkBehaviour
{
    public NetworkVariable<bool> n_IsMounted = new NetworkVariable<bool>();
    public NetworkVariable<bool> n_IsDestroyed = new NetworkVariable<bool>();
    public bool IsMounted;

    [SerializeField] float _rotateSpeed = 0.5f;
    [SerializeField] GameObject _cannon;

    [SerializeField] int HP = 10;
    [SerializeField] int maxHP = 10;
    PlayerInput playerInput;
    CinemachineVirtualCamera cam;
    Vector2 moveInput;


    private void Start()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void FixedUpdate()
    {
        if (IsMounted)
        {
            cam.Priority = 30;

            moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

            if (moveInput.y != 0)
            {
                if (_cannon.transform.localRotation.eulerAngles.x > 200)
                {
                    if (_cannon.transform.localRotation.eulerAngles.x > 300 || moveInput.y == -1)
                    {
                        UpdateRotationServerRpc(moveInput.y);
                    }
                }
                else if (_cannon.transform.localRotation.eulerAngles.x < 9 || moveInput.y == 1)
                {
                    UpdateRotationServerRpc(moveInput.y);
                }
            }
        }
        else
        {
            cam.Priority = 1;
        }
    }

    private void Update()
    {
        if (IsMounted)
        {
            if (playerInput.actions["Shoot"].WasPressedThisFrame())
            {
                GetComponent<TurretShoot>().SpawnTurretBulletServerRpc();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Hand")
        {
            HP--;
            //update hp visuals
            if (HP <= 0)
            {
                DestroyTurretServerRpc();
            }
        }
    }

    [ServerRpc]
    void DestroyTurretServerRpc()
    {
        n_IsDestroyed.Value = true;
        //do big explosion and replace with broken model
        StartCoroutine(RespawnTurret());
    }

    IEnumerator RespawnTurret()
    {
        yield return new WaitForSeconds(10f);
        //replace with fixed model
        n_IsDestroyed.Value = false;
        HP = maxHP;
    }

    public void Mount(bool mount, PlayerInput pi = null)
    {
        UpdateMountServerRpc(mount, GetComponent<NetworkObject>());
        IsMounted = mount;
        playerInput = pi;
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdateRotationServerRpc(float direction)
    {
        _cannon.transform.Rotate(-_rotateSpeed*direction, 0, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMountServerRpc(bool mounted, NetworkObjectReference turretReference)
    {
        n_IsMounted.Value = mounted;
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
