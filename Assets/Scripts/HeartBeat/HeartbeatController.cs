using System.Collections;
using UnityEngine;
using FMODUnity;


public class HeartbeatController : MonoBehaviour
{
    // public AudioSource heartbeatSource;
    // public AudioClip heartbeatClip;
    public float bpm = 60f;
    private float nextBeatTime = 0f;
    private float beatInterval;

    public EventReference heartbeatRef;

    void Start()
    {

        /*if (heartbeatSource == null)
        {
            heartbeatSource = GetComponent<AudioSource>();
        }
        if (heartbeatSource == null)
        {
            Debug.LogError("HeartbeatController requires an AudioSource.");
            return;
        }
        
        if (heartbeatClip == null)
        {
            Debug.LogError("HeartbeatController requires an AudioClip.");
            return;
        }

        heartbeatSource.clip = heartbeatClip;*/

        beatInterval = 60f / bpm;
        nextBeatTime = Time.time + beatInterval;
    }

    void Update()
    {
        beatInterval = 60f / bpm;

        if (/*heartbeatSource != null &&*/ Time.time >= nextBeatTime)
        {
            RuntimeManager.PlayOneShot(heartbeatRef);
            //heartbeatSource.PlayOneShot(heartbeatClip);
            nextBeatTime = Time.time + beatInterval;
        }

    }

    public void SetBPM(float newBPM)
    {
        bpm = Mathf.Clamp(newBPM, 20f, 130f);  // Clamp BPM to a reasonable range
        beatInterval = 60f / bpm;
    }
}