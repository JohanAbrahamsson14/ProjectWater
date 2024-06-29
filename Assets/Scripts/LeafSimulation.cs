using UnityEngine;

public class LeafSimulation : MonoBehaviour
{
    private Transform[] segments;
    private Mesh leafMesh;
    private int segmentCount;
    private float segmentLength;
    private float width;
    private float swayFrequency;
    private float swayAmplitude;
    private float buoyancyForce;
    private Material leafMaterial;

    public void Initialize(Vector3 direction, float angle, int segmentCount, float segmentLength, float width, float swayFrequency, float swayAmplitude, float buoyancyForce, Material leafMaterial)
    {
        this.segmentCount = segmentCount;
        this.segmentLength = segmentLength;
        this.width = width;
        this.swayFrequency = swayFrequency;
        this.swayAmplitude = swayAmplitude;
        this.buoyancyForce = buoyancyForce;
        this.leafMaterial = leafMaterial;

        InitializeSegments(direction, angle);
        CreateLeafMesh();
    }

    void FixedUpdate()
    {
        SimulateBuoyancyAndSway();
        UpdateLeafMesh();
    }

    private void InitializeSegments(Vector3 direction, float angle)
    {
        segments = new Transform[segmentCount];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject("LeafSegment" + i);
            segment.transform.position = startPosition + direction * segmentLength * i;
            segment.transform.parent = transform;
            segments[i] = segment.transform;

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.useGravity = false;

            if (i > 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = segments[i - 1].GetComponent<Rigidbody>();
                joint.anchor = Vector3.left * segmentLength;
                joint.axis = Vector3.up; // Allow swaying
                joint.secondaryAxis = Vector3.forward;

                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;

                joint.angularXMotion = ConfigurableJointMotion.Limited;
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                joint.angularZMotion = ConfigurableJointMotion.Limited;

                SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
                limitSpring.spring = 100;
                limitSpring.damper = 1;
                joint.angularXLimitSpring = limitSpring;
                joint.angularYZLimitSpring = limitSpring;

                SoftJointLimit lowAngularXLimit = new SoftJointLimit();
                lowAngularXLimit.limit = -15; // limit bending
                joint.lowAngularXLimit = lowAngularXLimit;

                SoftJointLimit highAngularXLimit = new SoftJointLimit();
                highAngularXLimit.limit = 15; // limit bending
                joint.highAngularXLimit = highAngularXLimit;

                SoftJointLimit angularYLimit = new SoftJointLimit();
                angularYLimit.limit = 15; // limit bending
                joint.angularYLimit = angularYLimit;

                SoftJointLimit angularZLimit = new SoftJointLimit();
                angularZLimit.limit = 15; // limit bending
                joint.angularZLimit = angularZLimit;
            }
            else
            {
                rb.isKinematic = true; // Anchor the first segment
                segment.transform.localRotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }

    private void SimulateBuoyancyAndSway()
    {
        for (int i = 1; i < segmentCount; i++)
        {
            Rigidbody rb = segments[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply buoyancy
                rb.AddForce(Vector3.up * buoyancyForce / segmentCount, ForceMode.Acceleration);

                // Apply swaying effect
                float swayOffset = Mathf.Sin(Time.time * swayFrequency + i) * swayAmplitude;
                rb.AddForce(new Vector3(swayOffset, 0, 0), ForceMode.Acceleration);

                // Apply wind effect
                rb.AddForce(Vector3.right * 0.1f, ForceMode.Acceleration); // Example wind effect
            }
        }
    }

    private void CreateLeafMesh()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = leafMaterial;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        leafMesh = new Mesh();

        Vector3[] vertices = new Vector3[segmentCount * 2];
        int[] triangles = new int[(segmentCount - 1) * 6];
        Vector2[] uv = new Vector2[segmentCount * 2];

        for (int i = 0; i < segmentCount; i++)
        {
            vertices[i * 2] = segments[i].localPosition + new Vector3(0, -width / 2, 0);
            vertices[i * 2 + 1] = segments[i].localPosition + new Vector3(0, width / 2, 0);

            uv[i * 2] = new Vector2((float)(i) / (segmentCount-1), 0);
            uv[i * 2 + 1] = new Vector2((float)(i) / (segmentCount-1), 1);

            if (i < segmentCount - 1)
            {
                int baseIndex = i * 6;
                triangles[baseIndex + 0] = i * 2;
                triangles[baseIndex + 1] = (i + 1) * 2;
                triangles[baseIndex + 2] = i * 2 + 1;
                triangles[baseIndex + 3] = i * 2 + 1;
                triangles[baseIndex + 4] = (i + 1) * 2;
                triangles[baseIndex + 5] = (i + 1) * 2 + 1;
            }
        }

        leafMesh.vertices = vertices;
        leafMesh.triangles = triangles;
        leafMesh.uv = uv;
        leafMesh.RecalculateBounds();
        leafMesh.RecalculateNormals();

        meshFilter.mesh = leafMesh;
    }

    private void UpdateLeafMesh()
    {
        Vector3[] vertices = leafMesh.vertices;

        for (int i = 0; i < segmentCount; i++)
        {
            vertices[i * 2] = segments[i].localPosition + new Vector3(0, -width / 2, 0);
            vertices[i * 2 + 1] = segments[i].localPosition + new Vector3(0, width / 2, 0);
        }

        leafMesh.vertices = vertices;
        leafMesh.RecalculateBounds();
        leafMesh.RecalculateNormals();
    }
}
