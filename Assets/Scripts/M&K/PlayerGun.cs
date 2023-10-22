using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerGun : NetworkBehaviour
{
    [SerializeField]
    float _bulSpeed = 1000f, _shootRate = 1f, _reloadTime = 1.5f;
    [SerializeField]
    int _storedAmmo = 30, _clipAmmo = 5, _clipCapacity = 5, _maxAmmo = 30, _boxAmmo = 15;

    [SerializeField] GameObject bulletPrefab;

    public bool CanShoot = true;

    Transform SpawnPoint;
    float shootRateTime = 0;
    bool isReloading; // para la animacion
    public SpawnAmmo ammoManager;

    private void Start()
    {
        ammoManager = GameObject.Find("AmmoBoxManager").GetComponent<SpawnAmmo>();
        SpawnPoint = GetComponent<AssignCamera>().cameras.transform.GetChild(0).transform.GetChild(0);
    }

    private void Update()
    {
        if (!IsOwner) return;

        UIManager.Instance.UpdateAmmo(_clipAmmo, _storedAmmo);
    }

    public void Shoot(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;

        if (obj.started && CanShoot)
        {
            if (_storedAmmo > 0 || _clipAmmo > 0)
            {
                if (_clipAmmo == 0)
                {
                    StartCoroutine(ReloadDelay());
                }
                else
                {
                    if (Time.time > shootRateTime && obj.started)
                    {
                        SpawnBulletServerRpc(SpawnPoint.position, SpawnPoint.rotation);
                        shootRateTime = Time.time + _shootRate;

                        _clipAmmo -= 1;
                    }
                }
            }
            else if (_storedAmmo <= 0 && _clipAmmo <= 0)
            {
                Debug.Log("NO AMMO :'V");
            }
        }
    }

    public void ReloadAction(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            Debug.Log("Reload");
            StartCoroutine(ReloadDelay());
        }
    }

    IEnumerator ReloadDelay()
    {
        CanShoot = false;
        yield return new WaitForSeconds(_reloadTime);
        Reload();
        CanShoot = true;
    }
    private void Reload()
    {
        if (_storedAmmo < _clipCapacity)
        {
            _clipAmmo = _storedAmmo;
            _storedAmmo = 0;
        }
        else
        {
            _storedAmmo -= (_clipCapacity - _clipAmmo);
            _clipAmmo = _clipCapacity;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.tag == "AmmoBox")
        {
            if (_storedAmmo < _maxAmmo)
            {
                if (_storedAmmo >= (_maxAmmo - _boxAmmo))
                {
                    _storedAmmo = _maxAmmo;
                }
                else
                {
                    _storedAmmo += _boxAmmo;
                }

                DeleteBoxServerRpc(other.gameObject);
            }
            else
            {
                //mostrar en pantalla icono de max _storedAmmo
            }
        }
    }


    [ServerRpc (RequireOwnership = false)]
    private void SpawnBulletServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;

        bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * -_bulSpeed);
    }

    [ServerRpc]
    public void DeleteBoxServerRpc(NetworkObjectReference boxGameObject)
    {
        if (!boxGameObject.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("error");
        }
        Transform objectTransform = networkObject.transform;
        networkObject.Despawn();

        ammoManager.EnableSpawnLocation(objectTransform);
    }
}
