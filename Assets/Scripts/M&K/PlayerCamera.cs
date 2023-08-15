using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour
{
    PlayerInput playerInput;

    public Vector2 sensitivity = new Vector2(0.1f, 0.1f);
    [SerializeField] GameObject playerCamera;

    private Vector2 input;
    public override void OnNetworkSpawn()
    {
        playerCamera.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();  
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner) return;
        MouseLook();
    }

    private void MouseLook()
    {
        input = playerInput.actions["CamaraLook"].ReadValue<Vector2>();

        if (input.x != 0)
        {
            transform.Rotate(0, input.x * sensitivity.x * Time.deltaTime, 0);
        }
        if(input.y != 0)
        {
            Vector3 rotation = playerCamera.transform.localEulerAngles;
            rotation.x = (rotation.x - input.y * sensitivity.y * Time.deltaTime + 360) % 360;

            if (rotation.x > 40 && rotation.x < 180) 
            {
                rotation.x = 40;
            }
            else
            {
                if(rotation.x < 340 && rotation.x > 180)
                {
                    rotation.x = 340;
                }
            }
            playerCamera.transform.localEulerAngles = rotation;
                
        }
    }
}
