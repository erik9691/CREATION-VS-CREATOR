using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteractions : NetworkBehaviour
{
    [SerializeField]
    float _interactMeterCapacity = 5, _interactSpeed = 0.1f, _interactAmount = 0.1f;

    float interactMeterCurrent = 0;
    GameObject interactable;
    bool isInteracting = false;
    bool isMounting = false;

    public void Interact(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        if (interactable != null && obj.started || isMounting && obj.started)
        {
            if (!isInteracting)
            {
                StartCoroutine(InteractWithLauncher());
            }
        }
        else if (obj.canceled || interactable == null)
        {
            interactMeterCurrent = 0;
            isInteracting = false;
            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            interactable = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            interactable = null;
        }
    }

    private IEnumerator InteractWithLauncher()
    {
        isInteracting = true;
        while (interactMeterCurrent < _interactMeterCapacity)
        {
            interactMeterCurrent += _interactAmount;
            yield return new WaitForSeconds(_interactSpeed);
        }
        //if (interactable.GetComponent<MissileLauncher>())
        //{
        //    interactable.GetComponent<MissileLauncher>().SpawnMissileServerRpc();
        //}
        if (interactable.GetComponent<TurretController>() && !isMounting && interactable.GetComponent<TurretController>().n_isMounted.Value == false)
        {
            isMounting = true;
            interactable.GetComponent<TurretController>().Mount(true, GetComponent<PlayerInput>());
            GetComponent<DisableMinion>().Disable();
        }
        else if (isMounting)
        {
            interactable.GetComponent<TurretController>().Mount(false);
            GetComponent<DisableMinion>().Enable();
            isMounting = false;
        }
        
        interactMeterCurrent = 0;
        isInteracting = false;
    }
}
