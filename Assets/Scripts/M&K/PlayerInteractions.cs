using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField]
    float _interactMeterCapacity = 5, _interactSpeed = 0.1f, _interactAmount = 0.1f;

    float interactMeterCurrent = 0;
    GameObject missileLauncher;
    bool isInteracting = false;

    public void Interact(InputAction.CallbackContext obj)
    {
        if (missileLauncher != null && obj.started)
        {
            Debug.Log("Start");
            if (!isInteracting)
            {
                StartCoroutine(InteractWithLauncher());
            }
        }
        else if (obj.canceled || missileLauncher == null)
        {
            Debug.Log("Stop");
            interactMeterCurrent = 0;
            isInteracting = false;
            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Missile Launcher")
        {
            missileLauncher = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Missile Launcher")
        {
            missileLauncher = null;
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
        missileLauncher.GetComponent<MissileLauncher>().SpawnMissileServerRpc();
        interactMeterCurrent = 0;
        isInteracting = false;
    }
}
