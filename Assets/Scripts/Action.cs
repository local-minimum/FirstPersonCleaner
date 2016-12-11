using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

	public string action;
	public List<string> data = new List<string> ();
	public CorridorTile tile;

	public Action(string toParse) {
		string[] parsed = toParse.Split (':');
		action = parsed [0];
		if (parsed.Length > 1) {
			for (int i = 1; i < parsed.Length; ++i) {
				data.Add (parsed [i]);
			}
		}
	}

	public bool IsActive(Direction fromDirection) {
		switch (action) {
		case "teleport":				
			if (HasValue (2)) {
				return fromDirection == GetDirection (2);
			}
			break;
		case "rotate":
			if (HasValue (1)) {
				return fromDirection == GetDirection (1);
			}
			break;
		case "lookat":
			if (HasValue (1)) {
				return fromDirection == GetDirection (1);
			}
			break;
		default:
			break;
		}
		return true;
	}

	public int GetInteger(int index) {
		return int.Parse(data[index]);
	}

	public bool HasValue(int index) {
		return index < data.Count;
	}

	public Direction GetDirection(int index) {
		if (data [index].ToLowerInvariant().Equals ("east")) {
			return Direction.East;
		}
		if (data [index].ToLowerInvariant().Equals ("north")) {
			return Direction.North;
		}
		if (data [index].ToLowerInvariant().Equals ("west")) {
			return Direction.West;
		}
		if (data [index].ToLowerInvariant().Equals ("south")) {
			return Direction.South;
		}
		return (Direction)(int.Parse (data [index])%4);
	}

	public bool NeedTile 
	{
		get 
		{
			return action == "teleport";
		}
	}

}
