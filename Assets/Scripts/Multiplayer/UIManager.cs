using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject minionUI;
    [SerializeField] GameObject[] reticles;

    TextMeshProUGUI clipAmmoUI;
    TextMeshProUGUI storedAmmoUI;
    Slider interactSlider;
    Slider overlordSlider;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        clipAmmoUI = minionUI.transform.Find("Ammo").transform.Find("ClipAmmo").GetComponent<TextMeshProUGUI>();
        storedAmmoUI = minionUI.transform.Find("Ammo").transform.Find("StoredAmmo").GetComponent<TextMeshProUGUI>();
        interactSlider = minionUI.transform.Find("Interact").GetComponent<Slider>();
        overlordSlider = minionUI.transform.Find("OverlordHealth").GetComponent<Slider>();
    }

    public void ActivateMinionUI()
    {
        minionUI.SetActive(true);
        interactSlider.gameObject.SetActive(false);
        ChangeReticle(0);
    }

    public void UpdateAmmo(int cAmmo, int sAmmo)
    {
        clipAmmoUI.text = cAmmo.ToString();
        storedAmmoUI.text = sAmmo.ToString();
    }

    public void ActivateInteractSlider(bool activate)
    {
        interactSlider.gameObject.SetActive(activate);
    }
    public void UpdateInteractSlider(float current)
    {
        interactSlider.value = current;
    }

    public void ChangeReticle(int retIndex)
    {
        for (int i = 0; i < reticles.Length; i++)
        {
            if (i == retIndex)
            {
                reticles[i].SetActive(true);
            }
            else
            {
                reticles[i].SetActive(false);
            }
        }
    }

    public void UpdateOverlordHealth(float current)
    {
        overlordSlider.value = current;
    }
}
