using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

	public Room east;
	public Room west;
	public Room north;
	public Room south;
	public CorridorTile corridorTile;

	public RoomType type;

	public int col;
	public int row;
}
