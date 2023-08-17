using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabMinion : NetworkBehaviour
{
    ulong minionId;
    NetworkObject minionSelected;
    public void OnSelectEnter(SelectEnterEventArgs eventArgs)
    {
        minionSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
        if (minionSelected != null)
        {
            RequestOwnershipServerRpc(OwnerClientId, minionSelected);
        }
    }

    public void OnSelectExit(SelectEnterEventArgs eventArgs)
    {
        if (minionSelected != null)
        {
            RequestOwnershipServerRpc(OwnerClientId, minionSelected);
        }
    }

    [ServerRpc]
    public void RequestOwnershipServerRpc(ulong newOwnerId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            minionId = networkObject.OwnerClientId;
            networkObject.ChangeOwnership(newOwnerId);
        }
        else
        {
            Debug.LogError("UNABLE TO CHANGE OWNERSHIP");
        }
    }

    [ServerRpc]
    public void ReturnOwnershipServerRpc(ulong newOwnerId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(minionId);
        }
        else
        {
            Debug.LogError("UNABLE TO RETURN OWNERSHIP");
        }
    }
}
