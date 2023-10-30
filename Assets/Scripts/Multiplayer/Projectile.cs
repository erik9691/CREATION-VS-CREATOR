using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : MonoBehaviour
{
    [SerializeField] float dmgAmount = 1;

    private void Start()
    {
        if (gameObject.tag == "Bullet")
        {
            Invoke("DeleteProjectileServerRpc", 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag != "Bullet") return;

        if (other.tag == "Overlord Head")
        {
            other.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount);
            DeleteProjectileServerRpc();
        }
        else if (other.tag == "Overlord Hand")
        {
            other.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount/2);
            DeleteProjectileServerRpc();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Bullet") return;

        if (collision.transform.tag == "Overlord Head")
        {
            collision.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount);
        }
        //do cool explosion
        DeleteProjectileServerRpc();
    }

    [ServerRpc]
    private void DeleteProjectileServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
