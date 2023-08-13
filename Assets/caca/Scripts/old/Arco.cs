using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Arco : MonoBehaviour
{
    [SerializeField] GameObject arcoTrue;
    [SerializeField] TextMeshProUGUI pickup;
    [SerializeField] GameObject intro;
    [SerializeField] ArcoScript arquito;
    bool change = false;
    bool introSkippeada = false;

    // Start is called before the first frame update
    void Start()
    {
        pickup.text = string.Empty;
        arcoTrue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && change == true)
        {
            arcoTrue.SetActive(true);
            Destroy(gameObject);
            pickup.text = string.Empty;
            arquito.condDis = true;
        }
        if (Input.GetKeyDown(KeyCode.E) && introSkippeada == false)
        {
            intro.gameObject.SetActive(false);
            introSkippeada = true;  
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            pickup.text = "Toca la E para agarrar el arco";
            change = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            pickup.text = string.Empty;
            change = false;
        }
    }
}
