using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnAmmo : NetworkBehaviour
{

    [SerializeField] Transform[] ammoBoxPosition;

    [SerializeField] GameObject ammoBox;


    private void Start()
    {
        for(int i = 0; i< ammoBoxPosition.Length; i++)
        {
            ammoBoxPosition[i].position += new Vector3(0, 1, 0);
        }
    }

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
        int i = Random.Range(0, ammoBoxPosition.Length);
        GameObject box;

        box = Instantiate(ammoBox, ammoBoxPosition[i].position, Quaternion.identity);
        box.GetComponent<NetworkObject>().Spawn();
    }
}
