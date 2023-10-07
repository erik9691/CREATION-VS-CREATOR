using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableMinion : NetworkBehaviour
{
    [SerializeField] MonoBehaviour[] minionComponents;
    [SerializeField] CapsuleCollider minionCollider;
    [SerializeField] Rigidbody minionRb;

    public void Disable()
    {
        foreach (MonoBehaviour comp in minionComponents)
        {
            comp.enabled = false;
        }

        GetComponent<PlayerGun>().CanShoot = false;

        minionRb.isKinematic = true;
        minionCollider.enabled = false;
        DisableModelServerRpc(GetComponent<NetworkObject>());
    }

    public void Enable()
    {
        foreach (MonoBehaviour comp in minionComponents)
        {
            comp.enabled = true;
        }

        GetComponent<PlayerGun>().CanShoot = true;

        minionRb.isKinematic = false;
        minionCollider.enabled = true;
        EnableModelServerRpc(GetComponent<NetworkObject>());
    }


    [ServerRpc(RequireOwnership = false)]
    public void DisableModelServerRpc(NetworkObjectReference minionReference)
    {
        if (minionReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            DisableModelClientRpc(minionReference);
        }
    }
    [ClientRpc]
    public void DisableModelClientRpc(NetworkObjectReference minionReference)
    {
        if (minionReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void EnableModelServerRpc(NetworkObjectReference minionReference)
    {
        if (minionReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            EnableModelClientRpc(minionReference);
        }
    }
    [ClientRpc]
    public void EnableModelClientRpc(NetworkObjectReference minionReference)
    {
        if (minionReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
