using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseInteractionTypes { None, Elevator, Door, Room, WorkOrder };

public delegate void MouseClick(MouseInteractionTypes interaction, bool refused, bool returnToNativeState);

public class MouseController : MonoBehaviour {

    public event MouseClick OnMouseClickEvent;

    public static LayerMask DoorLayer;

    Camera cam;

    public Camera Cam
    {
        get
        {
            if (cam == null)
            {
                cam = GetComponentInParent<Camera>();
            }
            return cam;
        }
    }

    [SerializeField]
    Sounder towelSounderDown;

    [SerializeField]
    Sounder towelSounderUp;

    [SerializeField]
    Sounder dndSounder;

    [SerializeField]
    Sounder wetSounderDown;

    [SerializeField]
    Sounder wetSounderUp;

    [SerializeField]
    Texture2D defaultCur;

    [SerializeField]
    Vector2 defaultCurHotspot = new Vector2(0.5f, 0.5f);

    [SerializeField]
    Texture2D cardCur;

    [SerializeField]
    Vector2 cardCurHotspot = new Vector2(0.75f, 0.75f);

    [SerializeField]
    Texture2D handCur;

    [SerializeField]
    Vector2 handCurHotspot = new Vector2(0.3f, 0.3f);

    [SerializeField]
    LayerMask doorLayer;

    [SerializeField]
    LayerMask roomLayer;

    [SerializeField]
    WorkInstructions workInstructions;

    [SerializeField]
    LayerMask workInstructionsLayer;

    [SerializeField]
    LayerMask elevatorInteractions;

    PlayerWalkController playerCtrl;

    void Start()
    {
        DoorLayer = doorLayer;
        if (cam == null)
        {
            cam = Cam;
        }
        
        playerCtrl = GetComponentInParent<PlayerWalkController>();
    }

    MouseInteractionTypes interaction = MouseInteractionTypes.None;

