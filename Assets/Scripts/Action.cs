using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

	public string action;
	public List<int> data = new List<int> ();
	public CorridorTile tile;

	public Action(string toParse) {
		string[] parsed = toParse.Split (':');
		action = parsed [0];
		if (parsed.Length > 1) {
			for (int i = 1; i < parsed.Length; ++i) {
				data.Add (int.Parse( parsed [i]));
			}
		}
	}

	public bool NeedTile 
	{
		get 
		{
			return action == "teleport";
		}
	}

}
