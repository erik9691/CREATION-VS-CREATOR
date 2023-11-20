using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject minionUI;
    [SerializeField] Sprite[] reticles;
    [SerializeField] Sprite[] statuses;
    [SerializeField] TextMeshProUGUI[] minuteCounters;

    TextMeshProUGUI clipAmmoUI;
    TextMeshProUGUI storedAmmoUI;
    Slider interactSlider;
    Slider overlordSlider;
    Image minionStatus;
    Image minionReticle;

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
        minionStatus = minionUI.transform.Find("Status").GetComponent<Image>();
        minionReticle = minionUI.transform.Find("Crosshair").GetComponent<Image>();
    }

    public void ActivateMinionUI()
    {
        minionUI.SetActive(true);
        interactSlider.gameObject.SetActive(false);
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
        minionReticle.sprite = reticles[retIndex];
    }
    public void UpdateMinionHealth(int statusIndex)
    {
        minionStatus.sprite = statuses[statusIndex];
    }

    public void UpdateOverlordHealth(float current)
    {
        overlordSlider.value = current;
    }

    public void UpdateTime(string currentTime)
    {
        foreach (TextMeshProUGUI Counter in minuteCounters)
        {
            Counter.text = currentTime;
        }
    }
}
