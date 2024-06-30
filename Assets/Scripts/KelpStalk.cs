using UnityEngine;

public class KelpStalk : MonoBehaviour
{
    [Header("Stalk Settings")]
    public int segmentCount = 10;
    public float segmentLength = 0.5f;
    public int radialSegments = 8;
    public float baseRadius = 0.1f;
    public float topRadius = 0.05f;
    public Material stalkMaterial;

    [Header("Stalk Buoyancy and Swaying")]
    public float stalkBuoyancyForce = 1.0f;
    public float stalkSwayFrequency = 1.0f;
    public float stalkSwayAmplitude = 0.1f;
    public Vector3 windDirection = Vector3.right;
    public float windForce = 0.1f;

    [Header("Leaf Settings")]
    public int leafSegmentCount = 5;
    public float leafSegmentLength = 0.2f;
    public float leafWidth = 0.2f;
    public int leafGroups = 3;
    public int leavesPerSegment = 3;
    public int leavesAtTopSegment = 10;
    public float leafRandomness = 0.1f;
    public float leafBuoyancyForce = 1f;
    public float swayFrequency = 1.0f;
    public float swayAmplitude = 0.1f;
    public Material leafMaterial;

    private Transform[] segments;
    private Rigidbody[] segmentRigidbodies;

    void Start()
    {
        segments = new Transform[segmentCount];
        segmentRigidbodies = new Rigidbody[segmentCount];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject("Segment" + i);
            segment.transform.position = startPosition + Vector3.up * segmentLength * i;
            segment.transform.parent = transform;
            segments[i] = segment.transform;

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.useGravity = false;
            segmentRigidbodies[i] = rb;

            if (i > 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = segments[i - 1].GetComponent<Rigidbody>();
                joint.anchor = Vector3.down * segmentLength;
                joint.axis = Vector3.forward; // Allow swaying
                joint.secondaryAxis = Vector3.right;

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

            if (i == 0)
            {
                rb.isKinematic = true; // Anchor the first segment
            }
        }

        AttachLeaves();
        CreateCylindricalSkinnedMesh();
    }

    void FixedUpdate()
    {
        SimulateBuoyancyAndSway();
    }

    void SimulateBuoyancyAndSway()
    {
        for (int i = 1; i < segmentCount; i++)
        {
            Rigidbody rb = segmentRigidbodies[i];
            // Apply buoyancy
            rb.AddForce(Vector3.up * stalkBuoyancyForce / segmentCount, ForceMode.Acceleration);

            // Apply swaying effect
            float swayOffset = Mathf.Sin(Time.time * stalkSwayFrequency + i) * stalkSwayAmplitude;
            rb.AddForce(new Vector3(swayOffset, 0, 0), ForceMode.Acceleration);

            // Apply wind effect
            rb.AddForce(windDirection * windForce, ForceMode.Acceleration);
        }
    }

    void AttachLeaves()
    {
        for (int i = 1; i < segmentCount; i++)
        {
            int numLeaves = (i == segmentCount - 1) ? leavesAtTopSegment : leavesPerSegment;

            for (int j = 0; j < numLeaves; j++)
            {
                if (Random.value > 0.5f)
                {
                    GameObject leaf = new GameObject("Leaf");
                    leaf.transform.parent = segments[i];

                    float angle = Random.Range(0f, 360f);
                    float radians = angle * Mathf.Deg2Rad;

                    // Random position around the stalk
                    Vector3 direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
                    leaf.transform.localPosition = direction * GetRadiusAtHeight(i);

                    // Random rotation around the stalk
                    Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), angle, 0);
                    leaf.transform.localRotation = randomRotation;

                    LeafSimulation leafSim = leaf.AddComponent<LeafSimulation>();
                    leafSim.Initialize(direction, angle, leafSegmentCount, leafSegmentLength, leafWidth, swayFrequency, swayAmplitude + Random.Range(-leafRandomness, leafRandomness), leafBuoyancyForce, leafMaterial);

                    // For top segment, make leaves point upwards
                    if (i == segmentCount - 1)
                    {
                        leaf.transform.localRotation = Quaternion.Euler(-90f, angle, 0);
                    }
                }
            }
        }
    }

    float GetRadiusAtHeight(int segmentIndex)
    {
        float t = (float)segmentIndex / (segmentCount - 1);
        return Mathf.Lerp(baseRadius, topRadius, t);
    }

    void CreateCylindricalSkinnedMesh()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();

        Mesh mesh = new Mesh();

        int vertexCount = segmentCount * radialSegments;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        BoneWeight[] boneWeights = new BoneWeight[vertexCount];
        Transform[] bones = new Transform[segmentCount];
        Matrix4x4[] bindPoses = new Matrix4x4[segmentCount];
        int[] triangles = new int[(segmentCount - 1) * radialSegments * 6];

        for (int i = 0; i < segmentCount; i++)
        {
            float y = i * segmentLength;
            float radius = GetRadiusAtHeight(i);
            for (int j = 0; j < radialSegments; j++)
            {
                float angle = j * Mathf.PI * 2 / radialSegments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices[i * radialSegments + j] = new Vector3(x, y, z);
                uv[i * radialSegments + j] = new Vector2((float)j / radialSegments, (float)i / segmentCount);

                boneWeights[i * radialSegments + j].boneIndex0 = i;
                boneWeights[i * radialSegments + j].weight0 = 1.0f;
            }

            bones[i] = segments[i];
            bindPoses[i] = segments[i].worldToLocalMatrix * transform.localToWorldMatrix;

            if (i < segmentCount - 1)
            {
                for (int j = 0; j < radialSegments; j++)
                {
                    int nextJ = (j + 1) % radialSegments;

                    int baseIndex = (i * radialSegments + j) * 6;
                    triangles[baseIndex + 0] = i * radialSegments + j;
                    triangles[baseIndex + 1] = (i + 1) * radialSegments + j;
                    triangles[baseIndex + 2] = i * radialSegments + nextJ;
                    triangles[baseIndex + 3] = i * radialSegments + nextJ;
                    triangles[baseIndex + 4] = (i + 1) * radialSegments + j;
                    triangles[baseIndex + 5] = (i + 1) * radialSegments + nextJ;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindPoses;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        skinnedMeshRenderer.sharedMesh = mesh;
        skinnedMeshRenderer.bones = bones;
        skinnedMeshRenderer.rootBone = segments[0];
        skinnedMeshRenderer.material = stalkMaterial != null ? stalkMaterial : new Material(Shader.Find("Standard")) { color = Color.green };
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, baseRadius);
    }
}
