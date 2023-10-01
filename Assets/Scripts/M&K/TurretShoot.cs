using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurretShoot : NetworkBehaviour
{
    [SerializeField] float _bulSpeed = 1000f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform ShootPoint;
    bool canShoot = true;

    private IEnumerator DeleteBulletDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(2);

        bullet.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnTurretBulletServerRpc()
    {
        if (canShoot)
        {
            GameObject bullet;

            bullet = Instantiate(bulletPrefab, ShootPoint.position, ShootPoint.rotation);
            bullet.GetComponent<NetworkObject>().Spawn();

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * _bulSpeed);

            StartCoroutine(DeleteBulletDelay(bullet));
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
