using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHealth : NetworkBehaviour
{
    public float MinionHealth = 10;

    [SerializeField] 
    float _dmgAmount = 1f,_dmgRate = 0.5f, _respawnTime = 5f;

    [SerializeField]
    SkinnedMeshRenderer skin;

    float maxHealth;

    PlayerMovement pMovement;
    PlayerRagdoll pRagdoll;

    Color[] ogColors = new Color[8];
    Color[] redColors = new Color[8];


    public override void OnNetworkSpawn()
    {
        maxHealth = MinionHealth;
        pMovement = GetComponent<PlayerMovement>();
        pRagdoll = GetComponent<PlayerRagdoll>();

        for (int i = 0; i < ogColors.Length; i++)
        {
            ogColors[i] = skin.materials[i].color;

            redColors[i] = skin.materials[i].color;
            redColors[i].b -= 50;
            redColors[i].g -= 50;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TakeDamage());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            pRagdoll.StopRagdollServerRpc();
            pRagdoll.EnableInputs();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Knock")
        {
            MinionHealth -= _dmgAmount;

            UpdateHealthUI();
            MakeRedServerRpc();

            if (MinionHealth > 0)
            {
                StartCoroutine(pRagdoll.Knockdown());
            }
        }
    }

    public IEnumerator TakeDamage(bool isOverlord = false, bool isRight = true)
    {
        if (isOverlord)
        {
            Transform hand = GameObject.FindGameObjectWithTag("Overlord").transform.GetChild(0);
            if (isRight)
            {
                hand.GetChild(2).GetComponent<GrabMinion>().anchor = Vector3.zero;
            }
            else
            {
                hand.GetChild(1).GetComponent<GrabMinion>().anchor = Vector3.zero;
            }
        }

        while (MinionHealth >= 0)
        {
            yield return new WaitForSeconds(_dmgRate);

            MinionHealth -= _dmgAmount;

            UpdateHealthUI();
            MakeRedServerRpc();
        }

        
    }

    void UpdateHealthUI()
    {
        if (MinionHealth <= (maxHealth - (maxHealth / 3)))
        {
            pMovement.HealthMult = 0.75f;
            UIManager.Instance.UpdateMinionHealth(1);
            if (MinionHealth <= (maxHealth - ((maxHealth / 3) * 2)))
            {
                pMovement.HealthMult = 0.5f;
                UIManager.Instance.UpdateMinionHealth(2);
                if (MinionHealth <= 0)
                {
                    pRagdoll.DisableInputs();
                    pRagdoll.StartRagdollServerRpc();

                    StartCoroutine(Respawn());
                }
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void MakeRedServerRpc()
    {
        StartCoroutine(MakeRedCoroutine());
        MakeRedClientRpc();
    }

    [ClientRpc]
    void MakeRedClientRpc()
    {
        StartCoroutine(MakeRedCoroutine());
    }

    IEnumerator MakeRedCoroutine()
    {
        for (int i = 0; i < ogColors.Length; i++)
        {
            skin.materials[i].color = redColors[i];
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < ogColors.Length; i++)
        {
            skin.materials[i].color = ogColors[i];
        }
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTime);

        pRagdoll.StopRagdollServerRpc();

        Vector3 spawnLocation = GameObject.Find("Minion Spawn").transform.position;
        transform.position = spawnLocation;
        transform.rotation = Quaternion.identity;

        MinionHealth = maxHealth;
        pMovement.HealthMult = 1f;
        UIManager.Instance.UpdateMinionHealth(0);

        pRagdoll.EnableInputs();
    }
}