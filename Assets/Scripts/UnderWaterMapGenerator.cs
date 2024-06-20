using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Station
{
    public Vector3 Position { get; set; }
    public List<Room> Rooms { get; set; }
}

public class Room
{
    public Vector3 Position { get; set; }
    public GameObject Content { get; set; }
}

public class PathwaySegment
{
    public Vector3 Start { get; set; }
    public Vector3 End { get; set; }
    public bool IsBroken { get; set; }
}

public class Pathway
{
    public List<PathwaySegment> Segments { get; set; } = new List<PathwaySegment>();
}

public class UnderWaterMapGenerator : MonoBehaviour
{
    public int Seed;
    private System.Random random;

    public GameObject Station;
    public GameObject Path;
    public GameObject PathEnd;
    
    int numberOfStations = 2;

    void Start()
    {
        random = new System.Random(Seed);
        GenerateMap();
    }

    void GenerateMap()
    {
        List<Station> stations = new List<Station>();
        List<Pathway> pathways = new List<Pathway>();

        GenerateStations(stations);
        GeneratePathways(stations, pathways);

        while (!EnsureConnectivity(stations, pathways))
        {
            pathways.Clear();
            GeneratePathways(stations, pathways);
        }

        // Instantiate stations and pathways in the scene
        InstantiateMap(stations, pathways);
    }

    void GenerateStations(List<Station> stations)
    {
        for (int i = 0; i < numberOfStations; i++)
        {
            Station station = new Station
            {
                Position = new Vector3(random.Next(-25, 25), random.Next(-3, 3), random.Next(-25, 25)),
                //Rooms = GenerateRooms(random.Next(5, 15))
            };

            stations.Add(station);
        }
    }

    void GeneratePathways(List<Station> stations, List<Pathway> pathways)
    {
        for (int i = 0; i < stations.Count - 1; i++)
        {
            Pathway pathway = new Pathway();
            Vector3 start = stations[i].Position;
            Vector3 end = stations[i + 1].Position;

            // Limit the number of segments
            int maxSegments = 20;
            Vector3 currentPos = start;
            while (Vector3.Distance(currentPos, end) > 10 && maxSegments > 0)
            {
                Vector3 nextPos = Vector3.Lerp(currentPos, end, 0.1f);
                bool isBroken = random.Next(0, 10) < 3; // 30% chance of being broken
                PathwaySegment segment = new PathwaySegment { Start = currentPos, End = nextPos, IsBroken = isBroken };
                pathway.Segments.Add(segment);
                currentPos = nextPos;
                maxSegments--;
            }

            // Final segment to reach the end position
            PathwaySegment finalSegment = new PathwaySegment { Start = currentPos, End = end, IsBroken = false };
            pathway.Segments.Add(finalSegment);

            pathways.Add(pathway);
        }
    }

    void InstantiateMap(List<Station> stations, List<Pathway> pathways)
    {
        foreach (var station in stations)
        {
            InstantiateStation(station);
        }

        foreach (var pathway in pathways)
        {
            foreach (var segment in pathway.Segments)
            {
                if (!segment.IsBroken)
                {
                    InstantiatePathwaySegment(segment);
                }
            }
        }
    }

    void InstantiateStation(Station station)
    {
        // Instantiate station at station.Position
        // Use object pooling if possible
        Instantiate(Station, station.Position, quaternion.identity);
    }

    void InstantiatePathwaySegment(PathwaySegment segment)
    {
        // Instantiate pathway segment between segment.Start and segment.End
        // Use object pooling if possible
        Instantiate(Path, (segment.Start+segment.End)/2, quaternion.identity);
    }
    
    bool EnsureConnectivity(List<Station> stations, List<Pathway> pathways)
    {
        // Implement a simple connectivity check using BFS or DFS
        HashSet<Station> visited = new HashSet<Station>();
        Queue<Station> queue = new Queue<Station>();
        queue.Enqueue(stations[0]);

        while (queue.Count > 0)
        {
            Station current = queue.Dequeue();
            if (current == stations[^1]) // Reached the final station
                return true;

            visited.Add(current);

            foreach (var pathway in pathways)
            {
                Station neighbor = GetConnectedStation(current, pathway, stations);
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    Station GetConnectedStation(Station current, Pathway pathway, List<Station> stations)
    {
        foreach (var segment in pathway.Segments)
        {
            if (segment.Start == current.Position && !segment.IsBroken)
            {
                return stations.FirstOrDefault(s => s.Position == segment.End);
            }
            if (segment.End == current.Position && !segment.IsBroken)
            {
                return stations.FirstOrDefault(s => s.Position == segment.Start);
            }
        }
        return null;
    }
    
    List<Room> GenerateRooms(int numberOfRooms)
    {
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < numberOfRooms; i++)
        {
            Room room = new Room
            {
                Position = new Vector3(random.Next(-10, 10), random.Next(-10, 10), random.Next(-10, 10)),
                Content = GenerateRoomContent()
            };

            rooms.Add(room);
        }
        return rooms;
    }
    
    GameObject GenerateRoomContent()
    {
        // Instantiate and return room content
        return new GameObject("RoomContent");
    }

}
