using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MissileLauncher : NetworkBehaviour
{
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] Transform rocketSpawn;

    [ServerRpc (RequireOwnership = false)]
    public void SpawnRocketServerRpc()
    {
        GameObject missile = Instantiate(rocketPrefab, rocketSpawn.position, Quaternion.Euler(-90, 0, 0));
        missile.GetComponent<NetworkObject>().Spawn();
    }
}
