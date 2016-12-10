using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    Camera cam;

    [SerializeField]
    LayerMask doorLayer;

    [SerializeField]
    PlayerWalkController playerCtrl;

    void Start()
    {
        cam = GetComponentInParent<Camera>();
        playerCtrl = GetComponentInParent<PlayerWalkController>();
    }

	void Update () {
		if (Input.GetButtonDown("interact"))
        {

            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 10, doorLayer))
            {
                OneRoomDoor door = hit.transform.GetComponent<OneRoomDoor>();
                if (door && playerCtrl.CurrentTile.HasDoor(door))
                {
                    door.OpenDoor(playerCtrl.LookDirection);
                }
            }
        }
	}
}
