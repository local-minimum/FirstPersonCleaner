using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportRoom : MonoBehaviour {

	List<Room> rooms;

	[SerializeField]
	CorridorSegmentPlacer placer; 	

	[SerializeField]
	PlayerWalkController walkController;

	public void createRooms(int level) {
		if (rooms != null) {
			foreach (var room in rooms) {
				while(room.corridorTile.transform.GetChildCount()>0){
					GameObject.Destroy(room.corridorTile.transform.GetChild (0));
				}

				Destroy (room.corridorTile);
			}
		}
		rooms = new List<Room> ();
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
					if (cell.StartsWith ("*")) {
						startTile = room;
						cell = cell.Replace ("*", "");
					}
					if (cell.StartsWith ("#")) {
						endTile = room;
						cell = cell.Replace ("#", "");
					}
					int cellType = int.Parse (cell);
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
				corridor = placer.PlaceCorridor (room.north == null, room.west == null, room.south == null, room.east == null);
			}
			float x = room.col * size;
			float z = -room.row * size;
			corridor.transform.parent = transform;
			corridor.transform.position = (new Vector3(x, 0, z));
			var tile = corridor.GetComponent<CorridorTile> ();
			room.corridorTile = tile;
			tile.hasGlitch = room.HasGlitch;
			tile.actions.AddRange (room.actions);
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

	// Use this for initialization
	void Start () {
		createRooms (1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
