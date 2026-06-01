using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPart : MonoBehaviour
{
    [SerializeField] RoadPart nextRoadPart; 
    [SerializeField] RoadPart previousRoadPart;
    [SerializeField] Vector3 myWaypoint; // Kart will move towards this point if target point is this part of the road
    [SerializeField] int index;

    public RoadPart NextRoadPart { get { return nextRoadPart; } }
    public RoadPart PreviousRoadPart { get { return previousRoadPart; } }
    public Vector3 MyWaypoint { get { return myWaypoint; } }
    public int Index { get { return index; } }

    public void SetRoadPart(RoadPart nextRoadPart, RoadPart previousRoadPart, float heightOffset, int index)
    {
        this.nextRoadPart = nextRoadPart;
        this.previousRoadPart = previousRoadPart;

        myWaypoint = new Vector3(transform.position.x, transform.position.y + heightOffset, transform.position.z);

        this.index = index;
    }
}
