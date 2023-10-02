using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodeHand : MonoBehaviour
{
    [SerializeField] float explosionForce = 10;
    [SerializeField] float explosionRadius = 10;

    [SerializeField] float chargeRate = 0.2f;
    [SerializeField] float chargeAmount = 1;
    [SerializeField] float chargeCurrent = 0;
    [SerializeField] float chargeLimit = 20;

    [SerializeField] InputActionReference _gripReference;

    Collider[] colliders = new Collider[20];

    private void Start()
    {
        _gripReference.action.performed += StartPower;
        _gripReference.action.canceled += StartPower;
    }

    public void StartPower(InputAction.CallbackContext context)
    {
        StopAllCoroutines();
        StartCoroutine(PowerCharge());
    }

    public void StopPower(InputAction.CallbackContext context)
    {
        StopAllCoroutines();
        StartCoroutine(PowerDrain());
    }


    IEnumerator PowerCharge()
    {
        while (chargeCurrent < chargeLimit)
        {
            yield return new WaitForSeconds(chargeRate);
            chargeCurrent += chargeAmount;
        }
        ExplodeNonAlloc();
    }

    IEnumerator PowerDrain()
    {
        while (chargeCurrent > 0)
        {
            yield return new WaitForSeconds(chargeRate/2);
            chargeCurrent -= chargeAmount;
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
                }
            }
        }
    }
}
