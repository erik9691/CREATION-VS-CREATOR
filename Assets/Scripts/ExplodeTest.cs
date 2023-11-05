using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplodeTest : NetworkBehaviour
{
    Collider[] colliders = new Collider[50];
    [SerializeField] float explosionForce = 100;
    [SerializeField] float explosionRadius = 100;
    int layerMask = 1 << 9;

    bool triggerEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || triggerEntered) return;
        if (other.tag == "Player")
        {
            ExplodeNonAlloc();
            triggerEntered = true;
        }
    }

    void ExplodeNonAlloc()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders, layerMask, QueryTriggerInteraction.Collide);
        if (numColliders > 0)
        {
            Debug.Log("Try explosion with " + numColliders + " colliders");
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out PlayerRagdoll pr))
                {
                    ClientRpcParams rpcParams = default;
                    rpcParams.Send.TargetClientIds = new ulong[] {pr.GetComponent<NetworkObject>().OwnerClientId};
                    SendExplosionClientRpc(pr.GetComponent<NetworkObject>(), explosionForce, transform.position, explosionRadius, rpcParams);
                }
                Debug.Log("Explode" + i);
            }
        }
    }
 
    [ClientRpc]
    public void SendExplosionClientRpc(NetworkObjectReference objectReference, float explosionForce, Vector3 explosionPos, float explosionRadius, ClientRpcParams rpcParams)
    {
        if (objectReference.TryGet(out NetworkObject minion))
        {
            minion.GetComponent<PlayerRagdoll>().ExplodeImpulse(explosionForce, explosionPos, explosionRadius, 3);
        }
    }
}
