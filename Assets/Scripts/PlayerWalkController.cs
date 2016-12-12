using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour {

    [SerializeField]
    Sounder walkSounds;

    [SerializeField]
    Sounder walkRefuse;

    [SerializeField]
    Sounder trollySounds;

	[SerializeField]
	ImportRoom importRoom;

    [SerializeField]
    Trolly trolly;

    [SerializeField]
    Transform movementTransform;

    [SerializeField]
    Transform lookTransform;

    [SerializeField]
    CorridorTile currentTile;

	[SerializeField]
	CorridorTile endTile;

    [SerializeField]
    Direction facingDirection;

    [SerializeField]
    AnimationCurve walkAnim;

    [SerializeField]
    AnimationCurve walkRefuseAnim;

    [SerializeField]
    AnimationCurve rotateAnim;

    [SerializeField, Range(0, 3)]
    float walkDuration = 1;

    [SerializeField, Range(0, 3)]
    float refuseDuration = 0.4f;

    [SerializeField, Range(0, 3)]
    float rotationDuration = 1f;

    [SerializeField, Range(0, 3)]
    float lookDuration = 1f;

    [SerializeField]
    Animator camAnim;

	[SerializeField]
	Camera myCamera;

	[SerializeField]
	public int currentLevel;

	[SerializeField]
	int maxLevel = 2;

    private bool _frozen = false;   
    public bool frozen
    {
        get
        {
            return _frozen;
        }

        set
        {
            _frozen = value;
            Cursor.visible = !value;
        }
    }

    public CorridorTile CurrentTile
    {
        get
        {
            return currentTile;
        }
    }

    public void SetCurrentTile(CorridorTile tile)
    {
        currentTile = tile;
        movementTransform.position = currentTile.playerPosition;
        tile.SoftMangageDoorRooms();
        trolly.UpdateDirection();
		var mainShader = myCamera.GetComponent<MainShader> ();
		mainShader.hasGlitch = tile.hasGlitch;
    }

	public void SetEndTile(CorridorTile tile) {
		endTile = tile;
	}

    public Direction LookDirection
    {
        get
        {
            return facingDirection;
        }
    }

    public void EnterElevator()
    {
        Debug.Log("Trigger enter cam");
        camAnim.SetTrigger("Elevator");
        //camAnim.ResetTrigger("Elevator");
    }

    public void SetCurrentDirection(Direction direction)
    {
        lookTransform.rotation = Quaternion.LookRotation(CorridorTile.GetLookDirection(direction), Vector3.up);
        facingDirection = direction;
        trolly.UpdateDirection();
    }

    float animSpeed = 0.01f;


    bool transitioning = false;

    PlayerInventory inventory;

    public PlayerInventory Inventory
    {
        get
        {
            return inventory;
        }
    }


    int cullingMask;
    CameraClearFlags clearMode;

    public void WhiteOut()
    {
        cullingMask = myCamera.cullingMask;
        myCamera.cullingMask = 0;
        clearMode = myCamera.clearFlags;
        myCamera.clearFlags = CameraClearFlags.Nothing;
    }

    public void ResumePlay()
    {
        frozen = false;
        myCamera.cullingMask = cullingMask;
        myCamera.clearFlags = clearMode;
    }

    public void StartNextLevel()
    {
        StartNextLevel(0);
    }

    public void StartNextLevel(float delay) {
		if (currentLevel < maxLevel) {
			currentLevel++;
			importRoom.createRooms (currentLevel);
            if (delay > 0)
            {
                StartCoroutine(_delayNextLevel(delay));
            }
            else
            {
                ResumePlay();
            }
		}
	}

    IEnumerator<WaitForSeconds> _delayNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumePlay();
    }

    void Start()
    {
        inventory = GetComponentInParent<PlayerInventory>();
    }

    bool isLookingIntoRoom = false;
    public bool IsLookingIntoRoom {
        get {
            return isLookingIntoRoom;
        }
    }

    public void LookIntoOneRoom()
    {
        isLookingIntoRoom = true;
        camAnim.SetBool("Looking", true);
    }

    public void StopLookingIntoOneRoom()
    {
        isLookingIntoRoom = false;
        camAnim.SetBool("Looking", false);
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!transitioning && !frozen)
        {
            if (Input.GetButton("walkForward"))
            {

                CorridorTile target = currentTile.GetEdge(facingDirection);
				if (target == null || currentTile == endTile)
                {
                    OneRoomDoor door = currentTile.GetDoor(facingDirection);
                    if (IsLookingIntoRoom || door == null)
                    {
                        StartCoroutine(RefuseWalk());
                    } else
                    {
                        StartCoroutine(WalkOpenDoor(door, facingDirection));
                    }
                }
                else
                {
                    StartCoroutine(Walk(target));
                }
            }
            else if (Input.GetButton("walkReverse"))
            {
                if (isLookingIntoRoom)
                {
                    StartCoroutine(WalkOutOfRoom());
                }
                else {

                    CorridorTile target = currentTile.GetEdge(CorridorTile.GetInverseDirection(facingDirection));
                    if (target == null || currentTile == endTile)
                    {
                        StartCoroutine(RefuseWalk());
                    }
                    else
                    {
                        StartCoroutine(Walk(target));
                    }
                }
            }
            else if (Input.GetButton("rotateLeft"))
            {
                StartCoroutine(Rotate(false));
            }
            else if (Input.GetButton("rotateRight"))
            {
                StartCoroutine(Rotate(true));
            }
        }
	}
   
    IEnumerator<WaitForSeconds> WalkOutOfRoom()
    {
        transitioning = true;
        StopLookingIntoOneRoom();
        yield return new WaitForSeconds(lookDuration);
        transitioning = false;
    }


    IEnumerator<WaitForSeconds> WalkOpenDoor(OneRoomDoor door, Direction direction)
    {
        transitioning = true;
        if (!door.IsOpen)
        {
            door.OpenDoor(facingDirection);

        }
        LookIntoOneRoom();
        yield return new WaitForSeconds(lookDuration);
        transitioning = false;
    }

    IEnumerator<WaitForSeconds> RefuseWalk()
    {
        if (isLookingIntoRoom)
        {
            StopLookingIntoOneRoom();
        }
        transitioning = true;
        walkRefuse.PlayOne();
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Vector3 startPos = currentTile.playerPosition;
        Vector3 targetPos = startPos + CorridorTile.GetLookDirection(facingDirection);
        while (progress < 1)
        {
            
            movementTransform.position = Vector3.Lerp(startPos, targetPos, walkRefuseAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / refuseDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        movementTransform.position = currentTile.playerPosition;

        transitioning = false;
    }

    IEnumerator<WaitForSeconds> Walk(CorridorTile target)
    {
        if (isLookingIntoRoom)
        {
            StopLookingIntoOneRoom();
        }
        walkSounds.PlayInSequence(2);
        trollySounds.PlayOne();
        transitioning = true;
        //currentTile.CloseAllDoors();
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Vector3 startPos = currentTile.playerPosition;
        Vector3 endPos = target.playerPosition;
        importRoom.ManageDoors(target, facingDirection);
        while (progress < 1)
        {
            
            movementTransform.position = Vector3.Lerp(startPos, endPos, walkAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / walkDuration;
            yield return new WaitForSeconds(animSpeed);
        }

		Direction from = target.GetPreviousDirection (currentTile);
		//Debug.Log (from);
        SetCurrentTile(target);                
		foreach (Action action in currentTile.actions) {
			if (action.IsActive (from)) {
				switch (action.action) {
				case "teleport":				
					SetCurrentTile (action.tile);
					break;
				case "rotate":
                        Direction targetDirection = (Direction)(((int)facingDirection + action.GetInteger(0)) % 4);
                        importRoom.ManageDoors(currentTile, targetDirection);
                        SetCurrentDirection (targetDirection);
					break;
				case "lookat":
                        targetDirection = action.GetDirection(0);
                        importRoom.ManageDoors(currentTile, targetDirection);
                        SetCurrentDirection (targetDirection);
					break;
				default:
					break;
				}
			}
		}
        transitioning = false;
    }

    IEnumerator<WaitForSeconds> Rotate(bool rotateRight)
    {
        walkSounds.PlayInSequence(2);
        transitioning = true;      
        if (isLookingIntoRoom)
        {
            StopLookingIntoOneRoom();
        }
        /*
        if (currentTile.CloseAllDoors())
        {
            StopLookingIntoOneRoom();
        }*/
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Direction targetDirection;
        CorridorTile.GetRotation(facingDirection, rotateRight, out targetDirection);
        importRoom.ManageDoors(currentTile, targetDirection);
        Quaternion startRotation = lookTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(CorridorTile.GetLookDirection(targetDirection), Vector3.up);
        while (progress < 1)
        {
            lookTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotateAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / rotationDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        SetCurrentDirection(targetDirection);        
        transitioning = false;
    }

    void OnDrawGizmos()
    {
        if (currentTile == null)
        {
            return;
        }

        Ray r = new Ray(currentTile.playerPosition, CorridorTile.GetLookDirection(LookDirection));
        Gizmos.DrawLine(r.origin, r.GetPoint(3));
    }

}
