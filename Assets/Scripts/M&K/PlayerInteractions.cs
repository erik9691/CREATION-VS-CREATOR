using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerInteractions : NetworkBehaviour
{
    [SerializeField]
    float _interactMeterCapacity = 5, _interactSpeed = 0.1f, _interactAmount = 0.1f;

    float interactMeterCurrent = 0;
    GameObject interactable;
    bool isInteracting = false;
    bool isMounting = false;
    Slider interactSlider;

    private void Start()
    {
        interactSlider = GameObject.Find("Canvas").transform.Find("Minion UI").transform.Find("Interact").GetComponent<Slider>();
        interactSlider.gameObject.SetActive(false);
    }


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

            if (interactable.GetComponent<TurretController>().n_isMounted.Value == false || interactable.GetComponent<TurretController>().n_isMounted.Value == true && isMounting)
            {
                interactSlider.gameObject.SetActive(true);
            }
            else
            {
                interactSlider.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        if (other.tag == "Interactable")
        {
            interactable = null;
            interactSlider.gameObject.SetActive(false);
        }
    }

    private IEnumerator InteractWithLauncher()
    {
        isInteracting = true;
        while (interactMeterCurrent < _interactMeterCapacity)
        {
            interactMeterCurrent += _interactAmount;
            interactSlider.value = interactMeterCurrent;
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
        interactSlider.value = interactMeterCurrent;
        isInteracting = false;
    }

    private IEnumerator DrainSlider()
    {
        while (interactMeterCurrent > 0)
        {
            interactMeterCurrent -= _interactAmount;
            interactSlider.value = interactMeterCurrent;
            yield return new WaitForSeconds(_interactSpeed / 2);
        }
        
    }
}
