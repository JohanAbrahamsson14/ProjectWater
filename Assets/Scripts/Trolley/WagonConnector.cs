using UnityEngine;

public class WagonConnector : MonoBehaviour
{
    public GameObject wagonPrefab; // Prefab of the wagon to be instantiated
    public int numberOfWagons = 3;
    public float distanceBetweenWagons = 2f;

    void Start()
    {
        GameObject previousWagon = gameObject;
        for (int i = 0; i < numberOfWagons; i++)
        {
            // Calculate the correct position behind the previous wagon
            Vector3 spawnPosition = previousWagon.transform.position - previousWagon.transform.forward * distanceBetweenWagons;
            GameObject newWagon = Instantiate(wagonPrefab, spawnPosition, Quaternion.identity);

            // Setup the Rigidbody and HingeJoint
            Rigidbody rb = newWagon.GetComponent<Rigidbody>();
            rb.useGravity = false; // Turn off gravity

            HingeJoint joint = newWagon.AddComponent<HingeJoint>();
            joint.connectedBody = previousWagon.GetComponent<Rigidbody>();
            joint.anchor = Vector3.zero;
            joint.autoConfigureConnectedAnchor = true;

            previousWagon = newWagon;
        }
    }
}