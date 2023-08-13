using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento : MonoBehaviour
{
    private float jumpForce = 250f, force = 7f, velocidadAgachado = 3f, velCorrer = 9f, fuerzaBala = 1000f;

    private Rigidbody rb;
    private PlayerInput pi;
    private Animator anim;

    private Vector2 input;

    public bool puedoSaltar;

    private float velocidadInicial;

    public CapsuleCollider colParado;
    public CapsuleCollider colAgachado;
    public GameObject cabeza;
    public Cabeza logicaCabeza;
    public bool estoyAgachado;
    public Camera camara1;
    public Camera camara2;

    private bool estoyCorriendo;

    public Transform spawnBala;
    public GameObject bala;
    public float shootRate = 1f;
    private float shootRateTime = 0;

    private void Start()
    {
        velocidadInicial = force;
        rb = GetComponent<Rigidbody>();
        pi = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        input = pi.actions["Move"].ReadValue<Vector2>();

        anim.SetFloat("VelX", input.x);
        anim.SetFloat("VelY", input.y);

        DejarAgachado();
        DejarDeCorrer();
    }

    private void FixedUpdate()
    {
        transform.Translate(input.x * Time.deltaTime * force, 0, input.y * Time.deltaTime * force);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && puedoSaltar)
        {
            anim.SetBool("salto", true);
            rb.AddForce(Vector3.up * jumpForce);
            puedoSaltar = false;


            anim.SetBool("salto", false);
            anim.SetBool("tocoSuelo", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "floor")
        {
            anim.SetBool("tocoSuelo", true);
            puedoSaltar = true;
        }
    }

    public void Crouch(InputAction.CallbackContext callbackContext)
    {
        if(puedoSaltar == true && estoyCorriendo == false)
        {
            anim.SetBool("agachado", true);
            force = velocidadAgachado;

            colAgachado.enabled = true;
            colParado.enabled = false;

            cabeza.SetActive(true);

            estoyAgachado = true;

            camara2.enabled = true;
            camara1.enabled = false;
        }
        if(callbackContext.phase == InputActionPhase.Canceled)
        {
            estoyAgachado = false;
        }
    }
    public void DejarAgachado()
    {
        if(estoyAgachado == false)
        {
            if (logicaCabeza.contadorDeColision <= 0)   
            {
                anim.SetBool("agachado", false);
                force = velocidadInicial;

                cabeza.SetActive(false);

                colAgachado.enabled = false;
                colParado.enabled = true;

                estoyAgachado = false;

                camara1.enabled = true;
                camara2.enabled = false;
            }
        }
    }


    public void Correr(InputAction.CallbackContext callbackContext)
    {
        if (!estoyAgachado && puedoSaltar)
        {
            force = velCorrer;
            if (input.y > 0)
            {
                anim.SetBool("correr", true);
                estoyCorriendo = true;
            }
            else
            {
                anim.SetBool("correr", false);
                estoyCorriendo = false;
            }
        }
        if (callbackContext.phase == InputActionPhase.Canceled)
        {
            estoyCorriendo = false;
        }
    }

    public void DejarDeCorrer()
    {
        if(estoyCorriendo == false)
        {
            anim.SetBool("correr", false);
        }
    }

    public void Shoot(InputAction.CallbackContext callbackContext)
    {

        if (Time.time > shootRateTime)
        {
            GameObject newBullet;

            newBullet = Instantiate(bala, spawnBala.position, spawnBala.transform.rotation);

            newBullet.GetComponent<Rigidbody>().AddForce(spawnBala.up * -fuerzaBala);

            shootRateTime = Time.time + shootRate;

            Destroy(newBullet, 2);
        }
    }




}
