using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int playerHealth = 10;

    public void TakeDamage()
    {
        playerHealth--;
    }
}
