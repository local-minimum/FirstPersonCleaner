using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, East, South, West};

public class CorridorTile : MonoBehaviour {

    CorridorTile[] edges = new CorridorTile[4];

    public CorridorTile GetEdge(Direction direction)
    {
        return edges[(int)direction];
    }

    public void SetEdge(Direction direction, CorridorTile other)
    {
        edges[(int)direction] = other;
        
    }
}
