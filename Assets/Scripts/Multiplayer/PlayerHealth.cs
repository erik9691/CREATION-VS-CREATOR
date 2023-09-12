using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float playerHealth = 10;
    [SerializeField] float dmgAmount = 1f;
    [SerializeField] float dmgRate = 0.5f;

    public IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(dmgRate);
        playerHealth -= dmgAmount;
    }
}
