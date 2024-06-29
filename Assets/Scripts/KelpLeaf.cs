using UnityEngine;

public class KelpLeaf : MonoBehaviour
{
    public float swayFrequency = 1.0f;
    public float swayAmplitude = 0.1f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float swayOffset = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;
        transform.localPosition = initialPosition + new Vector3(swayOffset, 0, 0);
    }
}