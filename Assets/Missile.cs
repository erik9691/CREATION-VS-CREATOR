using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Missile : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord")
        {
            DeleteMissileServerRpc();
        }
    }

    [ServerRpc]
    private void DeleteMissileServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
