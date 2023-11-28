using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OverlordHealth : NetworkBehaviour
{
    public NetworkVariable<float> overlordHealth = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        overlordHealth.Value = 150;
    }

    public void TakeDamage(float damage)
    {
        UpdateOverlordHealthServerRpc(damage);
        UpdateOverlordHealthClientRpc(overlordHealth.Value);
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

        if (value <= 0)
        {
            if (IsHost)
            {
                UIManager.Instance.GameLost();
            }
            else
            {
                UIManager.Instance.GameWon();
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
