using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MissileLauncher : NetworkBehaviour
{
    [SerializeField]
    float _speed = 1f, rotationSpeed = 0.3f, changeSpeed = 0.2f;
    [SerializeField] GameObject missilePrefab;

    [SerializeField] Transform target, tempTarget, playerTarget, cannon;
    GameObject missile;
    bool newMissile;

    public override void OnNetworkSpawn()
    {
        tempTarget = transform.GetChild(0);
        playerTarget = GameObject.FindGameObjectWithTag("Overlord Head").transform;
    }

    private void Update()
    {
        if (missile != null)
        {
            if (!IsInvoking("changeTarget") && newMissile)
            {
                target = tempTarget;
                Invoke("changeTarget", changeSpeed);
            }


            Vector3 lookDirection = target.position - missile.transform.position;
            lookDirection.Normalize();

            Quaternion lookdir = Quaternion.LookRotation(lookDirection);
            lookdir = Quaternion.Euler(new Vector3(lookdir.eulerAngles.x, lookdir.eulerAngles.y, 0));
            missile.transform.rotation = Quaternion.Euler(new Vector3(missile.transform.rotation.eulerAngles.x, missile.transform.rotation.eulerAngles.y, 0));
            missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, lookdir, rotationSpeed * Time.deltaTime);

            missile.transform.position += (target.position - missile.transform.position).normalized * _speed * Time.deltaTime;
        }
    }

    void changeTarget()
    {
        target = playerTarget;
        newMissile = false;
    }

    public void FindOverlord() 
    {
        playerTarget = GameObject.FindGameObjectWithTag("Overlord").transform.GetChild(0).transform.GetChild(0);
    }


    [ServerRpc (RequireOwnership = false)]
    public void SpawnMissileServerRpc()
    {
        missile = Instantiate(missilePrefab, cannon.position, cannon.rotation);
        missile.GetComponent<NetworkObject>().Spawn();

        newMissile = true;
    }
}
