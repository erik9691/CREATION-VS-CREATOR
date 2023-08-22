using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    [SerializeField] Slider hpBar;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        walkSpeed = _speed;

        hpBar.highValue = hp;
        hpBar.lowValue = 0f;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (transform.rotation != Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        //hpBar.value = hp;
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

    public void Shoot(InputAction.CallbackContext obj)
    {
        if (Time.time > shootRateTime && obj.started)
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
