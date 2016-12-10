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

    LayerMask activeLayer;

    Quaternion startRotation;
    Quaternion rotationTarget;
    Quaternion restingRotation;

    bool rotating = false;
    float rotationStart;
    bool isOpen = false;
    void Start()
    {
        restingRotation = transform.rotation;
        startRotation = restingRotation;
        tile = GetComponentInParent<CorridorTile>();
        mRend = GetComponentInChildren<MeshRenderer>();
        col = GetComponentInChildren<Collider>();
    }

    public void OpenDoor(Direction direction)
    {
        if (isOpen)
        {
            return;
        }
        if (room == null)
        {
            SpawnRoom(direction);
        }
        isOpen = true;
        activeLayer = gameObject.layer;
        gameObject.layer = 0;
        room.SetActive(true);        
        col.enabled = false;
        startRotation = restingRotation;
        rotationTarget = restingRotation * Quaternion.AngleAxis(90, Vector3.up);
        rotationStart = Time.timeSinceLevelLoad;
        rotating = true;
    }

    public void CloseDoor()
    {
        if (!isOpen)
        {
            return;
        }
        if (room)
        {
            room.SetActive(false);
        }
        isOpen = false;
        startRotation = transform.localRotation;
        rotationTarget = restingRotation;
        col.enabled = true;
        gameObject.layer = activeLayer;
        rotationStart = Time.timeSinceLevelLoad;
        rotating = true;

    }

    void SpawnRoom(Direction direction)
    {
        room = Instantiate(roomPrefab);
        room.transform.SetParent(transform.parent);
        room.transform.Rotate(Vector3.up, (int)direction * 90);
        Direction ortho;
        CorridorTile.GetRotation(direction, true, out ortho);
        room.transform.position = tile.transform.position + CorridorTile.GetLookDirection(direction) * 2.5f + CorridorTile.GetLookDirection(ortho) * 0.4f;
    }

    void Update()
    {
        if (rotating)
        {

            float progress = (Time.timeSinceLevelLoad - rotationStart) / 0.6f;
            if (progress > 1)
            {
                transform.localRotation = rotationTarget;
                rotating = false;
            } else
            {
                transform.localRotation = Quaternion.Lerp(startRotation, rotationTarget, progress);

            }
        }
    }
}
