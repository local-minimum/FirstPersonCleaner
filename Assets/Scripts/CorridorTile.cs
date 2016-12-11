using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Direction { North, East, South, West};

public class CorridorTile : MonoBehaviour {

    OneRoomDoor[] doors = new OneRoomDoor[0];
    
	[SerializeField]
    CorridorTile[] edges = new CorridorTile[4];

	[SerializeField]
	public List<Action> actions = new List<Action>();

    Vector3 offset = Vector3.up * 0.25f;

	[SerializeField]
	public bool hasGlitch;

    public bool HasDoor(OneRoomDoor door)
    {
        return doors.Contains(door);
    }

    public OneRoomDoor GetDoor(Direction direction)
    {
        Ray r = new Ray(playerPosition, GetLookDirection(direction));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 4, MouseController.DoorLayer))
        {
            OneRoomDoor door = hit.transform.GetComponentInParent<OneRoomDoor>();
            if (door != null && HasDoor(door))
            {
                return door;
            }            
        } 
        return null;
    }

	public Direction GetPreviousDirection(CorridorTile tile) {
		if (edges [0] == tile) {
			return Direction.North;
		}
		if (edges [1] == tile) {
			return Direction.East;
		}
		if (edges [2] == tile) {
			return Direction.South;
		}
		if (edges [3] == tile) {
			return Direction.West;
		}
		throw new UnityException ("Invalid direction");
	}
    
    public bool CloseAllDoors()
    {
        bool any = false;
        for (int i=0,l=doors.Length; i< l; i++)
        {
            any = any | doors[i].CloseDoor();
        }
        return any;
    }

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

    public static Direction GetLeftDirection(Direction from)
    {
        return (Direction)(((int)from + 3) % 4);
    }

    public static Direction GetRighDirection(Direction from)
    {
        return (Direction)(((int)from + 1) % 4);
    }

    public Vector3 playerPosition
    {
        get
        {
            return transform.position + offset;
        }
    }

    void Start()
    {
        doors = GetComponentsInChildren<OneRoomDoor>();
    }
}
