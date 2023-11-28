using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerRagdoll : NetworkBehaviour
{
    public bool KnockDown = false;

    [SerializeField]
    float _knockTime = 3f;

    Rigidbody[] ragdollRb;
    Collider[] ragdollCol;
    Rigidbody mainRb;
    Collider mainCol;
    Animator animator;

    public bool IsGrabbed;

    // Start is called before the first frame update
    void Start()
    {
        mainCol = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

        ragdollRb = transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
        ragdollCol = transform.GetChild(0).GetComponentsInChildren<Collider>(true);

        animator = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        if (KnockDown)
        {
            StartCoroutine(Knockdown());
            KnockDown = false;
        }
    }

    public IEnumerator Knockdown()
    {
        DisableInputs();
        StartRagdollServerRpc(GetComponent<NetworkObject>(), IsGrabbed);
        yield return new WaitForSeconds(_knockTime);
        StopRagdollServerRpc(GetComponent<NetworkObject>());
        EnableInputs();
    }

    public void ExplodeImpulse(float explodeForce, Vector3 explodePos, float explodeRadius, float upMod)
    {
        StartCoroutine(Knockdown());

        GetComponent<PlayerHealth>().MinionHealth = 1;
        StartCoroutine(GetComponent<PlayerHealth>().TakeDamage());

        ExplodeImpulseServerRpc(GetComponent<NetworkObject>(), explodeForce, explodePos, explodeRadius, upMod);
    }

    [ServerRpc (RequireOwnership = false)]
    void ExplodeImpulseServerRpc(NetworkObjectReference objectReference, float explodeForceS, Vector3 explodePosS, float explodeRadiusS, float upModS)
    {
        ExplodeImpulseClientRpc(objectReference, explodeForceS, explodePosS, explodeRadiusS, upModS);
    }

    [ClientRpc]
    void ExplodeImpulseClientRpc(NetworkObjectReference objectReference, float explodeForceC, Vector3 explodePosC, float explodeRadiusC, float upModC)
    {
        Rigidbody[] ragdolRb = null;

        if (objectReference.TryGet(out NetworkObject minion))
        {
            ragdolRb = transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
        }

        foreach (Rigidbody rb in ragdolRb)
        {
            rb.AddExplosionForce(explodeForceC, explodePosC, explodeRadiusC, upModC);
        }
    }
    public void DisableInputs()
    {
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<CinemachineFreeLook>().Priority += 20;
    }
    public void EnableInputs()
    {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponentInChildren<CinemachineFreeLook>().Priority -= 20;
    }

    public void StartRagdoll()
    {
        StartRagdollServerRpc(GetComponent<NetworkObject>(), IsGrabbed);
    }

    public void StopRagdoll()
    {
        StopRagdollServerRpc(GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    void StartRagdollServerRpc(NetworkObjectReference minionReference, bool grabby)
    {
        StartRagdollClientRpc(minionReference, grabby);
    }


    [ClientRpc]
    void StartRagdollClientRpc(NetworkObjectReference minionRef, bool nIsGrabbed)
    {
        Rigidbody[] nragdollRb = null;
        Collider[] nragdollCol = null;
        Animator nanimator = null;
        Rigidbody nmainRb = null;
        Collider nmainCol = null;

        if (minionRef.TryGet(out NetworkObject networkObject))
        {
            nmainCol = GetComponent<Collider>();
            nmainRb = GetComponent<Rigidbody>();

            nragdollRb = networkObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
            nragdollCol = networkObject.transform.GetChild(0).GetComponentsInChildren<Collider>(true);

            nanimator = networkObject.GetComponentInChildren<Animator>(true);
        }

        nanimator.enabled = false;

        foreach (Collider col in nragdollCol)
        {
            col.isTrigger = false;
        }
        foreach (Rigidbody rigid in nragdollRb)
        {
            rigid.isKinematic = false;
        }

        nmainCol.isTrigger = true;
        nmainRb.isKinematic = true;
        if (nIsGrabbed) nragdollCol[0].AddComponent<FixedJoint>().connectedBody = nmainRb;
    }



    [ServerRpc (RequireOwnership = false)]
    void StopRagdollServerRpc(NetworkObjectReference minionReference)
    {
        StopRagdollClientRpc(GetComponent<NetworkObject>());
    }

    [ClientRpc]
    void StopRagdollClientRpc(NetworkObjectReference minionRef)
    {
        Rigidbody[] nragdollRb = null;
        Collider[] nragdollCol = null;
        Animator nanimator = null;
        Rigidbody nmainRb = null;
        Collider nmainCol = null;
        GameObject ngameObject = null;

        if (minionRef.TryGet(out NetworkObject networkObject))
        {
            nmainCol = networkObject.GetComponent<Collider>();
            nmainRb = networkObject.GetComponent<Rigidbody>();
            ngameObject = networkObject.gameObject;

            nragdollRb = networkObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
            nragdollCol = networkObject.transform.GetChild(0).GetComponentsInChildren<Collider>(true);

            nanimator = networkObject.GetComponentInChildren<Animator>(true);
        }

        if (nragdollCol[0].GetComponent<FixedJoint>() != null) Destroy(nragdollCol[0].GetComponent<FixedJoint>());

        Vector3 getUpPosition = nragdollCol[0].transform.position;
        getUpPosition.y += 1;
        ngameObject.transform.position = getUpPosition;

        nanimator.enabled = true;

        foreach (Collider col in nragdollCol)
        {
            col.isTrigger = true;
        }
        foreach (Rigidbody rigid in nragdollRb)
        {
            rigid.isKinematic = true;
        }

        nmainCol.isTrigger = false;
        nmainRb.isKinematic = false;
    }
}
