using UnityEngine;

public class RopeSimulation : MonoBehaviour
{
    public int segmentCount = 10;
    public float segmentLength = 0.5f;
    public float ropeStiffness = 0.5f;
    public GameObject leafPrefab;
    public float buoyancyForce = 1.0f;

    private LineRenderer lineRenderer;
    private Rigidbody[] segments;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        segments = new Rigidbody[segmentCount];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject("Segment" + i);
            segment.transform.position = startPosition + Vector3.up * segmentLength * i;
            segment.transform.parent = transform; // Set parent to main rope GameObject

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.useGravity = false;
            segments[i] = rb;

            if (i == 0)
            {
                // Anchor the first segment
                rb.isKinematic = true;
                segment.transform.position = startPosition;
            }
            else
            {
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = segments[i - 1];
                joint.anchor = Vector3.down * segmentLength;
                joint.useSpring = true;
                JointSpring spring = joint.spring;
                spring.spring = ropeStiffness;
                joint.spring = spring;

                segment.AddComponent<Buoyancy>().buoyancyForce = buoyancyForce; // Add buoyancy
            }
        }

        AttachLeaves();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            lineRenderer.SetPosition(i, segments[i].transform.position);
        }
    }

    void AttachLeaves()
    {
        for (int i = 1; i < segmentCount - 1; i += 3)
        {
            addLeave(leafPrefab, Vector3.right, i);
            addLeave(leafPrefab, Vector3.left, i);
        }
    }

    void addLeave(GameObject leaf, Vector3 pos, int i)
    {
        leaf = Instantiate(leafPrefab, segments[i].transform);
        leaf.transform.localPosition = pos;
    }
}
