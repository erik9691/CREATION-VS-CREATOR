using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cabeza : MonoBehaviour
{

    public int contadorDeColision = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        contadorDeColision++;
    }

    private void OnTriggerExit(Collider other)
    {
        contadorDeColision--;
    }
}
