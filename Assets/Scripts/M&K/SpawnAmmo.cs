using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnAmmo : NetworkBehaviour
{

    [SerializeField] 
    Transform[] ammoBoxPosition;

    [SerializeField] GameObject ammoBox;

    // Update is called once per frame
    void Update()
    {
        if (!IsInvoking("SpawnBoxServerRpc"))
        {
            Invoke("SpawnBoxServerRpc", Random.Range(5, 26));
        }
        
    }

    
    [ServerRpc]
    private void SpawnBoxServerRpc()
    {
        GameObject box;

        box = Instantiate(ammoBox, ammoBoxPosition[Random.Range(0, 11)].position, Quaternion.identity);
        box.GetComponent<NetworkObject>().Spawn();
    }
}
