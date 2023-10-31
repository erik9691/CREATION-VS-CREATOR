using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplodeTest : NetworkBehaviour
{
    Collider[] colliders = new Collider[20];
    [SerializeField] float explosionForce = 100;
    [SerializeField] float explosionRadius = 100;
    GameObject a;
    int layerMask = 1 << 9;

    private void OnTriggerEnter(Collider other)
    {
        a = other.gameObject;
        StartCoroutine(a.GetComponent<PlayerRagdoll>().Knockdown());

        ExplodeNonAlloc();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    StartCoroutine(a.GetComponent<PlayerRagdoll>().Knockdown());

        //    ExplodeNonAllocServerRpc();
        //}
    }

    void ExplodeNonAlloc()
    {
        //Vector3 explosionPos = transform.position;
        //Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius, 9);
        //Debug.Log(colliders.Length);
        //foreach (Collider hit in colliders)
        //{
        //    Rigidbody rb = hit.GetComponent<Rigidbody>();

        //    if (rb != null)
        //        Debug.Log("Explode");
        //        rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 3.0F);
        //}

        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders, layerMask);
        if (numColliders > 0)
        {
            Debug.Log("Try explosion with " + numColliders + " colliders");
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out Rigidbody rb))
                {
                    Debug.Log("Explode" + i);
                    rb.AddExplosionForce(explosionForce * 50, transform.position, explosionRadius, 3);
                }
            }
        }
    }
}
