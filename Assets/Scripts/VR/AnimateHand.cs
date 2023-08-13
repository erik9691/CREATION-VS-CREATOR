using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHand : MonoBehaviour
{
    [SerializeField] InputActionReference _gripReference;
    [SerializeField] InputActionReference _triggerReference;

    Animator handAnimator;
    float gripValue;
    float triggerValue;

    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //AnimateGrip();
        AnimateTrigger();
    }

    /* por ahora no necesitamos esta accion
    void AnimateGrip()
    {
        gripValue = _gripReference.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
    */

    void AnimateTrigger()
    {
        triggerValue = _triggerReference.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", triggerValue);
    }
}
