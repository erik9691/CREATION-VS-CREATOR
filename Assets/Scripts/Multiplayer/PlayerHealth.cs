using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    
    [SerializeField] float playerHealth = 10;
    [SerializeField] float dmgAmount = 1f;
    [SerializeField] float dmgRate = 0.5f;
    Rigidbody[] ragdollRb;
    Collider[] ragdollCol;
    Rigidbody mainRb;
    Collider mainCol;
    [SerializeField] Animator animator;

    public IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(dmgRate);
        playerHealth -= dmgAmount;
    }

    public override void OnNetworkSpawn()
    {
        mainCol = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

        ragdollRb = GetComponentsInChildren<Rigidbody>(true);
        ragdollCol = GetComponentsInChildren<Collider>(true);

        //StopRagdollServerRpc();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Ragdoll");
            StartRagdollServerRpc();
        }
    }

    [ServerRpc]
    void StartRagdollServerRpc()
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

        StartRagdollClientRpc();
    }


    [ClientRpc]
    void StartRagdollClientRpc()
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
    }

    [ServerRpc]
    void StopRagdollServerRpc()
    {
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

    [ClientRpc]
    void StopRagdollClientRpc()
    {
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



    /*
    private void Start()
    {
        rb = transform.GetComponentInChildren<Rigidbody>();

        SetEnable(false);
    }
    private void Update()
    {
        if(playerHealth <= 0)
        {
            SetEnable(true);
        }
    }

    void SetEnable(bool enable)
    {
        bool isKinematic = !enable;
        foreach(Rigidbody rigidbody in rb)
        {
            rigidbody.isKinematic = isKinematic;
        }
        animator.enable = !enable;
    }
    */
}