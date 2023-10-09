using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodeHand : MonoBehaviour
{
    [SerializeField] float explosionForce = 10;
    [SerializeField] float explosionRadius = 10;

    [SerializeField] float chargeRate = 0.2f;
    [SerializeField] float chargeAmount = 2;
    [SerializeField] float chargeCurrent = 0;
    [SerializeField] float chargeLimit = 20;

    [SerializeField] InputActionReference _gripReference;
    public InputAction action;

    Collider[] colliders = new Collider[20];
    ParticleSystem fire;

    private void Start()
    {
        fire = GetComponentInChildren<ParticleSystem>();

        action = _gripReference.action;
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
        fire.Stop();
    }


    IEnumerator PowerCharge()
    {
        while (chargeCurrent < chargeLimit)
        {
            yield return new WaitForSeconds(chargeRate);
            chargeCurrent += chargeAmount;
        }
        fire.Play();
        //ExplodeNonAlloc();
    }

    IEnumerator PowerDrain()
    {
        while (chargeCurrent > 0)
        {
            yield return new WaitForSeconds(chargeRate/2);
            chargeCurrent -= chargeAmount;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (fire.isPlaying)
        {
            ExplodeNonAlloc();
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
