using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, East, South, West};

public class CorridorTile : MonoBehaviour {

    CorridorTile[] edges = new CorridorTile[4];

    Vector3 offset = Vector3.up * 1.5f;

    public CorridorTile GetEdge(Direction direction)
    {
        return edges[(int)direction];
    }

    public void SetEdge(Direction direction, CorridorTile other)
    {
        edges[(int)direction] = other;
        
    }

    public static Vector3 GetLookDirection(Direction direction)
    {
        if (direction == Direction.North)
        {
            return Vector3.forward;
        } else if (direction == Direction.South)
        {
            return Vector3.forward * -1;
        } else if (direction == Direction.West)
        {
            return Vector3.right * -1;
        } else
        {
            return Vector3.right;
        }

    }

    public Vector3 playerPosition
    {
        get
        {
            return transform.position + offset;
        }
    }
}
