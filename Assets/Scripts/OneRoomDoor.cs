using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneRoomDoor : MonoBehaviour {

    [SerializeField]
    GameObject roomPrefab;

    [SerializeField]
    Sounder soundOpen;

    [SerializeField]
    Sounder soundClose;

    [SerializeField]
    Sounder soundCreak;

    //Collider col;

    GameObject room;
    CorridorTile tile;

    LayerMask activeLayer;

    Quaternion rotationStart;
    Quaternion rotationTarget;
    Quaternion restingRotation;

    [SerializeField]
    Diorama diorama;

    bool rotating = false;
    float startOfRotationTime;
    bool isOpen = false;

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
    }
    
    Direction CalculateDirection()
    {

        Vector3 offset = (transform.position - tile.playerPosition).normalized;
        float OffDotNorth = Vector3.Dot(offset, Vector3.forward);
        float OffDotEast = Vector3.Dot(offset, Vector3.right);

        if (Mathf.Abs(OffDotNorth) > Mathf.Abs(OffDotEast))
        {
            if (OffDotNorth > 0)
            {
                return Direction.North;
            } else
            {
                return Direction.South;
            }
        } else
        {
            if (OffDotEast > 0)
            {
                return Direction.East;
            } else
            {
                return Direction.West;
            }
        }
    }

    Direction _direction;

    public Direction direction
    {
        get
        {
            return _direction;
        }
    }

    public void SetupDoor()
    {
        restingRotation = transform.rotation;
        tile = GetComponentInParent<CorridorTile>();
        _direction = CalculateDirection();
        Room room = tile.RoomData;
        name = string.Format("Door {0}:{1}, {2}, {3}", room.level, room.row, room.col, _direction);
        diorama.SetDoorName(name);
    }

    public bool OpenDoor()
    {

        if (isOpen)
        {
            return false;
        }

        if (diorama != null)
        {
            //TODO: Activate
        }
        else {
            if (room == null)
            {
                SpawnRoom();
            }
            room.SetActive(true);
        }
        soundOpen.PlayOne();
        soundCreak.ProbabilityPlayOne();
        isOpen = true;
        activeLayer = gameObject.layer;
        gameObject.layer = 0;      
        //col.enabled = false;
        rotationStart = Quaternion.AngleAxis((int)CorridorTile.GetRighDirection(_direction) * 90, Vector3.up);
        rotationTarget = Quaternion.AngleAxis((int)CorridorTile.GetInverseDirection(_direction) * 90, Vector3.up);
        startOfRotationTime = Time.timeSinceLevelLoad;
        rotating = true;
        return true;
    }

    public bool CloseDoor()
    {
        if (!isOpen)
        {
            return false;
        }
        Debug.Log("Hard close");
        soundCreak.ProbabilityPlayOne();
        soundClose.PlayOne();
        isOpen = false;
        rotationStart = rotationTarget;
        rotationTarget = restingRotation;
        //col.enabled = true;
        gameObject.layer = activeLayer;
        startOfRotationTime = Time.timeSinceLevelLoad;
        rotating = true;
        if (diorama)
        {

        }
        else {
            StartCoroutine(delayDisableRoom());
        }
        return true;
    }

    public void SoftCloseRoom() {
        if (room)
        {
            room.SetActive(false);
        }
    }

    public void SoftOpenRoom()
    {
        if (room)
        {
            room.SetActive(true);
        }
    }

    IEnumerator<WaitForSeconds> delayDisableRoom()
    {
        yield return new WaitForSeconds(2f);
        if (!isOpen)
        {
            if (room)
            {
                room.SetActive(false);
            }
        }
    }

    void SpawnRoom()
    {
        room = Instantiate(roomPrefab);
        room.transform.Rotate(Vector3.up, (int)_direction * 90);
        Direction ortho;
        CorridorTile.GetRotation(_direction, true, out ortho);
        room.transform.position = tile.transform.position + CorridorTile.GetLookDirection(_direction) * 2.65f + CorridorTile.GetLookDirection(ortho) * 0.4f + Vector3.down * 0.5f;
        room.transform.SetParent(transform.parent);
    }

    void Update()
    {
        if (rotating)
        {

            float progress = (Time.timeSinceLevelLoad - startOfRotationTime) / 0.6f;
            if (progress > 1)
            {
                transform.rotation = rotationTarget;
                rotating = false;
            } else
            {
                transform.rotation = Quaternion.Lerp(rotationStart, rotationTarget, progress);

            }
        }
    }
}
