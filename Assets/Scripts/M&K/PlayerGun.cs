using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerGun : NetworkBehaviour
{
    [SerializeField]
    float _bulSpeed = 1000f, _shootRate = 1f;

    [SerializeField] Transform _spawnPoint;
    [SerializeField] GameObject bulletPrefab;

    float shootRateTime = 0;
    int ammo;
    int clipAmmo;
    int clipCapacity;
    int maxAmmo;
    int boxAmmo;
    bool isReloading; // para la animacion
    Transform boxTransform;

    public void Shoot(InputAction.CallbackContext obj)
    {
        if(ammo > 0)
        {
            if (clipAmmo == 0)
            {
                Reload();
            }
            else
            {
                if (Time.time > shootRateTime && obj.started)
                {
                    SpawnBulletServerRpc(_spawnPoint.position, _spawnPoint.rotation);
                    shootRateTime = Time.time + _shootRate;

                    clipAmmo -= 1;
                }
            }
            
        }
    }

    private IEnumerator DeleteBulletDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(2);

        bullet.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;

        bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * -_bulSpeed);

        StartCoroutine(DeleteBulletDelay(bullet));
    }

    private void Reload()
    {
        if(ammo < clipCapacity)
        {
            clipAmmo = ammo;
            ammo = 0;
        }
        else
        {
            ammo -= (clipCapacity - clipAmmo);
            clipAmmo = clipCapacity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "AmmoBox")
        {
            boxTransform = other.transform;

            if (ammo < maxAmmo)
            {
                if (ammo >= (maxAmmo - boxAmmo))
                {
                    ammo = maxAmmo;
                }
                else
                {
                    ammo += boxAmmo;
                }

                DeleteBoxServerRpc();
            }
            else
            {
                //mostrar en pantalla icono de max ammo
            }
        }
    }

    [ServerRpc]
    private void DeleteBoxServerRpc()
    {
        boxTransform.GetComponent<NetworkObject>().Despawn();
    }
}
