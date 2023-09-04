using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Energy : MonoBehaviour
{
    [SerializeField]
    float _drainRate = 0.5f, _drainAmount = 0.1f;

    [SerializeField] SkinnedMeshRenderer handMesh;

    public float EnergyPoints = 1;

    //note: mantener energylimit en 1 debido a que el maximo de opacidad es 1
    float _energyLimit = 1;
    Material handMaterial;
    Color newColor;
    XRBaseInteractor handInteractor;

    private void Start()
    {
        handInteractor = GetComponent<XRBaseInteractor>();
        handMaterial = handMesh.material;
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("Exit");
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(DrainEnergy());
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Enter");
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(RecoverEnergy());
        }
    }

    private IEnumerator DrainEnergy()
    {
        while (EnergyPoints > 0)
        {
            Debug.Log("DRAIN ON");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints -= _drainAmount;

            newColor = handMaterial.color;
            newColor.a -= _drainAmount;
            handMaterial.color = newColor;
        }
        if (EnergyPoints <= 0)
        {
            handInteractor.allowSelect = false;
        }
        
    }

    private IEnumerator RecoverEnergy()
    {
        while (EnergyPoints < _energyLimit)
        {
            Debug.Log("DRAIN OFF");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints += _drainAmount;

            newColor = handMaterial.color;
            newColor.a += _drainAmount;
            handMaterial.color = newColor;
        }
        if (EnergyPoints >= _energyLimit)
        {
            handInteractor.allowSelect = true;
        }
    }
}
