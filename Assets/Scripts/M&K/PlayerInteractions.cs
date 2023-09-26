using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField]
    float _interactMeterCapacity = 5, _interactSpeed = 0.1f, _interactAmount = 0.1f;

    float interactMeterCurrent = 0;
    GameObject interactable;
    bool isInteracting = false;

    public void Interact(InputAction.CallbackContext obj)
    {
        if (interactable != null && obj.started)
        {
            Debug.Log("Start");
            if (!isInteracting)
            {
                StartCoroutine(InteractWithLauncher());
            }
        }
        else if (obj.canceled || interactable == null)
        {
            Debug.Log("Stop");
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
        if (other.tag == "Missile Launcher")
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
        if (interactable.GetComponent<MissileLauncher>())
        {
            interactable.GetComponent<MissileLauncher>().SpawnMissileServerRpc();
        }
        else
        {
            interactable.GetComponent<Turret>().SpawnTurretBulletServerRpc();
        }
        
        interactMeterCurrent = 0;
        isInteracting = false;
    }
}
