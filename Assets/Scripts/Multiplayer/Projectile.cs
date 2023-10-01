using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Head")
        {
            DeleteProjectileServerRpc();
        }
    }

    [ServerRpc]
    private void DeleteProjectileServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
