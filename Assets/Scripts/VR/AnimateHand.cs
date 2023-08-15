using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class AnimateHand : NetworkBehaviour
{
    [SerializeField] InputActionReference _gripReference;
    [SerializeField] InputActionReference _triggerReference;

    Animator handAnimator;
    float gripValue;
    float triggerValue;
    float oldTriggerValue;

    private void Awake()
    {
        handAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        
        AnimateTrigger();
        
    }

    void AnimateTrigger()
    {
        triggerValue = _triggerReference.action.ReadValue<float>();
        if (triggerValue != oldTriggerValue)
        {
            AnimateTriggerServerRpc();
        }
        oldTriggerValue = triggerValue;
    }

    [ServerRpc]
    private void AnimateTriggerServerRpc()
    {
        handAnimator.SetFloat("Grip", triggerValue);
    }


        /* por ahora no necesitamos esta accion
    void AnimateGrip()
    {
        gripValue = _gripReference.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
    */
}
