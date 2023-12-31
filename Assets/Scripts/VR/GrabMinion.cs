using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabMinion : NetworkBehaviour
{
    [SerializeField]
    float releaseDistance = 1f;

    [SerializeField] bool isRight;

    ulong minionId;
    NetworkObject minionSelected;
    public Vector3 anchor = Vector3.zero;
    XRBaseInteractor handInteractor;

    private void Start()
    {
        handInteractor = GetComponent<XRBaseInteractor>();
    }

    private void Update()
    {
        if (anchor != Vector3.zero && Vector3.Distance(anchor, transform.position) > releaseDistance)
        {
            handInteractor.allowSelect = false;
        }
    }

    public void OnSelectEnter(SelectEnterEventArgs eventArgs)
    {
        //Si agarro un Minion cambia su ownership para poder moverlo
        minionSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
        if (minionSelected != null && minionSelected.gameObject.tag == "Player")
        {
            anchor = transform.position;
            RequestOwnershipServerRpc(OwnerClientId, minionSelected);
        }
    }

    public void OnSelectExit(SelectExitEventArgs eventArgs)
    {
        if (minionSelected != null && minionSelected.gameObject.tag == "Player")
        {
            if (!handInteractor.allowSelect) handInteractor.allowSelect = true;
            anchor = Vector3.zero;
            ReturnOwnershipServerRpc(OwnerClientId, minionSelected);
        }
    }

    [ServerRpc]
    public void RequestOwnershipServerRpc(ulong newOwnerId, NetworkObjectReference networkObjectReference)
    {
        //Obtiene ID de Minion y lo reemplaza con el de Overlord
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            minionId = networkObject.OwnerClientId;
            networkObject.ChangeOwnership(newOwnerId);
            StartCoroutine(networkObject.gameObject.GetComponent<PlayerHealth>().TakeDamage(true, isRight));
            networkObject.gameObject.GetComponent<PlayerRagdoll>().IsGrabbed = true;
        }
        else
        {
            Debug.LogError("UNABLE TO CHANGE OWNERSHIP");
        }
    }

    [ServerRpc]
    public void ReturnOwnershipServerRpc(ulong newOwnerId, NetworkObjectReference networkObjectReference)
    {
        //Devuelve el ID al minion
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.GetComponent<PlayerRagdoll>().IsGrabbed = false;
            if (networkObject.GetComponent<PlayerHealth>().MinionHealth > 0)
            {
                StopAllCoroutines();
            }
            networkObject.ChangeOwnership(minionId);
        }
        else
        {
            Debug.LogError("UNABLE TO RETURN OWNERSHIP");
        }
    }
}
