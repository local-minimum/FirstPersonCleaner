using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

	public Room east;
	public Room west;
	public Room north;
	public Room south;
	public CorridorTile corridorTile;
	public List<Action> actions = new List<Action>(); 

	public RoomType type;

	public int col;
	public int row;

	public bool HasGlitch {
		get {
			return this.type == RoomType.Type2;
		}
	}
}
