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

    Transform cameraTransform;
    Rigidbody rb;
    PlayerInput playerInput;
    Vector2 moveInput;
    Transform modelTransform;
    bool canJump;
    float runSpeed, walkSpeed;
    Animator anim;
    public bool inAir = true;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        modelTransform = transform.GetChild(0);
        runSpeed = _speed * _runMult;
        walkSpeed = _speed;
        cameraTransform = GetComponent<AssignCamera>().cameras.transform.GetChild(0);

        //lockear el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }




    private void Update()
    {
        if (!IsOwner) return;
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        anim.SetFloat("VelX", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("VelY", moveInput.y, 0.1f, Time.deltaTime);
        //Resetear la posicion cuando te suelta el Overlord
        ResetPosition();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        //Movimiento Y rotacion
        RotateAndMove();

        if ((anim.GetBool("isJumping") && rb.velocity.y < 0) && inAir || rb.velocity.y < -4)
        {
            anim.SetBool("isFalling", true);
            inAir = true;
        }
    }

    //Condicion para saltar
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            canJump = true;
            if (inAir)
            {
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                anim.SetBool("isJumping", false);
                inAir = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isGrounded", false);
            canJump = false;
        }
    }


    //Funcion para saltar
    public void Jump(InputAction.CallbackContext obj)
    {
        if (canJump && obj.performed)
        {
            anim.SetBool("isJumping", true);
            canJump = false;
        }
    }

    public void JumpForce()
    {
        rb.AddForce(Vector3.up * _jumpForce);
        inAir = true;
    }



    //Funcion para correr
    public void Sprint(InputAction.CallbackContext obj)
    {
        if (canJump && obj.started)
        {
            _speed = runSpeed;
            anim.SetBool("isRunning", true);
        }
        else if (obj.canceled)
        {
            _speed = walkSpeed;
            anim.SetBool("isRunning", false);
        }
    }



    private void RotateAndMove()
    {
        //Movimiento
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0;

        if (moveInput.y < 0 || moveInput.y == 0 && moveInput.x != 0)
        {
            transform.Translate(move * Time.deltaTime * _speed * 0.75f);
        }
        else
        {
            transform.Translate(move * Time.deltaTime * _speed);
        }

        //Rotacion
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void ResetPosition()
    {
        //Cuando el Overlord te suelta volves a estar parado
        if (transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
