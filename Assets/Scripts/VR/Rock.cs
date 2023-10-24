using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Rock : NetworkBehaviour
{
    public void DespawnRock()
    {
        StartCoroutine(DespawnRockCoroutine());
    }

    IEnumerator DespawnRockCoroutine()
    {
        yield return new WaitForSeconds(4);
        GetComponent<NetworkObject>().Despawn();
    }
}
