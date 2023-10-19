using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] GameObject _overlordPrefab, _minionPrefab, _camerasPrefab;

    [SerializeField] Transform _overlordSpawn;
    [SerializeField] Transform[] _minionSpawns;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        for (int i = 0; i < ServerManager.Instance.ConnectedIds.Count; i++)
        {
            if (i == 0)
            {
                GameObject overlord = Instantiate(_overlordPrefab, _overlordSpawn.position, _overlordSpawn.rotation);
                overlord.GetComponent<NetworkObject>().SpawnAsPlayerObject(ServerManager.Instance.ConnectedIds[i]);
            }
            else
            {
                GameObject minion = Instantiate(_minionPrefab, _minionSpawns[i].position, _minionSpawns[i].rotation);
                minion.GetComponent<NetworkObject>().SpawnAsPlayerObject(ServerManager.Instance.ConnectedIds[i]);
                //minion.GetComponent<NetworkObject>().ChangeOwnership(ServerManager.Instance.ConnectedIds[i]);
            }
        }
    }
}
