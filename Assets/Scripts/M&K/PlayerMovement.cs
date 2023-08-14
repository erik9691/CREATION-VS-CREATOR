using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] 
    float jumpForce = 250f, speed = 7f, runSpeed = 10f, bulSpeed = 1000f;
    
    private float walkSpeed;

    private Rigidbody rb;
    private PlayerInput pi;

    private Vector2 input;

    public bool puedoSaltar;

    public Transform spawnBala;
    public GameObject bala;
    public float shootRate = 1f;
    private float shootRateTime = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pi = GetComponent<PlayerInput>();
        walkSpeed = speed;

        EnableActions();
    }

    private void Update()
    {
        if (!IsOwner) return;

        input = pi.actions["Move"].ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        transform.Translate(input.x * Time.deltaTime * speed, 0, input.y * Time.deltaTime * speed);
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

        pi.actions["Jump"].Enable();
        pi.actions["Jump"].performed += Jump;

        pi.actions["Sprint"].Enable();
        pi.actions["Sprint"].started += SprintStart;
        pi.actions["Sprint"].canceled += SprintCancel;

        pi.actions["Shoot"].Enable();
        pi.actions["Shoot"].started += Shoot;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (puedoSaltar)
        {
            rb.AddForce(Vector3.up * jumpForce);
            puedoSaltar = false;
        }
    }

    private void SprintStart(InputAction.CallbackContext obj)
    {
        if (puedoSaltar)
        {
            speed = runSpeed;
        }
    }
    private void SprintCancel(InputAction.CallbackContext obj)
    {
        speed = walkSpeed;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (Time.time > shootRateTime)
        {
            SpawnBulletServerRpc(spawnBala.position, spawnBala.rotation);
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
        GameObject newBullet;

        newBullet = Instantiate(bala, position, rotation);
        newBullet.GetComponent<NetworkObject>().Spawn();

        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.up * -bulSpeed);

        StartCoroutine(DeleteBulletDelay(newBullet));
    }

}
