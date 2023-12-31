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
        StartCoroutine(SpawnPlayers());
        ActivateMusicClientRpc();
    }

    IEnumerator SpawnPlayers()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < HostManager.Instance.ConnectedIds.Count; i++)
        {
            if (i == 0)
            {
                GameObject overlord = Instantiate(_overlordPrefab, _overlordSpawn.position, _overlordSpawn.rotation);
                overlord.GetComponent<NetworkObject>().SpawnAsPlayerObject(HostManager.Instance.ConnectedIds[i]);
            }
            else
            {
                GameObject minion = Instantiate(_minionPrefab, _minionSpawns[i-1].position, _minionSpawns[i-1].rotation);
                minion.GetComponent<NetworkObject>().SpawnAsPlayerObject(HostManager.Instance.ConnectedIds[i]);
            }
        }
    }

    [ClientRpc]
    void ActivateMusicClientRpc()
    {
        AudioManager.Instance.PlayMusic("Gameplay");
    }
}
