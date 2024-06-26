using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMAdjuster : MonoBehaviour
{
    public HeartbeatController heartbeatController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            heartbeatController.SetBPM(heartbeatController.bpm + 10f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            heartbeatController.SetBPM(heartbeatController.bpm - 10f);
        }
    }
}

