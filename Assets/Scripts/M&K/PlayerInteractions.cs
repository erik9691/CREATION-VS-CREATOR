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
                StopAllCoroutines();
                StartCoroutine(InteractWithLauncher());
            }
        }
        else if (obj.canceled || interactable == null)
        {
            isInteracting = false;
            StopAllCoroutines();
            StartCoroutine(DrainSlider());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;
        if (other.tag == "Interactable")
        {
            interactable = other.transform.parent.gameObject;

            if (interactable.GetComponent<MissileLauncher>())
            {
                UIManager.Instance.ActivateInteractSlider(true);
            }
            else if (interactable.GetComponent<TurretController>().n_IsMounted.Value == false || interactable.GetComponent<TurretController>().n_IsMounted.Value == true && isMounting || !interactable.GetComponent<TurretController>().n_IsDestroyed.Value)
            {
                UIManager.Instance.ActivateInteractSlider(true);
            }
            else
            {
                UIManager.Instance.ActivateInteractSlider(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        if (other.tag == "Interactable")
        {
            interactable = null;
            UIManager.Instance.ActivateInteractSlider(false);
        }
    }

    private void Update()
    {
        if (isMounting && interactable.GetComponent<TurretController>().n_IsDestroyed.Value)
        {
            interactable.GetComponent<TurretController>().Mount(false);
            GetComponent<DisableMinion>().Enable();
            isMounting = false;
        }
    }

    private IEnumerator InteractWithLauncher()
    {
        isInteracting = true;
        while (interactMeterCurrent < _interactMeterCapacity)
        {
            interactMeterCurrent += _interactAmount;
            UIManager.Instance.UpdateInteractSlider(interactMeterCurrent);
            yield return new WaitForSeconds(_interactSpeed);
        }

        if (interactable.GetComponent<MissileLauncher>())
        {
            Debug.Log("SpawnMissile");
            interactable.GetComponent<MissileLauncher>().SpawnRocketServerRpc();
        }
        else if (interactable.GetComponent<TurretController>() && !isMounting && interactable.GetComponent<TurretController>().n_IsMounted.Value == false && interactable.GetComponent<TurretController>().n_IsDestroyed.Value == false)
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
        UIManager.Instance.UpdateInteractSlider(interactMeterCurrent);
        isInteracting = false;
    }

    private IEnumerator DrainSlider()
    {
        while (interactMeterCurrent > 0)
        {
            interactMeterCurrent -= _interactAmount;
            UIManager.Instance.UpdateInteractSlider(interactMeterCurrent);
            yield return new WaitForSeconds(_interactSpeed / 2);
        }
    }
}
