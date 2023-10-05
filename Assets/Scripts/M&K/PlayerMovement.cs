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

    public Transform cameraTransform;

    Rigidbody rb;
    PlayerInput playerInput;
    Vector2 moveInput;

    Transform modelTransform;

    bool puedoSaltar;

    float initialSpeed;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        modelTransform = transform.GetChild(0);
        initialSpeed = _speed;

        //lockear el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }




    private void Update()
    {
        if (!IsOwner) return;
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        anim.SetFloat("VelX", moveInput.x);
        anim.SetFloat("VelY", moveInput.y);
        //Resetear la posicion cuando te suelta el Overlord
        ResetPosition();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        //Movimiento Y rotacion
        RotateAndMove();
    }

    //Condicion para saltar
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            puedoSaltar = true;
        }
    }


    //Funcion para saltar
    public void Jump(InputAction.CallbackContext obj)
    {
        if (puedoSaltar && obj.performed)
        {
            rb.AddForce(Vector3.up * _jumpForce);
            puedoSaltar = false;
        }
    }



    //Funcion para correr
    public void Sprint(InputAction.CallbackContext obj)
    {
        if (puedoSaltar && obj.started)
        {
            _speed *= _runMult;
        }
        else if (obj.canceled)
        {
            _speed = initialSpeed;
        }
    }



    private void RotateAndMove()
    {
        //Movimiento
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0;
        transform.Translate(move * Time.deltaTime * _speed);

        //Rotacion
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void ResetPosition()
    {
        //Cuando el Overlord te suelta volves a estar parado
        if (transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            Debug.Log("Rotation reset");
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
