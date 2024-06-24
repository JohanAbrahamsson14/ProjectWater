using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
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
    public bool IsStairs { get; set; }
}

public class Pathway
{
    public List<PathwaySegment> Segments { get; set; } = new List<PathwaySegment>();
}

public class UnderWaterMapGenerator : MonoBehaviour
{
    public int Seed;
    private System.Random random;

    public GameObject stationPrefab;
    public GameObject pathwayPrefab;
    public GameObject stairsPrefab;
    public GameObject brokenPrefab;
    public GameObject brokenStairsPrefab;
    
    public float pathwaySizeX = 10.0f; // Width of the pathway model
    public float pathwaySizeZ = 10.0f; // Depth of the pathway model
    public float stairsHeight = 3.0f;  // Height of the stairs model
    
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

        /*
        while (!EnsureConnectivity(stations, pathways))
        {
            pathways.Clear();
            GeneratePathways(stations, pathways);
        }
        */

        // Instantiate stations and pathways in the scene
        InstantiateMap(stations, pathways);
    }

    void GenerateStations(List<Station> stations)
    {
        
        Station startStation = new Station
        {
            Position = Vector3.zero,
            //Rooms = GenerateRooms(random.Next(5, 15))
        };
        stations.Add(startStation);
        
        for (int i = 1; i < numberOfStations; i++)
        {
            Station station = new Station
            {
                Position = new Vector3(
                    Mathf.Round(random.Next(-10, 10) * pathwaySizeX*2),
                    Mathf.Round(random.Next(-3, 3) * stairsHeight*2),
                    Mathf.Round(random.Next(8, 15) * pathwaySizeZ*2)
                ),
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
        Vector3 direction = Vector3.forward;
        Vector3 currentPos = start+direction*pathwaySizeX;

        float totalHeightDifference = Mathf.Abs(end.y - start.y);
        int totalSegments = Mathf.CeilToInt(Vector3.Distance(currentPos, end) / pathwaySizeX);
        int stairsSegments = Mathf.CeilToInt(totalHeightDifference / stairsHeight);

        int segmentsBetweenStairs = totalSegments / (stairsSegments + 1);

        int segmentCounter = 0;

        int randomDirection = 0;
        
        PathwaySegment firstSegment = new PathwaySegment
        {
            Start = start,
            End = currentPos,
            IsBroken = false,
            IsStairs = false,
        };
        pathway.Segments.Add(firstSegment);
        
        while (Vector3.Distance(currentPos, end) > pathwaySizeX || Mathf.Abs(end.y - currentPos.y) >= stairsHeight)
        {
            Vector3 nextPos = currentPos;
            
            // Ensure a more even distribution of stairs segments
            bool moveVertically = segmentCounter % (segmentsBetweenStairs + 1) == 0 && stairsSegments > 0;

            if (moveVertically)
            {
                nextPos.y += Mathf.Sign(end.y - currentPos.y) * stairsHeight;
                stairsSegments--;

                // Move horizontally as well to maintain pathway direction
                if (Mathf.Abs(end.x - currentPos.x) >= pathwaySizeX)
                {
                    nextPos.x += Mathf.Sign(end.x - currentPos.x) * pathwaySizeX;
                }
                else if (Mathf.Abs(end.z - currentPos.z) >= pathwaySizeZ)
                {
                    nextPos.z += Mathf.Sign(end.z - currentPos.z) * pathwaySizeZ;
                }

                nextPos = AlignToGrid(nextPos);

                PathwaySegment stairsSegment = new PathwaySegment
                {
                    Start = currentPos,
                    End = nextPos,
                    IsBroken = random.Next(0, 10) < 3, // 30% chance of being broken
                    IsStairs = true
                };

                pathway.Segments.Add(stairsSegment);
            }
            else
            {
                //70% Chance to be normal
                // Move in the X or Z direction if not moving vertically
                if (random.Next(0, 10) < 7 && Mathf.Abs(end.x - currentPos.x) >= pathwaySizeX)
                {
                    nextPos.x += Mathf.Sign(end.x - currentPos.x) * pathwaySizeX;
                }
                else if (Mathf.Abs(end.z - currentPos.z) >= pathwaySizeZ)
                {
                    nextPos.z += Mathf.Sign(end.z - currentPos.z) * pathwaySizeZ;
                }
                
                
                nextPos = AlignToGrid(nextPos);

                PathwaySegment segment = new PathwaySegment
                {
                    Start = currentPos,
                    End = nextPos,
                    IsBroken = random.Next(0, 10) < 3, // 30% chance of being broken
                    IsStairs = false
                };

                pathway.Segments.Add(segment);
            }
            segmentCounter++;
            
            if(random.Next(0,10)<2&&Vector3.Distance(currentPos, end) > pathwaySizeX*8&&Vector3.Distance(currentPos, start) > pathwaySizeX*8) CreateDeadEndPathway(nextPos,pathways, nextPos-currentPos);
            currentPos = nextPos;
        }

        /*
        // Add final segment to reach the end
        PathwaySegment finalSegment = new PathwaySegment
        {
            Start = currentPos,
            End = end,
            IsBroken = false,
            IsStairs = Mathf.Abs(end.y - currentPos.y) >= stairsHeight / 2
        };
        pathway.Segments.Add(finalSegment);
        */

        pathways.Add(pathway);
    }
}

Vector3 AlignToGrid(Vector3 position)
{
    float x = Mathf.Round(position.x / pathwaySizeX) * pathwaySizeX;
    float y = Mathf.Round(position.y / stairsHeight) * stairsHeight;
    float z = Mathf.Round(position.z / pathwaySizeZ) * pathwaySizeZ;

    return new Vector3(x, y, z);
}


void CreateDeadEndPathway(Vector3 startPos, List<Pathway> pathways, Vector3 notDirection)
{
    Pathway deadEndPathway = new Pathway();
    Vector3 currentPos = startPos;

    int deadEndLength = random.Next(2, 8); // Dead-end length between 2 to 7 segments
    int direction = random.Next(0, 2);

    bool isXNormal = Mathf.Abs(notDirection.x) <= Mathf.Abs(notDirection.z);

    for (int i = 0; i < deadEndLength; i++)
    {
        Vector3 nextPos = currentPos;

        // Randomly choose a direction for the dead-end pathway, ensuring grid alignment
        if (isXNormal)
        {
            nextPos.x += pathwaySizeX * (direction== 0 ? 1 : -1);
        }
        else if (!isXNormal)
        {
            nextPos.z += pathwaySizeZ * (direction== 0 ? 1 : -1);
        }
        
        if (random.Next(0, 10) < 1)
        {
            nextPos.y += stairsHeight * (random.Next(0, 2) == 0 ? 1 : -1);
        }

        nextPos = AlignToGrid(nextPos);

        PathwaySegment segment = new PathwaySegment
        {
            Start = currentPos,
            End = nextPos,
            IsBroken = random.Next(0, 10) < 3, // 30% chance of being broken
            IsStairs = Mathf.Abs(nextPos.y - currentPos.y) > stairsHeight / 2
        };

        deadEndPathway.Segments.Add(segment);
        currentPos = nextPos;
        if (random.Next(0, 10) < 2)
        {
            direction = random.Next(0, 2);
            isXNormal = !isXNormal;
        }
    }

    pathways.Add(deadEndPathway);
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
                    InstantiatePathwaySegment(segment);
            }
        }
    }

    void InstantiateStation(Station station)
    {
        // Instantiate station prefab at station.Position
        GameObject stationObj = Instantiate(stationPrefab, station.Position, Quaternion.identity);
        // Adjust station scale and rotation as needed
    }

    void InstantiatePathwaySegment(PathwaySegment segment)
    {
        if (segment.IsStairs)
        {
            if (segment.IsBroken)
            {
              InstantiateBrokenStairs(segment.Start, segment.End);
                return;
            }
            // Instantiate stairs model between segment.Start and segment.End
            InstantiateStairs(segment.Start, segment.End);
        }
        else
        {
            if (segment.IsBroken)
            {
               InstantiateBroken(segment.Start, segment.End);
                return;
            }
            // Instantiate flat pathway model between segment.Start and segment.End
            InstantiateFlatPathway(segment.Start, segment.End);
        }
    }

    
    void InstantiateBrokenStairs(Vector3 start, Vector3 end)
    {
        // Calculate the midpoint and direction
        Vector3 midpoint = (start + end) / 2;
        Vector3 direction = end - start;
        if (start.y > end.y) direction = start - end;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Instantiate your stairs prefab and position it between start and end
        GameObject stairs = Instantiate(brokenStairsPrefab, new Vector3(end.x, midpoint.y, end.z), rotation);
        // Adjust the scale and rotation as needed
        stairs.isStatic = true;
    }
    
    void InstantiateStairs(Vector3 start, Vector3 end)
    {
        // Calculate the midpoint and direction
        Vector3 midpoint = (start + end) / 2;
        Vector3 direction = end - start;
        if (start.y > end.y) direction = start - end;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Instantiate your stairs prefab and position it between start and end
        GameObject stairs = Instantiate(stairsPrefab, new Vector3(end.x, midpoint.y, end.z), rotation);
        // Adjust the scale and rotation as needed
        stairs.isStatic = true;
    }
    
     void InstantiateBroken(Vector3 start, Vector3 end)
     {
            // Calculate the midpoint and direction
            Vector3 midpoint = (start + end) / 2;
            Vector3 direction = end - start;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    
            // Instantiate your stairs prefab and position it between start and end
            GameObject broken = Instantiate(brokenPrefab, end, rotation);
            // Adjust the scale and rotation as needed
            broken.isStatic = true;
     }
    
    void InstantiateFlatPathway(Vector3 start, Vector3 end)
    {
        // Calculate the midpoint and direction
        Vector3 midpoint = (start + end) / 2;
        Vector3 direction = end - start;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Instantiate your flat pathway prefab and position it between start and end
        GameObject pathway = Instantiate(pathwayPrefab, end, rotation);
        // Adjust the scale and rotation as needed
        pathway.isStatic = true;
    }




    
    /*
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
    */

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
