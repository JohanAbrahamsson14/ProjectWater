using UnityEngine;

public class TrackInitializer : MonoBehaviour
{
    public TrackSegment initialSegment; // Reference to the initial track segment prefab

    void Start()
    {
        TrackFollower trackFollower = GetComponent<TrackFollower>();
        trackFollower.InitializeTrack(initialSegment);
    }
}