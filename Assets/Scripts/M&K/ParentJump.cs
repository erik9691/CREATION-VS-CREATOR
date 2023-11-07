using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentJump : MonoBehaviour
{
    public void JumpEvent()
    {
        GetComponentInParent<PlayerMovement>().JumpForce();
    }

    public void ReloadEvent()
    {
        GetComponentInParent<PlayerGun>().Reload();
    }
}
