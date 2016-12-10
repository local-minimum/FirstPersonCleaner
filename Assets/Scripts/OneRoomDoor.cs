using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneRoomDoor : MonoBehaviour {

    MeshRenderer mRend;

    void Start()
    {
        mRend = GetComponent<MeshRenderer>();
    }

    public void OpenDoor()
    {
        mRend.enabled = false;
    }

    public void CloseDoor()
    {
        mRend.enabled = true;
    }

    public void ToggleOpenClose()
    {
        mRend.enabled = !mRend.enabled;
    }
}
