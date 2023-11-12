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
        string secs;
        string minsandsecs;

        if (currentTime % 60 < 10)
        {
            secs = "0" + currentTime % 60;
        }
        else
        {
            secs = (currentTime % 60).ToString();
        }

        minsandsecs = currentTime / 60 + ":" + secs;
        UIManager.Instance.UpdateTime(minsandsecs);
    }
}
