using UnityEngine;

public class TrackIntersection : MonoBehaviour
{
    public TrackSegment path1Segment; // Reference to the first path segment prefab
    public TrackSegment path2Segment; // Reference to the second path segment prefab
    public bool choosePath2; // Bool to choose path2 if true, path1 if false
}
