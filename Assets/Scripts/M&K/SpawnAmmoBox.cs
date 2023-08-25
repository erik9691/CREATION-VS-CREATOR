using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnAmmoBox : NetworkBehaviour
{

    [SerializeField] GameObject[] ammoBoxPosition;
    int spawnTime;

    [SerializeField] GameObject ammoBox;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Invoke("SpawnBoxServerRpc", Random.Range(12, 25));
    }

    
    [ServerRpc]
    private void SpawnBoxServerRpc()
    {
        GameObject box;

        box = Instantiate(ammoBox, ammoBoxPosition[Random.Range(0, 11)].transform.position, Quaternion.identity);
        box.GetComponent<NetworkObject>().Spawn();
    }
}
