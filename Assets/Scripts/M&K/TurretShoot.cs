using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurretShoot : NetworkBehaviour
{
    [SerializeField] float _bulSpeed = 1000f;
    [SerializeField] GameObject _missilePrefab;
    [SerializeField] Transform _shootPoint;
    bool canShoot = true;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnTurretBulletServerRpc()
    {
        if (canShoot)
        {
            GameObject bullet;

            bullet = Instantiate(_missilePrefab, _shootPoint.position, _shootPoint.rotation);
            bullet.GetComponent<NetworkObject>().Spawn();

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * _bulSpeed);

            StartCoroutine(ShootDelay());
        }
    }

    IEnumerator ShootDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(2);
        canShoot = true;
    }
}
