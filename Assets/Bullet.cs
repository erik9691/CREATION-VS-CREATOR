using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Head")
        {
            DeleteBulletServerRpc();
        }
    }

    [ServerRpc]
    private void DeleteBulletServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
