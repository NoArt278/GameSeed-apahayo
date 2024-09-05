using UnityEngine;

public enum WaypointType { FemaleChild, Male, BuffDude }

[System.Serializable]
public class Waypoint
{
    public WaypointType type;
    public Transform[] position;
}