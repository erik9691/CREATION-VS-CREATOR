using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnAmmo : NetworkBehaviour
{

    [SerializeField] Transform[] ammoBoxPosition;
    [SerializeField] bool[] boxesSpawned;
    [SerializeField] GameObject ammoBox;

    // Update is called once per frame
    void Update()
    {
        if (!IsInvoking("SpawnBoxServerRpc"))
        {
            Invoke("SpawnBoxServerRpc", Random.Range(5, 26));
        }
        
    }

    public void EnableSpawnLocation(Transform boxTransform)
    {
        for (int i = 0; i < ammoBoxPosition.Length; i++)
        {
            if (boxTransform.position == ammoBoxPosition[i].position)
            {
                boxesSpawned[i] = false;
                return;
            }
        }
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void SpawnBoxServerRpc()
    {
        int i = Random.Range(0, ammoBoxPosition.Length);
        if (!boxesSpawned[i])
        {
            GameObject box = Instantiate(ammoBox, ammoBoxPosition[i].position, Quaternion.identity, ammoBoxPosition[i]);
            box.GetComponent<NetworkObject>().Spawn();
            box.GetComponent<NetworkObject>().TrySetParent(ammoBoxPosition[i]);
            boxesSpawned[i] = true;
        }
    }
}