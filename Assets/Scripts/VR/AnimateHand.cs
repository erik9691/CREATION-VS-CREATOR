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
        gripValue = _gripReference.action.ReadValue<float>();

        if (gripValue != oldGripValue)
        {
            AnimateHandServerRpc(gripValue);
        }
        
        oldGripValue = gripValue;
    }

    [ServerRpc]
    private void AnimateHandServerRpc(float value)
    {
        handAnimator.SetFloat("Grip", value);
    }


    
}
