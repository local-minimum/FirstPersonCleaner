using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Direction { North, East, South, West};

public class CorridorTile : MonoBehaviour {

    OneRoomDoor[] doors = new OneRoomDoor[0];
    
	[SerializeField]
    CorridorTile[] edges = new CorridorTile[4];

    Vector3 offset = Vector3.up * 0.25f;
	    
	public bool hasGlitch
    {
        get
        {
            return _roomData == null ? false : _roomData.HasGlitch;
        }
    }

    public bool HasDoor(OneRoomDoor door)
    {
        return doors.Contains(door);
    }

    public void SoftMangageDoorRooms(List<Direction> visibleDirections)
    {
        foreach (Direction direction in doorDirections.Keys)
        {
            OneRoomDoor room = doorDirections[direction];
            if (room.IsOpen)
            {                
                if (visibleDirections.Contains(direction))
                {
                    room.SoftOpenRoom();
                } else
                {
                    room.SoftCloseRoom();
                }
            }
        }
    }

    Room _roomData;

    public Room RoomData
    {
        get
        {
            return _roomData;
        }
    }

    public void Initiate(Room room)
    {
        _roomData = room;

        for (int i = 0, l = doors.Length; i < l; i++)
        {
            doors[i].SetupDoor();
        }

    }

    public List<Action> actions
    {
        get
        {
            return _roomData.actions;
        }
    }

    public void SoftMangageDoorRooms()
    {        
        foreach (Direction direction in doorDirections.Keys)
        {
            OneRoomDoor room = doorDirections[direction];
            if (room.IsOpen)
            {
                room.SoftOpenRoom();
            }
        }
    }

    Dictionary<Direction, OneRoomDoor> doorDirections = new Dictionary<Direction, OneRoomDoor>();

    public OneRoomDoor GetDoor(Direction direction)
    {
        if (doorDirections.ContainsKey(direction))
        {
            return doorDirections[direction];
        }

        Ray r = new Ray(playerPosition, GetLookDirection(direction));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 4, MouseController.DoorLayer))
        {
            OneRoomDoor door = hit.transform.GetComponentInParent<OneRoomDoor>();
            if (door != null && HasDoor(door))
            {
                doorDirections.Add(direction, door);
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

    void Awake()
    {
        doors = GetComponentsInChildren<OneRoomDoor>();
    }
}
