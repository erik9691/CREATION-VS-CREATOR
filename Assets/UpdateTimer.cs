using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTimer : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TimeCount>().n_Time.OnValueChanged += TimeChanged;
    }

    void TimeChanged(int prevTime, int currentTime)
    {
        string minutosYSegs = currentTime / 60 + ":" + currentTime % 60;
        Debug.Log(minutosYSegs);
        UIManager.Instance.UpdateTime(minutosYSegs);
    }
}
