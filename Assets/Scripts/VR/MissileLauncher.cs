using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MissileLauncher : NetworkBehaviour
{
    [SerializeField]
    float _speed = 1f, rotationSpeed = 0.3f;
    [SerializeField] GameObject missilePrefab;

    public Transform target, tempTarget, playerTarget;
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
                Invoke("changeTarget", 1f);
            }
            //missile.transform.LookAt(target);


            Vector3 lookDirection = target.position - missile.transform.position;
            lookDirection.Normalize();

            //missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, Quaternion.LookRotation(lookDirection), rotationSpeed * Time.deltaTime);

            Quaternion lookdir = Quaternion.LookRotation(lookDirection);
            lookdir = Quaternion.Euler(new Vector3(lookdir.eulerAngles.x, lookdir.eulerAngles.y, 0));
            missile.transform.rotation = Quaternion.Euler(new Vector3(missile.transform.rotation.eulerAngles.x, missile.transform.rotation.eulerAngles.y, 0));
            missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, lookdir, rotationSpeed * Time.deltaTime);
            //missile.transform.rotation = Quaternion.Euler(new Vector3(look.eulerAngles.x, look.eulerAngles.y, 0));

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
        missile = Instantiate(missilePrefab, transform.position, Quaternion.Euler(-90,transform.rotation.y,0 ));
        missile.GetComponent<NetworkObject>().Spawn();

        newMissile = true;
    }
}
