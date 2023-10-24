using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplodeTest : MonoBehaviour
{
    Collider[] colliders = new Collider[20];
    [SerializeField] float explosionForce = 100;
    [SerializeField] float explosionRadius = 100;
    GameObject a;

    private void OnTriggerEnter(Collider other)
    {
        a = other.gameObject;
        StartCoroutine(other.GetComponent<PlayerRagdoll>().Knockdown());

        ExplodeNonAllocServerRpc();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(a.GetComponent<PlayerRagdoll>().Knockdown());

            ExplodeNonAllocServerRpc();
        }
    }

    [ServerRpc]
    void ExplodeNonAllocServerRpc()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out Rigidbody rb))
                {
                    Debug.Log("Explode" + i);
                    rb.AddExplosionForce(explosionForce*50, transform.position, explosionRadius, 0, ForceMode.Impulse);
                }
            }
        }
    }
}
