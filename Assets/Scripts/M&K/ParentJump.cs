using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentJump : MonoBehaviour
{
    public void JumpEvent()
    {
        GetComponentInParent<PlayerMovement>().JumpForce();
    }
}
