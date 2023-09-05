using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MissileLauncher : NetworkBehaviour
{
    [SerializeField]
    float _speed = 1f;
    [SerializeField] GameObject missilePrefab;

    Transform target, tempTarget, playerTarget;
    GameObject missile;
    bool newMissile;

    public override void OnNetworkSpawn()
    {
        tempTarget = transform.GetChild(0);
        playerTarget = GameObject.FindGameObjectWithTag("Overlord").transform.GetChild(0).transform.GetChild(0);
    }

    private void Update()
    {
        if (missile != null)
        {
            if (!IsInvoking("changeTarget") && newMissile)
            {
                target = tempTarget;
                Invoke("changeTarget", 1f);
            }
            //missile.transform.LookAt(target);
            

            Vector3 direction = target.position - missile.transform.position;
            Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
            missile.transform.rotation = Quaternion.Lerp(missile.transform.rotation, toRotation, 0.1f * Time.time);

            missile.transform.position += (target.position - missile.transform.position).normalized * _speed * Time.deltaTime;
        }
    }

    void changeTarget()
    {
        target = playerTarget;
        newMissile = false;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnMissileServerRpc()
    {
        missile = Instantiate(missilePrefab, transform.position, missilePrefab.transform.rotation);
        //missile.transform.rotation = Quaternion.Euler(-90, 0, 0);
        missile.GetComponent<NetworkObject>().Spawn();

        newMissile = true;
    }
}