    void Update()
    {
        if (playerCtrl.frozen)
        {
            return;
        }

        Ray r;
        RaycastHit hit;

        MouseInteractionTypes newInteraction = GetMouseHover(out r, out hit);
        if (newInteraction != interaction)
        {
            if (newInteraction == MouseInteractionTypes.None) {
                Cursor.SetCursor(defaultCur, defaultCurHotspot, CursorMode.Auto);
            } else if (newInteraction == MouseInteractionTypes.Door)
            {
                Cursor.SetCursor(cardCur, cardCurHotspot, CursorMode.Auto);
            } else
            {
                Cursor.SetCursor(handCur, handCurHotspot, CursorMode.Auto);
            }

            interaction = newInteraction;
        }

        if (Input.GetButtonDown("interact"))
        {

            if (interaction == MouseInteractionTypes.Elevator)
            {
                Elevator elevator = hit.transform.GetComponentInParent<Elevator>();
                if (elevator)
                {
                    elevator.PressForElevator(playerCtrl);
                    if (OnMouseClickEvent != null)
                    {
                        OnMouseClickEvent(MouseInteractionTypes.Elevator, false, false);
                    }
                }
            }
            else if (interaction == MouseInteractionTypes.Door)
            {                
                OneRoomDoor door = hit.transform.GetComponentInParent<OneRoomDoor>();                
                if (door && playerCtrl.CurrentTile.HasDoor(door))
                {
                                    
                    OneRoomDoor lookDirDoor = playerCtrl.CurrentTile.GetDoor(playerCtrl.LookDirection);
                    if (lookDirDoor == door && !door.IsOpen)
                    {
                        if (playerCtrl.IsLookingIntoRoom)
                        {
                            door.CloseDoor();
                            playerCtrl.StopLookingIntoOneRoom();
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Door, false, true);
                            }
                        }
                        else {
                            door.OpenDoor(playerCtrl.LookDirection);
                            playerCtrl.LookIntoOneRoom();
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Door, false, false);
                            }
                        }
                    } else
                    {
                        door.CloseDoor();
                        if (OnMouseClickEvent != null)
                        {
                            OnMouseClickEvent(MouseInteractionTypes.Door, false, true);
                        }
                    }
                }
            }
            else if (interaction == MouseInteractionTypes.Room)
            {
                Transform target = null;

                if (hit.transform.gameObject.tag == "TV")
                {
                    TVNoise tv = hit.transform.gameObject.GetComponent<TVNoise>();
                    if (tv)
                    {
                        tv.ToggleTV();
                        if (OnMouseClickEvent != null)
                        {
                            OnMouseClickEvent(MouseInteractionTypes.Room, false, tv.TvIsOn);
                        }
                    }                    
                }
                else if (hit.transform.gameObject.tag == "Cupboard")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Cupboard", out target))
                    {
                        playerCtrl.Inventory.ReturnDND(target.GetChild(0).gameObject);
                        dndSounder.PlayOne();
                        if (OnMouseClickEvent != null)
                        {
                            OnMouseClickEvent(MouseInteractionTypes.Room, false, true);
                        }
                    }
                    else if (target)
                    {
                        GameObject dnd = playerCtrl.Inventory.GetDND();
                        if (dnd)
                        {
                            dnd.transform.SetParent(target);
                            dnd.transform.rotation = target.rotation;
                            dnd.transform.position = target.position;
                            dndSounder.PlayOne();
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, false, false);
                            }
                        } else
                        {
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, true, false);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.tag == "Floor")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Floor", out target))
                    {
                        wetSounderUp.PlayOne();
                        playerCtrl.Inventory.ReturnWetFloor(target.GetChild(0).gameObject);
                        if (OnMouseClickEvent != null)
                        {
                            OnMouseClickEvent(MouseInteractionTypes.Room, false, true);
                        }
                    }
                    else if (target)
                    {

                        GameObject wetFloor = playerCtrl.Inventory.GetWetFloor();
                        if (wetFloor)
                        {
                            wetFloor.transform.SetParent(target);
                            wetFloor.transform.rotation = target.rotation;
                            wetFloor.transform.position = target.position;
                            wetSounderDown.PlayOne();
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, false, false);
                            }
                        } else
                        {
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, true, false);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.tag == "Bed")
                {

                    if (RoomInteractionTargetOccupied(hit.transform, "Bed", out target))
                    {
                        playerCtrl.Inventory.ReturnTowel(target.GetChild(0).gameObject);
                        towelSounderUp.PlayOne();
                        if (OnMouseClickEvent != null)
                        {
                            OnMouseClickEvent(MouseInteractionTypes.Room, false, true);
                        }
                    }
                    else if (target)
                    {
                        GameObject towel = playerCtrl.Inventory.GetTowel();
                        if (towel)
                        {
                            towel.transform.SetParent(target);
                            towel.transform.rotation = target.rotation;
                            towel.transform.position = target.position;
                            towelSounderDown.PlayOne();
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, false, false);
                            }
                        } else
                        {
                            if (OnMouseClickEvent != null)
                            {
                                OnMouseClickEvent(MouseInteractionTypes.Room, true, false);
                            }
                        }
                    }
                }
            }
            else if (interaction == MouseInteractionTypes.WorkOrder)
            {
                workInstructions.Toggle();
                if (OnMouseClickEvent != null)
                {
                    OnMouseClickEvent(MouseInteractionTypes.WorkOrder, false, !workInstructions.IsShowing);
                }
            } else
            {
                if (OnMouseClickEvent != null)
                {
                    OnMouseClickEvent(MouseInteractionTypes.None, true, false);
                }
            }
        }
    }

    public Ray MouseRay
    {
        get
        {
            return cam.ScreenPointToRay(Input.mousePosition);
        }
    }

    MouseInteractionTypes GetMouseHover(out Ray ray, out RaycastHit hit)
    {
        ray = MouseRay;
        if (Physics.Raycast(ray, out hit, 3, elevatorInteractions))
        {
            return MouseInteractionTypes.Elevator;
        }
        else if (Physics.Raycast(ray, out hit, 3, doorLayer))
        {
            return MouseInteractionTypes.Door;
        }
        else if (Physics.Raycast(ray, out hit, 10, roomLayer))
        {
            return MouseInteractionTypes.Room;
        }
        else if (Physics.Raycast(ray, out hit, 10, workInstructionsLayer))
        {
            return MouseInteractionTypes.WorkOrder;
        } else
        {
            return MouseInteractionTypes.None;
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
