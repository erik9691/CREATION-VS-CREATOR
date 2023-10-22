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

    public void TakeDamage(float damage)
    {
        UpdateOverlordHealthServerRpc(damage);
        UpdateOverlordHealthClientRpc(overlordHealth.Value);

        if (overlordHealth.Value <= 0)
        {
            //muere el overlord
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateOverlordHealthServerRpc(float value)
    {
        overlordHealth.Value -= value;
    }

    [ClientRpc]
    void UpdateOverlordHealthClientRpc(float value)
    {
        UIManager.Instance.UpdateOverlordHealth(value);
    }
}
