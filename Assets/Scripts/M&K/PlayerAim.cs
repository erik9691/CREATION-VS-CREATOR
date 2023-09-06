using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.Netcode;

public class PlayerAim : MonoBehaviour
{
    [SerializeField]
    int priorityBoostAmount = 10;

    Canvas thirdPersonCanvas;
    Canvas aimCanvas;

    public CinemachineVirtualCamera virtualCamera;

    public void Aim(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            virtualCamera.Priority += priorityBoostAmount;
            //aimCanvas.enabled = true;
            //thirdPersonCanvas.enabled = false;
        }
        else if (obj.canceled)
        {
            virtualCamera.Priority -= priorityBoostAmount;
            //aimCanvas.enabled = false;
            //thirdPersonCanvas.enabled = true;
        }
    }
}
