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
            col.enabled = true;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = false;
        }

        mainCol.enabled = false;
        mainRb.isKinematic = true;

        ragdollRb[0].gameObject.AddComponent<FixedJoint>().connectedBody = mainRb;

        StartRagdollClientRpc();
    }


    [ClientRpc]
    public void StartRagdollClientRpc()
    {
        animator.enabled = false;

        foreach (Collider col in ragdollCol)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = false;
        }

        mainCol.enabled = false;
        mainRb.isKinematic = true;

        ragdollRb[0].gameObject.AddComponent<FixedJoint>().connectedBody = mainRb;
    }



    [ServerRpc (RequireOwnership = false)]
    public void StopRagdollServerRpc()
    {
        Destroy(ragdollRb[0].gameObject.GetComponent<FixedJoint>());

        animator.enabled = true;

        foreach (Collider col in ragdollCol)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = true;
        }

        mainCol.enabled = true;
        mainRb.isKinematic = false;

        StopRagdollClientRpc();
    }

    [ClientRpc]
    public void StopRagdollClientRpc()
    {
        Destroy(ragdollRb[0].gameObject.GetComponent<FixedJoint>());

        animator.enabled = true;

        foreach (Collider col in ragdollCol)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigid in ragdollRb)
        {
            rigid.isKinematic = true;
        }

        mainCol.enabled = true;
        mainRb.isKinematic = false;
    }
}
