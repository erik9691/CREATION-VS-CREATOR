using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractableHealth : NetworkBehaviour
{

    public NetworkVariable<bool> n_IsDestroyed = new NetworkVariable<bool>();

    [SerializeField] int HP = 10;
    [SerializeField] int maxHP = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Overlord Hand")
        {
            HP--;
            //update hp visuals
            if (HP <= 0)
            {
                DestroyInterServerRpc();
            }
        }
    }

    [ServerRpc]
    void DestroyInterServerRpc()
    {
        n_IsDestroyed.Value = true;
        //do big explosion and replace with broken model
        GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(RespawnTurret());
    }

    IEnumerator RespawnTurret()
    {
        yield return new WaitForSeconds(10f);
        //replace with fixed model
        n_IsDestroyed.Value = false;
        HP = maxHP;
    }
}
