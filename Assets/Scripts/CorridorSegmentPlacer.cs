using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CorridorType { Default};

public class CorridorSegmentPlacer : MonoBehaviour {

    [SerializeField]
    Transform[] corridorPrefabs;

    [SerializeField]
    bool debugWestWall;

    [SerializeField]
    bool debugEastWall;

    [SerializeField]
    bool debugNorthWall;

    [SerializeField]
    bool debugSouthWall;

    [SerializeField]
    bool debugPlace;

	void Start () {

        if (debugPlace)
        {
            DebugPlaceCorridor();
        }
	}

    public GameObject PlaceCorridor(Vector3 position, CorridorType style, bool northWall, bool westWall, bool southWall, bool eastWall)
    {
        //TODO: add style selections
        return PlaceCorridor(position, northWall, westWall, southWall, eastWall);
    }


    public void DebugPlaceCorridor()
    {
        PlaceCorridor(transform.position, debugNorthWall, debugWestWall, debugSouthWall, debugEastWall);
    }

    public GameObject PlaceCorridor(bool northWall, bool westWall, bool southWall, bool eastWall)
    {
        return PlaceCorridor(transform.position, northWall, westWall, southWall, eastWall);
    }

    public GameObject PlaceCorridor(Vector3 position, bool northWall, bool westWall, bool southWall, bool eastWall)
    {
        Transform segment; 

        int walls = (northWall ? 1 : 0) + (southWall ? 1 : 0) + (westWall ? 1 : 0) + (eastWall ? 1 : 0);
        Debug.Log(walls);
        if (walls > 0 && walls < 4)
        {

            if (walls == 3)
            {
                segment = Instantiate(corridorPrefabs[3]);
                if (!eastWall)
                {
                    segment.Rotate(Vector3.up, 180);
                }
                else if (!northWall)
                {
                    segment.Rotate(Vector3.up, 90);
                }
                else if (!southWall)
                {
                    segment.Rotate(Vector3.up, -90);
                }
            }
            else if (walls == 1)
            {
                segment = Instantiate(corridorPrefabs[0]);
                if (southWall)
                {
                    segment.Rotate(Vector3.up, 180);
                }
                else if (eastWall)
                {
                    segment.Rotate(Vector3.up, 90);
                }
                else if (westWall)
                {
                    segment.Rotate(Vector3.up, -90);
                }
            }
            else
            {
                segment = Instantiate(corridorPrefabs[walls - 1]);
            }

        } else {
            throw new System.ArgumentException("1-3 walls must be set");
        }

        segment.position = position;

        return segment.gameObject;
    }
}
