using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class TurretController : NetworkBehaviour
{
    public PlayerInput playerInput;
    public bool isAwake = false;
    [SerializeField] GameObject cannon;

    private void FixedUpdate()
    {
        if (isAwake)
        {
            Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            if (moveInput.y > 0)
            {
                cannon.transform.rotation = Quaternion.Euler(new Vector3(cannon.transform.rotation.eulerAngles.x, cannon.transform.rotation.eulerAngles.y, cannon.transform.rotation.eulerAngles.z));
            }
        }
    }
}
