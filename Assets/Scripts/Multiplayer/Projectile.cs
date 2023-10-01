using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : MonoBehaviour
{
    [SerializeField] float dmgAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Head")
        {
            other.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount);
            DeleteProjectileServerRpc();
        }
    }

    [ServerRpc]
    private void DeleteProjectileServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
