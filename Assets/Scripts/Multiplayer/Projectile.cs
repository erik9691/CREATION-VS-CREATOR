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

    [ServerRpc]
    private void DeleteProjectileServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
