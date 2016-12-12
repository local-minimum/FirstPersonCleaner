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

    void Start()
    {
        restingRotation = transform.rotation;
        tile = GetComponentInParent<CorridorTile>();
        //col = GetComponentInChildren<Collider>();
    }

    public bool OpenDoor(Direction direction)
    {
        if (isOpen)
        {
            return false;
        }
        if (room == null)
        {
            SpawnRoom(direction);
        }
        soundOpen.PlayOne();
        soundCreak.ProbabilityPlayOne();
        isOpen = true;
        activeLayer = gameObject.layer;
        gameObject.layer = 0;
        room.SetActive(true);
        //col.enabled = false;
        rotationStart = Quaternion.AngleAxis((int)CorridorTile.GetRighDirection(direction) * 90, Vector3.up);
        rotationTarget = Quaternion.AngleAxis((int)CorridorTile.GetInverseDirection(direction) * 90, Vector3.up);
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
        soundClose.PlayOne();
        soundCreak.ProbabilityPlayOne();
        isOpen = false;
        rotationStart = rotationTarget;
        rotationTarget = restingRotation;
        //col.enabled = true;
        gameObject.layer = activeLayer;
        startOfRotationTime = Time.timeSinceLevelLoad;
        rotating = true;
        StartCoroutine(delayDisableRoom());
        return true;
    }

    public void SoftCloseRoom() {

        room.SetActive(false);
    }

    public void SoftOpenRoom()
    {
        room.SetActive(true);
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

    void SpawnRoom(Direction direction)
    {
        room = Instantiate(roomPrefab);
        room.transform.Rotate(Vector3.up, (int)direction * 90);
        Direction ortho;
        CorridorTile.GetRotation(direction, true, out ortho);
        room.transform.position = tile.transform.position + CorridorTile.GetLookDirection(direction) * 2.65f + CorridorTile.GetLookDirection(ortho) * 0.4f + Vector3.down * 0.5f;
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
