using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHP : MonoBehaviour
{
    XRBaseInteractor handInteractor;
    [SerializeField] int handHP = 10;
    [SerializeField] int totalHandHP = 10;
    bool canDamage = false;

    private void Start()
    {
        handInteractor = GetComponentInParent<XRBaseInteractor>();
    }

    public void OnSelectEnter(SelectEnterEventArgs eventArgs)
    {
        canDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" && canDamage)
        {
            handHP--;
            if (handHP <= 0)
            {
                handInteractor.allowSelect = false;
                StartCoroutine(RecoverHand());
            }
        }
    }

    IEnumerator RecoverHand()
    {
        yield return new WaitForSeconds(2f);
        handInteractor.allowSelect = true;
        handHP = totalHandHP;
    }
}
