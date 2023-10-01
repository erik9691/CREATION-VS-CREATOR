using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    [SerializeField] float playerHealth = 10;
    [SerializeField] float dmgAmount = 1f;
    [SerializeField] float dmgRate = 0.5f;
    Rigidbody[] rb;
    [SerializeField] private Animator animator;

    public IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(dmgRate);
        playerHealth -= dmgAmount;
    }
    /*
    private void Start()
    {
        rb = transform.GetComponentInChildren<Rigidbody>();

        SetEnable(false);
    }
    private void Update()
    {
        if(playerHealth <= 0)
        {
            SetEnable(true);
        }
    }

    void SetEnable(bool enable)
    {
        bool isKinematic = !enable;
        foreach(Rigidbody rigidbody in rb)
        {
            rigidbody.isKinematic = isKinematic;
        }
        animator.enable = !enable;
    }
    */
}