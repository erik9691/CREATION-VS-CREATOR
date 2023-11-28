using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractableHealth : NetworkBehaviour
{

    public NetworkVariable<bool> n_IsDestroyed = new NetworkVariable<bool>();

    [SerializeField] int HP = 10, maxHP = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Hand")
        {
            Debug.Log("Hand Hit");
            HP--;

            if (HP <= 0 && n_IsDestroyed.Value != true)
            {
                DestroyInterServerRpc(gameObject);
                StartCoroutine(RespawnInter());
            }
        }
    }

    [ServerRpc]
    void DestroyInterServerRpc(NetworkObjectReference objectReference)
    {
        n_IsDestroyed.Value = true;

        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponentInChildren<ParticleSystem>().Play();
            networkObject.transform.GetChild(0).gameObject.SetActive(false);
            networkObject.transform.GetChild(1).gameObject.SetActive(true);
        } 

        DestroyInterClientRpc(objectReference);
    }

    [ClientRpc]
    void DestroyInterClientRpc(NetworkObjectReference objectReference)
    {
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponentInChildren<ParticleSystem>().Play();
            networkObject.transform.GetChild(0).gameObject.SetActive(false);
            networkObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    IEnumerator RespawnInter()
    {
        yield return new WaitForSeconds(20f);
        
        RepairModelServerRpc(gameObject);
        HP = maxHP;
    }

    [ServerRpc]
    void RepairModelServerRpc(NetworkObjectReference objectReference)
    {
        n_IsDestroyed.Value = false;
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.GetChild(0).gameObject.SetActive(true);
            networkObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        RepairModelClientRpc(objectReference);
    }

    [ClientRpc]
    void RepairModelClientRpc(NetworkObjectReference objectReference)
    {
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.GetChild(0).gameObject.SetActive(true);
            networkObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
