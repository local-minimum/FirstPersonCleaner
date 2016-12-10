using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneRoomDoor : MonoBehaviour {

    [SerializeField]
    GameObject roomPrefab;

    MeshRenderer mRend;
    Collider col;

    GameObject room;
    CorridorTile tile;
    
    void Start()
    {
        tile = GetComponentInParent<CorridorTile>();
        mRend = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    public void OpenDoor(Direction direction)
    {
        if (room == null)
        {
            SpawnRoom(direction);
        }

        room.SetActive(true);
        mRend.enabled = false;
        col.enabled = false;
    }

    public void CloseDoor()
    {
        if (room)
        {
            room.SetActive(false);
        }
        mRend.enabled = true;
        col.enabled = true;
    }

    void SpawnRoom(Direction direction)
    {
        room = Instantiate(roomPrefab);
        room.transform.SetParent(transform);
        room.transform.Rotate(Vector3.up, (int)direction * 90);
        room.transform.position = tile.transform.position + CorridorTile.GetLookDirection(direction) * 2.5f;
    }
}
