using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerGun : NetworkBehaviour
{
    [SerializeField]
    float _bulSpeed = 1000f, _shootRate = 1f;
    [SerializeField]
    int storedAmmo = 20, clipAmmo = 5, clipCapacity = 5, maxAmmo = 25, boxAmmo = 10;

    public Transform SpawnPoint;
    [SerializeField] GameObject bulletPrefab;

    float shootRateTime = 0;
    bool isReloading; // para la animacion
    Transform boxTransform;
    SpawnAmmo ammoManager;

    private void Start()
    {
        ammoManager = GameObject.Find("AmmoBoxManager").GetComponent<SpawnAmmo>();
    }

    public void Shoot(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            if (storedAmmo > 0 || clipAmmo > 0)
            {
                if (clipAmmo == 0)
                {
                    Reload();
                }
                else
                {
                    if (Time.time > shootRateTime && obj.started)
                    {
                        SpawnBulletServerRpc(SpawnPoint.position, SpawnPoint.rotation);
                        shootRateTime = Time.time + _shootRate;

                        clipAmmo -= 1;
                    }
                }
            }
            else if (storedAmmo <= 0 && clipAmmo <= 0)
            {
                Debug.Log("NO AMMO :'V");
            }
        }
    }

    private IEnumerator DeleteBulletDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(2);

        bullet.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc (RequireOwnership = false)]
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
        if(storedAmmo < clipCapacity)
        {
            clipAmmo = storedAmmo;
            storedAmmo = 0;
        }
        else
        {
            storedAmmo -= (clipCapacity - clipAmmo);
            clipAmmo = clipCapacity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.tag == "AmmoBox")
        {
            boxTransform = other.transform;

            if (storedAmmo < maxAmmo)
            {
                if (storedAmmo >= (maxAmmo - boxAmmo))
                {
                    storedAmmo = maxAmmo;
                }
                else
                {
                    storedAmmo += boxAmmo;
                }

                DeleteBoxServerRpc(other.gameObject);
            }
            else
            {
                //mostrar en pantalla icono de max storedAmmo
            }
        }
    }

    [ServerRpc]
    public void DeleteBoxServerRpc(NetworkObjectReference boxGameObject)
    {
        if (!boxGameObject.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("error");
        }
        networkObject.Despawn();

        ammoManager.EnableSpawnLocation(networkObject.transform);
    }
}
