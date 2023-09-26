using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Turret : NetworkBehaviour
{
    [SerializeField] float _bulSpeed = 1000f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform ShootPoint;

    private IEnumerator DeleteBulletDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(2);

        bullet.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnTurretBulletServerRpc()
    {
        GameObject bullet;

        bullet = Instantiate(bulletPrefab, ShootPoint.position, ShootPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * _bulSpeed);

        StartCoroutine(DeleteBulletDelay(bullet));
    }
}
