using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ExplodeHand : NetworkBehaviour
{
    Collider[] colliders = new Collider[50];
    [SerializeField] float explosionForce = 1000;
    [SerializeField] float explosionRadius = 15;

    [SerializeField] float chargeRate = 0.2f;
    [SerializeField] float chargeAmount = 2;
    [SerializeField] float chargeCurrent = 0;
    [SerializeField] float chargeLimit = 20;

    [SerializeField] InputActionReference _gripReference;
    [SerializeField] InputActionReference _triggerReference;
    [SerializeField] bool _isRight;
    bool fireOn = false;

    bool gripOn = false;

    bool didExplosion = false;

    int layerMask = 1 << 9;

    private void Start()
    {
        _triggerReference.action.started += StartPower;
        _triggerReference.action.canceled += StopPower;
        _gripReference.action.started += ConfirmPower;
        _gripReference.action.canceled += UnconfirmPower;
    }

    public void StartPower(InputAction.CallbackContext context)
    {
        if (!gripOn || didExplosion) return;
        StopAllCoroutines();
        StartCoroutine(PowerCharge());
    }

    public void StopPower(InputAction.CallbackContext context)
    {
        StopAllCoroutines();
        StartCoroutine(PowerDrain());
        fireOn = false;
        StartFire(false, GetComponentInParent<NetworkObject>(), _isRight);
    }
    public void ConfirmPower(InputAction.CallbackContext context)
    {
        gripOn = true;
    }

    public void UnconfirmPower(InputAction.CallbackContext context)
    {
        didExplosion = false;
        gripOn = false;
        StopAllCoroutines();
        StartCoroutine(PowerDrain());
        fireOn = false;
        StartFire(false, GetComponentInParent<NetworkObject>(), _isRight);
    }

    IEnumerator PowerCharge()
    {
        while (chargeCurrent < chargeLimit)
        {
            yield return new WaitForSeconds(chargeRate);
            chargeCurrent += chargeAmount;
        }
        fireOn = true;
        StartFire(true, GetComponentInParent<NetworkObject>(), _isRight);
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
            Debug.Log("BOOM");
            Explode();
        }
    }

    void StartFire(bool activate, NetworkObjectReference networkObjectReference, bool isRight)
    {
        StartFireClientRpc(activate, networkObjectReference, isRight);
    }

    [ClientRpc]
    void StartFireClientRpc(bool activate, NetworkObjectReference networkObjectReference, bool isRight)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            ParticleSystem fire;

            if (isRight)
            {
                fire = networkObject.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<ParticleSystem>();
            }
            else
            {
                fire = networkObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
            }


            if (activate)
            {
                fire.Play();
            }
            else
            {
                fire.Stop();
            }
        }
    }


    void ExplodeEffect(NetworkObjectReference networkObjectReference, bool isRight)
    {
        ExplodeEffectClientRpc(networkObjectReference, isRight);
    }

    [ClientRpc]
    void ExplodeEffectClientRpc(NetworkObjectReference networkObjectReference, bool isRight)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            ParticleSystem explode;

            if (isRight)
            {
                explode = networkObject.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<ParticleSystem>();
            }
            else
            {
                explode = networkObject.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<ParticleSystem>();
            }

            explode.Play();
        }
    }
    void Explode()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders, layerMask, QueryTriggerInteraction.Collide);
        if (numColliders > 0)
        {
            Debug.Log("Try explosion with " + numColliders + " colliders");
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out PlayerRagdoll pr))
                {
                    ClientRpcParams rpcParams = default;
                    rpcParams.Send.TargetClientIds = new ulong[] {pr.GetComponent<NetworkObject>().OwnerClientId};
                    SendExplosionClientRpc(pr.GetComponent<NetworkObject>(), explosionForce, transform.position, explosionRadius, rpcParams);
                }
                Debug.Log("Explode" + i);
            }
            ExplodeEffect(GetComponentInParent<NetworkObject>(), _isRight);
            StopAllCoroutines();
            StartCoroutine(PowerDrain());
            fireOn = false;
            didExplosion = true;
            StartFire(false, GetComponentInParent<NetworkObject>(), _isRight);
        }
    }
 
    [ClientRpc]
    public void SendExplosionClientRpc(NetworkObjectReference objectReference, float explodeForce, Vector3 explosionPos, float explosionRadius, ClientRpcParams rpcParams)
    {
        if (objectReference.TryGet(out NetworkObject minion))
        {
            minion.GetComponent<PlayerRagdoll>().ExplodeImpulse(explodeForce, explosionPos, explosionRadius, 3);
        }
    }
}
