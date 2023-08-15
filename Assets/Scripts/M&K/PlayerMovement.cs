using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] 
    float _jumpForce = 250f, _speed = 7f, _runMult = 2f, _bulSpeed = 1000f;
    
    float walkSpeed;

    Rigidbody rb;
    PlayerInput playerInput;

    private Vector2 moveInput;

    bool puedoSaltar;

    [SerializeField]
    Transform _spawnPoint;

    [SerializeField] 
    GameObject bulletPrefab;

    [SerializeField] float shootRate = 1f;
    float shootRateTime = 0;

    [SerializeField] float hp = 1f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        walkSpeed = _speed;

        EnableActions();
    }

    private void Update()
    {
        if (!IsOwner) return;

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        transform.Translate(moveInput.x * Time.deltaTime * _speed, 0, moveInput.y * Time.deltaTime * _speed * hp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            puedoSaltar = true;
        }
    }





    private void EnableActions()
    {
        if (!IsOwner) return;

        playerInput.actions["Jump"].Enable();
        playerInput.actions["Jump"].performed += Jump;

        playerInput.actions["Sprint"].Enable();
        playerInput.actions["Sprint"].started += SprintStart;
        playerInput.actions["Sprint"].canceled += SprintCancel;

        playerInput.actions["Shoot"].Enable();
        playerInput.actions["Shoot"].started += Shoot;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (puedoSaltar)
        {
            rb.AddForce(Vector3.up * _jumpForce);
            puedoSaltar = false;
        }
    }

    private void SprintStart(InputAction.CallbackContext obj)
    {
        if (puedoSaltar)
        {
            _speed *= _runMult;
        }
    }
    private void SprintCancel(InputAction.CallbackContext obj)
    {
        _speed /= _runMult;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (Time.time > shootRateTime)
        {
            SpawnBulletServerRpc(_spawnPoint.position, _spawnPoint.rotation);
            shootRateTime = Time.time + shootRate;
        }
    }

    private IEnumerator DeleteBulletDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(2);

        bullet.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;

        bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * -_bulSpeed);

        StartCoroutine(DeleteBulletDelay(bullet));
    }

}
