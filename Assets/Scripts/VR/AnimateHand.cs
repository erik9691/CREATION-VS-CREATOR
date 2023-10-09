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
    public float gripValue;
    float triggerValue;
    float oldTriggerValue;
    float oldGripValue;

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
        gripValue = _gripReference.action.ReadValue<float>();

        if (triggerValue != oldTriggerValue)
        {
            if (triggerValue > gripValue || gripValue == 0)
            {
                AnimateHandServerRpc(triggerValue);
            }
        }

        if (gripValue != oldGripValue)
        {
            if (gripValue > triggerValue || triggerValue == 0)
            {
                AnimateHandServerRpc(gripValue);
            }
        }

        oldTriggerValue = triggerValue;
        oldGripValue = gripValue;
    }

    [ServerRpc]
    private void AnimateHandServerRpc(float value)
    {
        handAnimator.SetFloat("Grip", value);
    }


    
}
