using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UpdateTimer : NetworkBehaviour
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

        if (currentTime <= 0)
        {
            if (IsHost)
            {
                UIManager.Instance.GameWon();
            }
            else
            {
                UIManager.Instance.GameLost();
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
