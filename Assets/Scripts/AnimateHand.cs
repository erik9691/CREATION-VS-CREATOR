using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHand : MonoBehaviour
{
    public InputActionReference gripReference;
    public InputActionReference triggerReference;

    Animator _handAnimator;
    float _gripValue;
    float _triggerValue;

    void Start()
    {
        _handAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //AnimateGrip();
        AnimateTrigger();
    }

    /* por ahora no necesitamos esta accion
    void AnimateGrip()
    {
        _gripValue = gripReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Grip", _gripValue);
    }
    */

    void AnimateTrigger()
    {
        _triggerValue = triggerReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Grip", _triggerValue);
    }
}
