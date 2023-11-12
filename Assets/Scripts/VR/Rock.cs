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

    public void EnableKnock()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        gameObject.tag = "Knock";
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    IEnumerator DespawnRockCoroutine()
    {
        yield return new WaitForSeconds(4);
        GetComponent<NetworkObject>().Despawn();
    }
}
