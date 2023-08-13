using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public PlayerInput playerInput;

    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 200.0f;

    private Animator anim;
    [SerializeField] float x, y;

    [SerializeField] Rigidbody rb;
    [SerializeField] float fuerzaSalto = 15.0f;
    public bool puedoSaltar;

    public float velocidadInicial;
    public float velocidadAgachado;

    public CapsuleCollider colParado;
    public CapsuleCollider colAgachado;
    public GameObject cabeza;
    public Cabeza logicaCabeza;
    public bool estoyAgachado;

    public Camera camara1;
    public Camera camara2;

    public GameObject arco;

    public int fuerzaExtra = 2;

    float velCorrer = 8;
    bool estoyCorriendo = false;

    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        puedoSaltar = false;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        velocidadInicial = movementSpeed;
        velocidadAgachado = movementSpeed * 0.5f;

        camara2.enabled = false;
        
    }

     void FixedUpdate()
    {
        transform.Translate(x*Time.deltaTime * movementSpeed, 0, y * Time.deltaTime * movementSpeed);
    }
    
    void Update()
    {
        Movimiento();
        Correr();
        Saltar();
        
    }

    
    
    
    public void Movimiento()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);
    }
    public void Cayendo()
    {
        rb.AddForce(fuerzaExtra * Physics.gravity);

        anim.SetBool("tocoSuelo", false);
        anim.SetBool("salto", false);
    }

    void Correr()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !estoyAgachado && puedoSaltar)
        {
            movementSpeed = velCorrer;
            if (y > 0)
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
        else
        {
            anim.SetBool("correr", false);
            estoyCorriendo = false;
        }
    }
    void Saltar()
    {
        if (puedoSaltar)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("salto", true);
                rb.AddForce(new Vector3(0, fuerzaSalto, 0), ForceMode.Impulse);
            }


            if (Input.GetKey(KeyCode.C) && estoyCorriendo == false)
            {
                anim.SetBool("agachado", true);
                movementSpeed = velocidadAgachado;

                colAgachado.enabled = true;
                colParado.enabled = false;

                cabeza.SetActive(true);

                estoyAgachado = true;

                camara2.enabled = true;
                camara1.enabled = false;
            }
            else
            {
                if (logicaCabeza.contadorDeColision <= 0)
                {
                    anim.SetBool("agachado", false);
                    movementSpeed = velocidadInicial;

                    cabeza.SetActive(false);

                    colAgachado.enabled = false;
                    colParado.enabled = true;

                    estoyAgachado = false;

                    camara1.enabled = true;
                    camara2.enabled = false;
                }
            }
            anim.SetBool("tocoSuelo", true);
        }
        else
        {
            Cayendo();
        }
    }
}
