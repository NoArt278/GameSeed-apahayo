using UnityEngine;

public enum WaypointType { FemaleChild, Male, BuffDude }

[System.Serializable]
public class Waypoint
{
    public WaypointType Type;
    public Transform[] Position;
}