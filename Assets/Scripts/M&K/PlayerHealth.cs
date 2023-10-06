using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHealth : NetworkBehaviour
{
    
    [SerializeField] float playerHealth = 10;
    [SerializeField] float dmgAmount = 1f;
    [SerializeField] float dmgRate = 0.5f;
    float maxHealth;
    Rigidbody[] ragdollRb;
    Collider[] ragdollCol;
    Rigidbody mainRb;
    Collider mainCol;
    [SerializeField] Animator animator;

    public IEnumerator TakeDamage()
    {
        while (playerHealth >= 0)
        {
            yield return new WaitForSeconds(dmgRate);
            playerHealth -= dmgAmount;
            if (playerHealth <= (maxHealth - (maxHealth / 3)))
            {
                UIManager.Instance.UpdateMinionHealth(1);
                if (playerHealth <= (maxHealth - ((maxHealth / 3) * 2)))
                {
                    UIManager.Instance.UpdateMinionHealth(2);
                }
            }
        }

        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<CinemachineFreeLook>().Priority += 20;
        StartRagdollServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        maxHealth = playerHealth;

        mainCol = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

        ragdollRb = GetComponentsInChildren<Rigidbody>(true);
        ragdollCol = GetComponentsInChildren<Collider>(true);

        //StopRagdollServerRpc();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TakeDamage());
        }
    }



    [ServerRpc(RequireOwnership = false)]
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
}