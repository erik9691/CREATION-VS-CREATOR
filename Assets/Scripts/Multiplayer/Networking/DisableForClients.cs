using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableForClients : NetworkBehaviour
{
    private void Start()
    {
        if (!IsHost)
        {
            Destroy(gameObject);
        }
    }
}
