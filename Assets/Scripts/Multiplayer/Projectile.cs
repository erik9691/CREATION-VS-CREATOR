using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : MonoBehaviour
{
    [SerializeField] float dmgAmount = 1;

    private void Start()
    {
        if (gameObject.tag == "Bullet")
        {
            Invoke("DeleteBulletServerRpc", 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag != "Bullet") return;

        if (other.tag == "Overlord Head")
        {
            other.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount);
            DeleteBulletServerRpc();
        }
        else if (other.tag == "Overlord Hand")
        {
            other.transform.parent.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount/2);
            DeleteBulletServerRpc();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Bullet") return;

        if (collision.transform.tag == "Overlord Head")
        {
            collision.transform.parent.parent.GetComponent<OverlordHealth>().TakeDamage(dmgAmount);
        }

        DeleteProjectileServerRpc(GetComponent<NetworkObject>());
    }

    [ServerRpc]
    private void DeleteBulletServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void DeleteProjectileServerRpc(NetworkObjectReference objectReference)
    {
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            StartCoroutine(DespawnDelay(networkObject));
        }
        
        ExplodeEffectClientRpc(objectReference);
    }

    [ClientRpc]
    void ExplodeEffectClientRpc(NetworkObjectReference objectRef)
    {
        if (objectRef.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponent<MeshRenderer>().enabled = false;
            networkObject.transform.Find("TinyExplosion").GetComponent<ParticleSystem>().Play();
            AudioManager.Instance.PlaySfx("Missile Explosion", networkObject.gameObject);
        }
    }

    IEnumerator DespawnDelay(NetworkObject objecty)
    {
        yield return new WaitForSeconds(3);
        objecty.Despawn();
    }
}
