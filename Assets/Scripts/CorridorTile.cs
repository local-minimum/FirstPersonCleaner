using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, East, South, West};

public class CorridorTile : MonoBehaviour {

	[SerializeField]
    CorridorTile[] edges = new CorridorTile[4];

    Vector3 offset = Vector3.up * 0.25f;

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

    public static float GetRotation(Direction from, Direction to)
    {
        int iFrom = (int)from;
        int iTo = (int)to;
        int diff = iTo - iFrom;
        if (diff == 1 || diff == -3)
        {
            return 90;
        } else if (diff == -1 || diff == 3)
        {
            return -90;
        } else
        {
            return 180;
        }
    }

    public static float GetRotation(Direction from, bool right, out Direction to)
    {
        to = (Direction) (((int) from + (right ? 1 : 3)) % 4);
        return GetRotation(from, to);
    }

    public static Direction GetInverseDirection(Direction from)
    {
        return (Direction)(((int)from + 2) % 4);
    }

    public Vector3 playerPosition
    {
        get
        {
            return transform.position + offset;
        }
    }
}
