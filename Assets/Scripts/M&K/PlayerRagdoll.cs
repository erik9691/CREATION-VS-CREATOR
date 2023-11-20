using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Knockdown());
        }
    }

    public IEnumerator Knockdown()
    {
        DisableInputs();
        StartRagdollServerRpc();
        yield return new WaitForSeconds(_knockTime);
        StopRagdollServerRpc();
        EnableInputs();
    }

    public void ExplodeImpulse(float explodeForce, Vector3 explodePos, float explodeRadius, float upMod)
    {
        Debug.Log("Explode Impulse");

        StartCoroutine(Knockdown());

        ExplodeImpulseServerRpc(GetComponent<NetworkObject>(), explodeForce, explodePos, explodeRadius, upMod);
    }

    [ServerRpc (RequireOwnership = false)]
    void ExplodeImpulseServerRpc(NetworkObjectReference objectReference, float explodeForceS, Vector3 explodePosS, float explodeRadiusS, float upModS)
    {
        Debug.Log("Explode Impulse SERVER");

        // Rigidbody[] ragdolRb = null;

        // if (objectReference.TryGet(out NetworkObject minion))
        // {
        //     ragdolRb = transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
        // }

        // foreach (Rigidbody rb in ragdolRb)
        // {
        //     rb.AddExplosionForce(eForce, ePos, eRadius, uMod);
        // }

        ExplodeImpulseClientRpc(objectReference, explodeForceS, explodePosS, explodeRadiusS, upModS);
    }
    [ClientRpc]
    void ExplodeImpulseClientRpc(NetworkObjectReference objectReference, float explodeForceC, Vector3 explodePosC, float explodeRadiusC, float upModC)
    {
        Debug.Log("Explode Impulse CLIENT");

        Rigidbody[] ragdolRb = null;

        if (objectReference.TryGet(out NetworkObject minion))
        {
            Debug.Log("get object CLIENT");
            ragdolRb = transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
        }

        foreach (Rigidbody rb in ragdolRb)
        {
            Debug.Log("add force CLIENT");
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


    [ServerRpc(RequireOwnership = false)]
    public void StartRagdollServerRpc()
    {
        animator.enabled = false;

        foreach (Collider col in ragdollCol)
        {
            col.isTrigger = false;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = false;
        }

        mainCol.isTrigger = true;
        mainRb.isKinematic = true;

        StartRagdollClientRpc();
    }


    [ClientRpc]
    public void StartRagdollClientRpc()
    {
        animator.enabled = false;

        foreach (Collider col in ragdollCol)
        {
            col.isTrigger = false;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = false;
        }

        mainCol.isTrigger = true;
        mainRb.isKinematic = true;
    }



    [ServerRpc (RequireOwnership = false)]
    public void StopRagdollServerRpc()
    {
        // Vector3 getUpPosition = ragdollCol[0].transform.position;
        // getUpPosition.y += 0.5f;
        // transform.position = getUpPosition;

        // animator.enabled = true;

        // foreach (Collider col in ragdollCol)
        // {
        //     col.isTrigger = true;
        // }
        // foreach (Rigidbody rigid in ragdollRb)
        // {
        //     rigid.isKinematic = true;
        // }

        // mainCol.isTrigger = false;
        // mainRb.isKinematic = false;

        StopRagdollClientRpc();
    }

    [ClientRpc]
    void StopRagdollClientRpc()
    {
        Vector3 getUpPosition = ragdollCol[0].transform.position;
        getUpPosition.y += 1;
        transform.position = getUpPosition;

        animator.enabled = true;

        foreach (Collider col in ragdollCol)
        {
            col.isTrigger = true;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = true;
        }

        mainCol.isTrigger = false;
        mainRb.isKinematic = false;
    }

    [ServerRpc (RequireOwnership = false)]
    public void TurnKinematicServerRpc()
    {
        mainRb.isKinematic = true;
        TurnKinematicClientRpc();
    }

    [ClientRpc]

    void TurnKinematicClientRpc()
    {
        mainRb.isKinematic = true;
    }
}
