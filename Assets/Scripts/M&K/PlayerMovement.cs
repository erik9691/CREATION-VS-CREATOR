using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    float _jumpForce = 250f, _speed = 7f, _runMult = 2f, _rotationSpeed = 1f;

    Rigidbody rb;
    PlayerInput playerInput;
    Vector2 moveInput;
    Transform cameraTransform;
    Transform modelTransform;
    CinemachineVirtualCamera vc;
    bool puedoSaltar;

    public override void OnNetworkSpawn()
    {
        //Solo el dueño puede usar la camara
        if (IsOwner)
        {
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        modelTransform = transform.GetChild(0);
        cameraTransform = transform.parent.gameObject.transform.GetChild(1);
        vc = transform.parent.gameObject.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();

        //lockear el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }




    private void Update()
    {
        if (!IsOwner) return;

        //Cuando el Overlord te suelta volves a estar parado
        if (transform.rotation != Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        //Movimiento
        Vector3 move = new Vector3(moveInput.x, 0 , moveInput.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0;
        transform.Translate(move * Time.deltaTime * _speed);

        //Rotacion
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            puedoSaltar = true;
        }
    }

    public void Jump(InputAction.CallbackContext obj)
    {
        if (puedoSaltar && obj.performed)
        {
            rb.AddForce(Vector3.up * _jumpForce);
            puedoSaltar = false;
        }
    }

    public void Sprint(InputAction.CallbackContext obj)
    {
        if (puedoSaltar && obj.started)
        {
            _speed *= _runMult;
        }
        else if (obj.canceled)
        {
            _speed /= _runMult;
        }
    }
}
