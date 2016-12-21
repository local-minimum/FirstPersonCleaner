using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public enum WalkInstruction { Forward, Reverse, RotateLeft, RotateRight};

public delegate void WalkEvent(WalkInstruction instruction, bool refused, bool wasFacingDoor, bool isLookingIntoDoor);

public enum MazeEventTypes {Teleport, Rotate, LookAt};

public delegate void MazeEvent(MazeEventTypes eventType, CorridorTile causeTile);

public class PlayerWalkController : MonoBehaviour {

    static PlayerWalkController _playerCTRL;

    public static PlayerWalkController PlayerCTRL
    {
        get
        {
            if (_playerCTRL == null)
            {
                _playerCTRL = FindObjectOfType<PlayerWalkController>();
            }
            return _playerCTRL;
        }
    }

    public event WalkEvent OnWalk;
    public event MazeEvent OnMazeEvent;

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

    CorridorTile currentTile;

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

    public Camera PlayerCam
    {
        get
        {
            return myCamera;
        }
    }

    PlayerStatsCollector _stats;

    public PlayerStatsCollector PlayerStats
    {
        get
        {
            if (_stats == null)
            {
                _stats = GetComponentInChildren<PlayerStatsCollector>();
            }
            return _stats;
        }
    }

    MouseController _mouseCtrl;

    public MouseController MouseCtrl
    {
        get
        {
            if (_mouseCtrl == null)
            {
                _mouseCtrl = GetComponentInChildren<MouseController>();
            }
            return _mouseCtrl;
        }
    }

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

    MainShader glitchShader;

    public void SetCurrentTile(CorridorTile tile)
    {
        currentTile = tile;
        movementTransform.position = currentTile.playerPosition;
        tile.SoftMangageDoorRooms();
        trolly.UpdateDirection(true);
        if (glitchShader)
        {
            if (tile.hasGlitch)
            {
                glitchShader.IncreaseFrequency();
            }
            else
            {
                glitchShader.DecreaseFrequency();
            }
        }
    }

	public void SetEndTile(CorridorTile tile) {
		//endTile = tile;
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

    public void SetCurrentDirection(Direction direction, bool animateTrolley)
    {
        lookTransform.rotation = Quaternion.LookRotation(CorridorTile.GetLookDirection(direction), Vector3.up);
        facingDirection = direction;
        trolly.UpdateDirection(animateTrolley);
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

    [SerializeField]
    Animator[] theEndTriggers;

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
		} else
        {
            glitchShader.SetMaxGlitchFrequency();
            for (int i = 0; i < theEndTriggers.Length; i++)
            {
                theEndTriggers[i].SetTrigger("TheEnd");
                Debug.Log("Started end on " + theEndTriggers[i].name);
            }
            myCamera.cullingMask = cullingMask;
            myCamera.clearFlags = clearMode;
            camAnim.Stop();
            var dof = myCamera.GetComponent<DepthOfField>();
            dof.focalLength = 0.1f;
        }
    }

    IEnumerator<WaitForSeconds> _delayNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumePlay();
    }

    void Start()
    {
        _playerCTRL = this;
        inventory = GetComponentInParent<PlayerInventory>();
        glitchShader = myCamera.GetComponent<MainShader>();
    }

    void OnDestory()
    {
        _playerCTRL = null;
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
				if (target == null)
                {
                    OneRoomDoor door = currentTile.GetDoor(facingDirection);
                    if (IsLookingIntoRoom || door == null)
                    {
                        if (OnWalk != null)
                        {
                            OnWalk(WalkInstruction.Forward, true, true, IsLookingIntoRoom);
                        }
                        StartCoroutine(RefuseWalk());
                    } else
                    {
                        if (OnWalk != null)
                        {
                            OnWalk(WalkInstruction.Forward, false, true, false);
                        }
                        StartCoroutine(WalkOpenDoor(door, facingDirection));
                    }
                }
                else
                {
                    if (OnWalk != null)
                    {
                        OnWalk(WalkInstruction.Forward, false, false, false);
                    }
                    StartCoroutine(Walk(target));
                    trolly.WalkForward();
                }
            }
            else if (Input.GetButton("walkReverse"))
            {
                if (isLookingIntoRoom)
                {
                    if (OnWalk != null)
                    {
                        OnWalk(WalkInstruction.Reverse, false, true, true);
                    }
                    StartCoroutine(WalkOutOfRoom());
                }
                else {

                    CorridorTile target = currentTile.GetEdge(CorridorTile.GetInverseDirection(facingDirection));
                    if (target == null)
                    {
                        if (OnWalk != null)
                        {
                            OnWalk(WalkInstruction.Reverse, true, currentTile.GetEdge(facingDirection) == null, false);
                        }

                        StartCoroutine(RefuseWalk());
                    }
                    else
                    {
                        if (OnWalk != null)
                        {
                            OnWalk(WalkInstruction.Reverse, false, currentTile.GetEdge(facingDirection) == null, false);
                        }

                        StartCoroutine(Walk(target));
                    }
                }
            }
            else if (Input.GetButton("rotateLeft"))
            {
                if (OnWalk != null)
                {
                    OnWalk(WalkInstruction.RotateLeft, false, true, true);
                }
                StartCoroutine(Rotate(false));
            }
            else if (Input.GetButton("rotateRight"))
            {
                if (OnWalk != null)
                {
                    OnWalk(WalkInstruction.RotateRight, false, true, true);
                }
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
        Angler.HideLures();
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
                        if (OnMazeEvent != null)
                        {
                            OnMazeEvent(MazeEventTypes.Teleport, currentTile);
                        }                
					    SetCurrentTile (action.tile);                  
                        break;

				    case "rotate":
                        if (OnMazeEvent != null)
                        {
                            OnMazeEvent(MazeEventTypes.Rotate, currentTile);
                        }
                        Direction targetDirection = (Direction)(((int)facingDirection + action.GetInteger(0)) % 4);
                        importRoom.ManageDoors(currentTile, targetDirection);
                        SetCurrentDirection (targetDirection, false);
					    break;

				    case "lookat":
                        if (OnMazeEvent != null)
                        {
                            OnMazeEvent(MazeEventTypes.LookAt, currentTile);
                        }
                        targetDirection = action.GetDirection(0);
                        importRoom.ManageDoors(currentTile, targetDirection);
                        SetCurrentDirection (targetDirection, false);
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
        Angler.HideLures(); 
        while (progress < 1)
        {
            lookTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotateAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / rotationDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        SetCurrentDirection(targetDirection, true);        
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
