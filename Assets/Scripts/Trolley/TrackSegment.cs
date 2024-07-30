using UnityEngine;
using System.Collections.Generic;

public class TrackSegment : MonoBehaviour
{
    public List<Transform> keyPoints = new List<Transform>(); // Ensure key points are unique per segment
    public List<Transform> controlPoints = new List<Transform>();
    public TrackIntersection intersection; // Reference to a potential intersection point
    public TrackSegment nextSegment; // Reference to the next track segment if no intersection
}