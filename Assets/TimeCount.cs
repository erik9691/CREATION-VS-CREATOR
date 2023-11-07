using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TimeCount : NetworkBehaviour
{
    public NetworkVariable<int> n_Time = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        n_Time.Value = 300;
        if (!IsServer) return;
        TimerDownServerRpc();
    }

    private IEnumerator TimerDown()
    {
        while (n_Time.Value > 1)
        {
            yield return new WaitForSeconds(1);
            n_Time.Value--;
        }
    }

    [ServerRpc]
    void TimerDownServerRpc()
    {
        StartCoroutine(TimerDown());
    }
}
