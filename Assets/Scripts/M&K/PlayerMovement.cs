using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    float _jumpForce = 400f, _speed = 7f, _runMult = 2f, _rotationSpeed = 1f, maxForce = 1;

    public float HealthMult = 1;

    Transform cameraTransform;
    Rigidbody rb;
    PlayerInput playerInput;
    Vector2 moveInput;
    Transform modelTransform;
    bool canJump;
    float runSpeed, walkSpeed;
    Animator anim;
    bool inAir = true;
    bool onGround;
    public LayerMask layer;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        modelTransform = transform.GetChild(0);
        runSpeed = _speed * _runMult;
        walkSpeed = _speed;
        cameraTransform = GetComponent<AssignCamera>().cameras.transform.GetChild(0);

        UIManager.Instance.ActivateMinionUI();

        //lockear el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }




    private void Update()
    {
        if (!IsOwner) return;
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        anim.SetFloat("VelX", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("VelY", moveInput.y, 0.1f, Time.deltaTime);

        onGround = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, 1.2f, layer);

        //Resetear la posicion cuando te suelta el Overlord
        ResetPosition();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        //Movimiento Y rotacion
        // SpeedControl();
        RotateAndMove();

        if (anim.GetBool("isJumping") && rb.velocity.y < 0 && inAir || rb.velocity.y < -4)
        {
            anim.SetBool("isFalling", true);
            anim.SetBool("isGrounded", false);
            inAir = true;
            canJump = false;
        }
    }

    //Condicion para saltar
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.tag == "Floor")
        {
            if (inAir)
            {
                canJump = true;
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                anim.SetBool("isJumping", false);
                inAir = false;
                AudioManager.Instance.PlaySfx("Land", gameObject);
            }
        }
    }

    //Funcion para saltar
    public void Jump(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        if (canJump && obj.performed)
        {
            AudioManager.Instance.PlaySfx("Jump", gameObject);
            anim.SetBool("isJumping", true);
            canJump = false;
        }
    }

    public void JumpForce()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        anim.SetBool("isGrounded", false);
        inAir = true;
    }



    //Funcion para correr
    public void Sprint(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
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
        //Find target velocity
        Vector3 currentVelocity = rb.velocity;
        if (moveInput.y < 0 || moveInput.y == 0 && moveInput.x != 0)
        {
            move *= _speed * HealthMult * 0.75f;
        }
        else
        {
            move *= _speed * HealthMult;
        }

        //Align direction
        move = transform.TransformDirection(move);
        //Calculate forces
        Vector3 velocityChange = move - currentVelocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        //Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);


        //Rotacion
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void SpeedControl()
    {
        if (onGround)
        {
            rb.drag = 5;
        } 
        else
        {
            rb.drag = 0;
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > _speed * HealthMult / 5)
        {
            Vector3 limitedVel = flatVel.normalized * _speed * HealthMult / 6;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
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
