using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WaterTrigger : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.SetInWater(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.SetInWater(false);
            }
        }
    }
}