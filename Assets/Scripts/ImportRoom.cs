using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportRoom : MonoBehaviour {

	List<Room> rooms;

	[SerializeField]
	CorridorSegmentPlacer placer; 	

	[SerializeField]
	PlayerWalkController walkController;

	// Use this for initialization
	void Start () {
		rooms = new List<Room> ();
		Room[][] matrix;
		Room startTile = null;

		TextAsset asset = Resources.Load ("level1") as TextAsset;
		string dataString = asset.text.Replace ("\r", "\n");
		string[] rows = dataString.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		matrix = new Room[rows.Length][]; 
		for (int row = 0; row < rows.Length; ++row) {
			string rowValue = rows [row];
			string[] columns = rowValue.Split (';');
			matrix [row] = new Room[columns.Length];
			for (int col = 0; col < columns.Length; ++col) {
				string cell = columns [col];
				if (cell != "") {
					Room room = new Room ();
					matrix [row] [col] = room;
					room.col = col;
					room.row = row;
					if (cell.StartsWith ("*")) {
						startTile = room;
						cell = cell.Replace ("*", "");
					}
					int cellType = int.Parse (cell);
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

		float size = 4f;

		foreach (var room in rooms) {
			GameObject corridor = placer.PlaceCorridor (room.north == null, room.west == null, room.south == null, room.east == null);
			float x = room.col * size;
			float z = -room.row * size;
			corridor.transform.position = (new Vector3(x, 0, z));
			var tile = corridor.GetComponent<CorridorTile> ();
			room.corridorTile = tile;
		}

		foreach (var room in rooms) {
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
