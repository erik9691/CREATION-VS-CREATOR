using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OverlordHealth : NetworkBehaviour
{
    public NetworkVariable<float> overlordHealth = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        overlordHealth.Value = 100;
    }

    private void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        overlordHealth.Value -= damage;
        UpdateOverlordHealthClientRpc(overlordHealth.Value);

        if (overlordHealth.Value <= 0)
        {
            //muere el overlord
        }
    }

    [ClientRpc]
    public void UpdateOverlordHealthClientRpc(float value)
    {
        UIManager.Instance.UpdateOverlordHealth(value);
    }
}
