using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public float EnergyPoints;
    [SerializeField] float _energyLimit;

    [SerializeField] float _drainRate;
    [SerializeField] float _drainAmount;

    MeshRenderer handMesh;
    public Material handMaterial;
    public Color clr;

    private void Start()
    {
        handMesh = GetComponent<MeshRenderer>();
        handMaterial = handMesh.material;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(DrainEnergy(true));
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Safe Area")
        {
            StopAllCoroutines();
            StartCoroutine(DrainEnergy(false));
        }
    }

    private IEnumerator DrainEnergy(bool drainOn)
    {
        while (drainOn && EnergyPoints > 0)
        {
            Debug.Log("DRAIN ON");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints -= _drainAmount;
            clr = Color.white;
            handMaterial.color = clr;
        }
        while (!drainOn && EnergyPoints < _energyLimit)
        {
            Debug.Log("DRAIN OFF");
            yield return new WaitForSeconds(_drainRate);
            EnergyPoints += _drainAmount;
            clr = Color.white;
            handMaterial.color = clr;
        }
    }
}
