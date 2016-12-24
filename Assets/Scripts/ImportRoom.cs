using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ImportRoom : MonoBehaviour {

	List<Room> rooms;

	[SerializeField]
	CorridorSegmentPlacer placer; 	

	[SerializeField]
	PlayerWalkController walkController;

    public void ManageDoors(CorridorTile currentTile, Direction dir)
    {        
        List<Direction> visiblDirections = new List<Direction>();
        List<Direction> noDirections = new List<Direction>();
        
        visiblDirections.Add(dir);
        visiblDirections.Add(CorridorTile.GetRighDirection(dir));

        Room thisRoom = rooms.Where(r => r.corridorTile == currentTile).FirstOrDefault();

        List<Room> checkRooms = rooms.Where(r => r.corridorTile != null && r.corridorTile != currentTile).ToList();
        Debug.Log(dir);
        for (int i=0, l=checkRooms.Count; i< l; i++)
        {
            Room room = checkRooms[i];
            
            if (RoomBehindMe(thisRoom, room, dir))
            {

                room.corridorTile.SoftMangageDoorRooms(noDirections);
            } else if (LineOfSight(thisRoom, room, dir))
            {

                room.corridorTile.SoftMangageDoorRooms(visiblDirections);
            } else
            {

                room.corridorTile.SoftMangageDoorRooms(noDirections);
            }
        }        
    }

    bool LineOfSight(Room thisRoom, Room otherRoom, Direction facingDirection)
    {
        int dRow = otherRoom.row - thisRoom.row;
        int dCol = otherRoom.col - thisRoom.col;

        switch (facingDirection)
        {
            case Direction.East:
                if (dRow == 0 && dCol > 0 && dCol < 5)
                {
                    return true;
                }
                else if (dRow == 1 && Mathf.Abs(dCol) == 1)
                {
                    return true;
                }
                return false;
            case Direction.West:
                if (dRow == 0 && dCol < 0 && dCol > -5)
                {
                    return true;
                }
                else if (dRow == -1 && Mathf.Abs(dCol) == 1)
                {
                    return true;
                }
                return false;
            case Direction.North:
                if (dCol == 0 && dRow < 0 && dRow > -5)
                {
                    return true;
                }
                else if (dCol == -1 && Mathf.Abs(dRow) == 1)
                {
                    return true;
                }
                return false;
            case Direction.South:
                if (dCol == 0 && dRow > 0 && dRow < 5)
                {
                    return true;
                }
                else if (dCol == 1 && Mathf.Abs(dRow) == 1)
                {
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    bool RoomBehindMe(Room thisRoom, Room otherRoom, Direction facingDirection)
    {
        switch (facingDirection)
        {
            case Direction.East:
                return otherRoom.col < thisRoom.col;
            case Direction.West:
                return otherRoom.col > thisRoom.col;
            case Direction.North:
                return otherRoom.row > thisRoom.row;
            case Direction.South:
                return otherRoom.row < thisRoom.row;
            default:
                return false;  
        }
    }

	public void createRooms(int level) {
		if (rooms != null) {
			foreach (var room in rooms) {
				Destroy (room.corridorObject);
			}
		}
		rooms = new List<Room> ();
		walkController.currentLevel = level;
		Room[][] matrix;
		Room startTile = null;
		Room endTile = null;

		TextAsset asset = Resources.Load ("level" + level) as TextAsset;
		string dataString = asset.text.Replace ("\r", "\n");
		string[] rows = dataString.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		matrix = new Room[rows.Length][]; 
		for (int row = 0; row < rows.Length; ++row) {
			string rowValue = rows [row];
			string[] columns = rowValue.Split (';');
			matrix [row] = new Room[columns.Length];
			for (int col = 0; col < columns.Length; ++col) {
				string[] cells = columns [col].Split (',');
				string cell = cells[0];
				if (cell != "") {
					Room room = new Room ();
					matrix [row] [col] = room;
					room.col = col;
					room.row = row;
                    room.level = level;
					if (cell.StartsWith ("*")) {
						startTile = room;
						cell = cell.Replace ("*", "");
					}
					if (cell.StartsWith ("#")) {
						endTile = room;
						cell = cell.Replace ("#", "");
					}
                    int cellType = 0;
                    try {
                        cellType = int.Parse(cell[0].ToString());
                    } catch (System.FormatException)
                    {
                        Debug.LogError(string.Format("Position {0} {1}, bad type '{2}'", row, col, cell));
                        throw;
                    }

					if (cells.Length > 1) {
						for (int i = 1; i < cells.Length; ++i) {
							room.actions.Add(new Action(cells[i]));
						}
					}
					room.type = (RoomType)cellType;
					rooms.Add (room);
				}
			}
		}

		int rowCount = matrix.Length;
		int colCount = matrix [0].Length;

		foreach (var room in rooms) {
			int col = room.col;
			int row = room.row;
			// north
			if (row > 0) {
				room.north = matrix [row - 1] [col];
			}
			// south
			if (row < rowCount - 1) {
				room.south = matrix [row + 1] [col];
			}
			// west
			if (col > 0) {
				room.west = matrix [row] [col - 1];
			}
			// east
			if (col < colCount - 1) {
				room.east = matrix [row] [col + 1];
			}
		}

		float size = 5f;

		foreach (var room in rooms) {
			GameObject corridor;
			if (room == endTile) {
				corridor = placer.PlaceEndTile (room.north == null, room.west == null, room.south == null, room.east == null);
			} else {
				corridor = placer.PlaceCorridor (room.actions, room.north == null, room.west == null, room.south == null, room.east == null);
			}
			float x = room.col * size;
			float z = -room.row * size;
			corridor.transform.parent = transform;
			corridor.transform.position = (new Vector3(x, 0, z));
			var tile = corridor.GetComponent<CorridorTile> ();

            room.corridorObject = corridor;
			room.corridorTile = tile;

            //Must come last
            tile.Initiate(room);
		}

		foreach (var room in rooms) {

			foreach (var action in room.actions) {
				if (action.NeedTile) {
					int row = action.GetInteger (0);
					int col = action.GetInteger (1);
					action.tile = matrix [row] [col].corridorTile;
				}
			}

			if (room.north != null) {
				room.corridorTile.SetEdge (Direction.North, room.north.corridorTile);					
			}
			if (room.east != null) {
				room.corridorTile.SetEdge (Direction.East, room.east.corridorTile);					
			}
			if (room.west != null) {
				room.corridorTile.SetEdge (Direction.West, room.west.corridorTile);					
			}
			if (room.south != null) {
				room.corridorTile.SetEdge (Direction.South, room.south.corridorTile);					
			}
		}

		walkController.SetCurrentTile(startTile.corridorTile);
		walkController.SetEndTile (endTile.corridorTile);
	}

    [SerializeField]
    int startLevel = 1;
	// Use this for initialization
	void Start () {
		createRooms (startLevel);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
