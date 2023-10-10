using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHealth : NetworkBehaviour
{
    
    [SerializeField] float _playerHealth = 10;
    [SerializeField] float _dmgAmount = 1f;
    [SerializeField] float _dmgRate = 0.5f;
    float maxHealth;
    Rigidbody[] ragdollRb;
    Collider[] ragdollCol;
    Rigidbody mainRb;
    Collider mainCol;
    Animator animator;

    public override void OnNetworkSpawn()
    {
        maxHealth = _playerHealth;

        mainCol = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

        ragdollRb = transform.GetChild(0).GetComponentsInChildren<Rigidbody>(true);
        ragdollCol = transform.GetChild(0).GetComponentsInChildren<Collider>(true);

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TakeDamage());
        }
    }


    public IEnumerator TakeDamage(bool isOverlord = false, bool isRight = true)
    {
        while (_playerHealth >= 0)
        {
            yield return new WaitForSeconds(_dmgRate);
            _playerHealth -= _dmgAmount;
            if (_playerHealth <= (maxHealth - (maxHealth / 3)))
            {
                UIManager.Instance.UpdateMinionHealth(1);
                if (_playerHealth <= (maxHealth - ((maxHealth / 3) * 2)))
                {
                    UIManager.Instance.UpdateMinionHealth(2);
                }
            }
        }

        DisableInputs();
        StartRagdollServerRpc();

        if (isOverlord)
        {
            Transform hand = GameObject.FindGameObjectWithTag("Overlord").transform.GetChild(0);
            if (isRight)
            {
                hand.GetChild(2).GetComponent<GrabMinion>().anchor = Vector3.zero;
            }
        }

    }

    void DisableInputs()
    {
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<CinemachineFreeLook>().Priority += 20;
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