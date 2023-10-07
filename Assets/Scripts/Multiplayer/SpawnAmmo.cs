using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnAmmo : NetworkBehaviour
{

    [SerializeField] Transform[] ammoBoxPosition;
    [SerializeField] bool[] boxesSpawned;
    [SerializeField] GameObject ammoBox;

    //habilita el spawn de un box determinado y empieza el spawn de uno nuevo
    public void EnableSpawnLocation(Transform boxTransform)
    {
        for (int i = 0; i < ammoBoxPosition.Length; i++)
        {
            Debug.Log("Pass 1");
            if (Vector3.Distance(boxTransform.position, ammoBoxPosition[i].position) < 2)
            {
                Debug.Log("Pass 2");
                boxesSpawned[i] = false;
                StartCoroutine(SpawnBoxDelay(i));

                return;
            }
        }
    }

    IEnumerator SpawnBoxDelay(int index)
    {
        yield return new WaitForSeconds(Random.Range(3, 15));
        SpawnBox(index);
    }

    private void SpawnBox(int i)
    {
        GameObject box = Instantiate(ammoBox, ammoBoxPosition[i].position, Quaternion.identity, ammoBoxPosition[i]);
        box.GetComponent<NetworkObject>().Spawn();
        box.GetComponent<NetworkObject>().TrySetParent(ammoBoxPosition[i]);
        boxesSpawned[i] = true;
    }
}
