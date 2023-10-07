using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerAim : MonoBehaviour
{
    int priorityBoostAmount = 10;
    CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera = GetComponent<AssignCamera>().cameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
    }

    public void Aim(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            virtualCamera.Priority += priorityBoostAmount;
            UIManager.Instance.ChangeReticle(1);
        }
        else if (obj.canceled)
        {
            virtualCamera.Priority -= priorityBoostAmount;
            UIManager.Instance.ChangeReticle(0);
        }
    }
}
