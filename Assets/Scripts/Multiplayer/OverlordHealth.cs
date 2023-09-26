using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OverlordHealth : NetworkBehaviour
{
    private NetworkVariable<int> overlordHealth = new NetworkVariable<int>();
    private const int initialValue = 100;

    public override void OnNetworkSpawn()
    {
        overlordHealth.Value = initialValue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DamageTake();
        }
    }

    private void DamageTake()
    {
        overlordHealth.Value--;
        Debug.Log(overlordHealth.Value);
    }
}
