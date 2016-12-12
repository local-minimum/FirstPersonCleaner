using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum CorridorType { Default};

public class CorridorSegmentPlacer : MonoBehaviour {

    [SerializeField]
    LayerMask doorLayer;

    [SerializeField]
    Transform elevatorPrefab;

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

    [SerializeField]
    Material carpetMat;

    [SerializeField]
    Material spinMat;

	void Start () {

        if (debugPlace)
        {
            DebugPlaceCorridor();
        }
	}

    public void DebugPlaceCorridor()
    {
        PlaceCorridor(debugNorthWall, debugWestWall, debugSouthWall, debugEastWall);
    }

	public GameObject PlaceEndTile(bool northWall, bool westWall, bool southWall, bool eastWall)
	{
		Vector3 position = transform.position;
		Transform segment; 

		segment = Instantiate (elevatorPrefab);
		if (!westWall) {
			segment.Rotate (Vector3.up, 180);
		} else if (!southWall) {
			segment.Rotate (Vector3.up, 90);
		} else if (!northWall) {
			segment.Rotate (Vector3.up, -90);
		}

		segment.position = position;
		segment.gameObject.AddComponent<CorridorTile> ();        
		return segment.gameObject;
	}

    public GameObject PlaceCorridor(bool northWall, bool westWall, bool southWall, bool eastWall)
    {
        return PlaceCorridor(transform.position, northWall, westWall, southWall, eastWall, false, false);
    }

    public GameObject PlaceCorridor(List<Action> actions, bool northWall, bool westWall, bool southWall, bool eastWall)
    {
        return PlaceCorridor(transform.position, northWall, westWall, southWall, eastWall, actions.Any(a => a.action == "rotate" || a.action == "lookat"), actions.Any(a => a.action == "teleport"));
    }

    public GameObject PlaceCorridor(Vector3 position, bool northWall, bool westWall, bool southWall, bool eastWall, bool hasSpin, bool hasTeleport)
    {
        Transform segment; 

        int walls = (northWall ? 1 : 0) + (southWall ? 1 : 0) + (westWall ? 1 : 0) + (eastWall ? 1 : 0);

        if (walls > 3)
        {
            return null;
        } else if (walls == 0)
        {
            segment = Instantiate(corridorPrefabs[4]);
        }
        else if (walls == 3)
        {
            segment = Instantiate(corridorPrefabs[3]);
            if (!westWall)
            {
                segment.Rotate(Vector3.up, 180);
            }
            else if (!southWall)
            {
                segment.Rotate(Vector3.up, 90);
            }
            else if (!northWall)
            {
                segment.Rotate(Vector3.up, -90);
            }
        }
        else if (walls == 1)
        {
            segment = Instantiate(corridorPrefabs[0]);
            if (eastWall)
            {
                segment.Rotate(Vector3.up, 180);
            }
            else if (northWall)
            {
                segment.Rotate(Vector3.up, 90);
            }
            else if (southWall)
            {
                segment.Rotate(Vector3.up, -90);
            }
        }
        else
        {
            if (northWall == southWall || westWall == eastWall)
            {
                //Straight
                segment = Instantiate(corridorPrefabs[1]);

                if (eastWall)
                {
                    segment.Rotate(Vector3.up, -90);
                }

            }
            else
            {
                //Rotating segment
                segment = Instantiate(corridorPrefabs[2]);

                if (eastWall)
                {
                    if (southWall)
                    {
                        segment.Rotate(Vector3.up, -90);
                    }
                    else
                    {
                        segment.Rotate(Vector3.up, 180);
                    }
                }
                else
                {
                    if (northWall)
                    {
                        segment.Rotate(Vector3.up, 90);

                    }
                    else
                    {
                        //segment.Rotate(Vector3.up, 180);

                    }
                }
            }
                
        }

        segment.position = position;
        segment.gameObject.AddComponent<CorridorTile>();
        if (hasTeleport)
        {
            foreach(Light light in segment.GetComponentsInChildren<Light>())
            {
                light.gameObject.AddComponent<CorridorLightFlicker>();
            }
        }
        if (hasSpin)
        {
            bool foundCarpet = false;
            string searchStr = carpetMat.name;
            
            foreach(MeshRenderer mRend in segment.GetComponentsInChildren<MeshRenderer>())
            {
                for (int i=0, l=mRend.materials.Length; i< l; i++)
                {                    
                    if (mRend.materials[i].name.StartsWith(searchStr))
                    {
                        Material[] mats = mRend.materials;
                        mats[i] = spinMat;
                        mRend.materials = mats;
                        foundCarpet = true;
                        break;
                    }
                }
                if (foundCarpet)
                {
                    break;
                }
            }
        }
        return segment.gameObject;
    }
}
