using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class AnimateHandALT : NetworkBehaviour
{
    [SerializeField] InputActionReference _gripReference;
    [SerializeField] InputActionReference _triggerReference;

    Animator handAnimator;

    private void Awake()
    {
        handAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        //AnimateTrigger();
        AnimateHandy(_gripReference.action.ReadValue<float>(), _triggerReference.action.ReadValue<float>());
    }

    void AnimateHandy(float gripValue, float triggerValue)
    {
        handAnimator.SetFloat("Grip", gripValue);
        handAnimator.SetFloat("Trigger", triggerValue);
    }

    //void AnimateTrigger()
    //{
    //    triggerValue = _triggerReference.action.ReadValue<float>();
    //    gripValue = _gripReference.action.ReadValue<float>();

    //    if (triggerValue != oldTriggerValue)
    //    {
    //        if (triggerValue > gripValue || gripValue == 0)
    //        {
    //            AnimateHandServerRpc(triggerValue);
    //        }
    //    }

    //    if (gripValue != oldGripValue)
    //    {
    //        if (gripValue > triggerValue || triggerValue == 0)
    //        {
    //            AnimateHandServerRpc(gripValue);
    //        }
    //    }

    //    oldTriggerValue = triggerValue;
    //    oldGripValue = gripValue;
    //}

    //[ServerRpc]
    //private void AnimateHandServerRpc(float value)
    //{
    //    handAnimator.SetFloat("Grip", value);
    //}



}
