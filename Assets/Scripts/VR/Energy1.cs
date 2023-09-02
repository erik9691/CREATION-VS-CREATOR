using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Energy1 : MonoBehaviour
{
    public float EnergyPoints;
    [SerializeField] float _energyLimit;

    [SerializeField] float _drainRate;
    [SerializeField] float _drainAmount;

    [SerializeField] InputAction grabInput;

    MeshRenderer handMesh;
    Material handMaterial;
    Color newColor;

    private void Start()
    {
        handMesh = GetComponent<MeshRenderer>();
        handMaterial = handMesh.material;
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("Exit");
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(DrainEnergy(true));
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Enter");
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(DrainEnergy(false));
        }
    }

    //note: mantener energylimit en 1 debido a que el maximo de opacidad es 1
    private IEnumerator DrainEnergy(bool drainOn)
    {
        while (drainOn && EnergyPoints > 0)
        {
            Debug.Log("DRAIN ON");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints -= _drainAmount;

            newColor = handMaterial.color;
            newColor.a -= _drainAmount;
            handMaterial.color = newColor;
        }
        //if (EnergyPoints == 0)
        //{
        //    grabInput.Disable();
        //}
        while (!drainOn && EnergyPoints < _energyLimit)
        {
            Debug.Log("DRAIN OFF");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints += _drainAmount;

            newColor = handMaterial.color;
            newColor.a += _drainAmount;
            handMaterial.color = newColor;
        }
        //if (EnergyPoints == _energyLimit)
        //{
        //    grabInput.Enable();
        //}
    }
}