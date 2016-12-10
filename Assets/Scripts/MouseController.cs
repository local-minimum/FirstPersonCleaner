using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    Camera cam;

    [SerializeField]
    LayerMask doorLayer;

    [SerializeField]
    LayerMask roomLayer;

    [SerializeField]
    WorkInstructions workInstructions;

    [SerializeField]
    LayerMask workInstructionsLayer;

    bool lookingAtInstructions = false;

    PlayerWalkController playerCtrl;

    void Start()
    {
        cam = GetComponentInParent<Camera>();
        playerCtrl = GetComponentInParent<PlayerWalkController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("interact"))
        {

            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 4, doorLayer))
            {                
                OneRoomDoor door = hit.transform.GetComponentInParent<OneRoomDoor>();                
                if (door && playerCtrl.CurrentTile.HasDoor(door))
                {
                    door.OpenDoor(playerCtrl.LookDirection);
                }
            }
            else if (Physics.Raycast(r, out hit, 10, roomLayer))
            {
                Transform target = null;

                if (hit.transform.gameObject.tag == "TV")
                {
                    TVNoise tv = hit.transform.gameObject.GetComponent<TVNoise>();
                    if (tv.enabled)
                    {
                        tv.TurnOffTV();
                    }
                    //TURN OFF IF POSSIBLE
                }
                else if (hit.transform.gameObject.tag == "Cupboard")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Cupboard", out target))
                    {
                        playerCtrl.Inventory.ReturnDND(target.GetChild(0).gameObject);
                    }
                    else if (target)
                    {
                        GameObject dnd = playerCtrl.Inventory.GetDND();
                        if (dnd)
                        {
                            dnd.transform.SetParent(target);
                            dnd.transform.rotation = target.rotation;
                            dnd.transform.position = target.position;
                        }
                    }
                }
                else if (hit.transform.gameObject.tag == "Floor")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Floor", out target))
                    {

                        playerCtrl.Inventory.ReturnWetFloor(target.GetChild(0).gameObject);
                    }
                    else if (target)
                    {

                        GameObject wetFloor = playerCtrl.Inventory.GETWetFloor();
                        if (wetFloor)
                        {
                            wetFloor.transform.SetParent(target);
                            wetFloor.transform.rotation = target.rotation;
                            wetFloor.transform.position = target.position;
                        }
                    }
                }
                else if (hit.transform.gameObject.tag == "Bed")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Bed", out target))
                    {
                        playerCtrl.Inventory.ReturnTowel(target.GetChild(0).gameObject);
                    }
                    else if (target)
                    {
                        GameObject towel = playerCtrl.Inventory.GetTowel();
                        if (towel)
                        {
                            towel.transform.SetParent(target);
                            towel.transform.rotation = target.rotation;
                            towel.transform.position = target.position;
                        }
                    }
                }
            }
            else if (Physics.Raycast(r, out hit, 10, workInstructionsLayer))
            {
                workInstructions.PickUp();
                lookingAtInstructions = true;
            }
        } else if (Input.GetButtonUp("interact") && lookingAtInstructions)
        {
            workInstructions.PutDown();
        }
    }

    static bool RoomInteractionTargetOccupied(Transform parent, string tag, out Transform child)
    {
        child = null;
        for (int i = 0, l = parent.childCount; i < l; i++)
        {            
            child = parent.GetChild(i);
            if (child.tag == tag)
            {
                break;
            } else
            {
                child = null;
            }
        }
        return child != null && child.tag == tag && child.childCount == 1;
    }
}
