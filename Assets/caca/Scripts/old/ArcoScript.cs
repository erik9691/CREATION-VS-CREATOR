using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcoScript : MonoBehaviour
{
    public bool condDis = false;
    [SerializeField] GameObject flecha;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float shootForce = 1500;
    [SerializeField] float shootRate = 1f;
    private float shootRateTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && condDis == true)
        {
            if(Time.time > shootRateTime)
            {
                GameObject newBullet;

                newBullet = Instantiate(flecha, spawnPoint.position, spawnPoint.transform.rotation);

                newBullet.GetComponent<Rigidbody>().AddForce(spawnPoint.up * shootForce);

                shootRateTime = Time.time + shootRate;

                Destroy(newBullet, 2);
            }
        }
    }

    void Disparar()
    {
        
    }
}
