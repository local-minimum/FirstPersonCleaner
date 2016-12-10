using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneRoomDoor : MonoBehaviour {

    MeshRenderer mRend;
    Collider col;

    void Start()
    {
        mRend = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    public void OpenDoor()
    {
        Debug.Log("Open");
        mRend.enabled = false;
        col.enabled = false;
    }

    public void CloseDoor()
    {
        mRend.enabled = true;
        col.enabled = true;
    }

}
