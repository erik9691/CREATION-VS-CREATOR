using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ExplodeHand : NetworkBehaviour
{
    [SerializeField] float explosionForce = 10;
    [SerializeField] float explosionRadius = 10;

    [SerializeField] float chargeRate = 0.2f;
    [SerializeField] float chargeAmount = 2;
    [SerializeField] float chargeCurrent = 0;
    [SerializeField] float chargeLimit = 20;

    [SerializeField] InputActionReference _gripReference;
    bool fireOn = false;

    Collider[] colliders = new Collider[20];

    private void Start()
    {
        _gripReference.action.started += StartPower;
        _gripReference.action.canceled += StopPower;
    }

    public void StartPower(InputAction.CallbackContext context)
    {
        Debug.Log("StartPower");
        StopAllCoroutines();
        StartCoroutine(PowerCharge());
    }

    public void StopPower(InputAction.CallbackContext context)
    {
        StopAllCoroutines();
        StartCoroutine(PowerDrain());
        fireOn = false;
        StartFireServerRpc(false, GetComponentInParent<NetworkObject>());
    }


    IEnumerator PowerCharge()
    {
        while (chargeCurrent < chargeLimit)
        {
            yield return new WaitForSeconds(chargeRate);
            chargeCurrent += chargeAmount;
        }
        fireOn = true;
        StartFireServerRpc(true, GetComponentInParent<NetworkObject>());
    }

    IEnumerator PowerDrain()
    {
        while (chargeCurrent > 0)
        {
            yield return new WaitForSeconds(chargeRate/2);
            chargeCurrent -= chargeAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fireOn)
        {
            ExplodeNonAlloc();
        }
    }

    [ServerRpc]
    void StartFireServerRpc(bool activate, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            if (activate)
            {
                networkObject.GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                networkObject.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }


    void ExplodeNonAlloc()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    chargeCurrent = 0;
                }
            }
        }
    }
}
