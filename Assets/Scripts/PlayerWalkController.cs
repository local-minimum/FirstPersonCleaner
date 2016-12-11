using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour {

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

    [SerializeField]
    Animator camAnim;

	[SerializeField]
	Camera myCamera;

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

    void Start()
    {
        inventory = GetComponentInParent<PlayerInventory>();
    }

    public void LookIntoOneRoom()
    {
        camAnim.SetBool("Looking", true);
    }

    public void StopLookingIntoOneRoom()
    {
        camAnim.SetBool("Looking", false);
    }

	void Update () {
        if (!transitioning)
        {
            if (Input.GetButtonDown("walkForward"))
            {
                CorridorTile target = currentTile.GetEdge(facingDirection);
				if (target == null || currentTile == endTile)
                {
                    StartCoroutine(RefuseWalk());
                }
                else
                {
                    StartCoroutine(Walk(target));
                }
            }
            else if (Input.GetButtonDown("walkReverse"))
            {
                CorridorTile target = currentTile.GetEdge(CorridorTile.GetInverseDirection(facingDirection));
				if (target == null || currentTile == endTile)
                {
                    StartCoroutine(RefuseWalk());
                } else
                {
                    StartCoroutine(Walk(target));
                }
            }
            else if (Input.GetButtonDown("rotateLeft"))
            {
                StartCoroutine(Rotate(false));
            }
            else if (Input.GetButtonDown("rotateRight"))
            {
                StartCoroutine(Rotate(true));
            }
        }
	}

    IEnumerator<WaitForSeconds> RefuseWalk()
    {
        transitioning = true;
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
        transitioning = true;
        //currentTile.CloseAllDoors();
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Vector3 startPos = currentTile.playerPosition;
        Vector3 endPos = target.playerPosition;

        while (progress < 1)
        {
            
            movementTransform.position = Vector3.Lerp(startPos, endPos, walkAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / walkDuration;
            yield return new WaitForSeconds(animSpeed);
        }

		Direction from = target.GetPreviousDirection (currentTile);
		Debug.Log (from);
        SetCurrentTile(target);                
		foreach (Action action in currentTile.actions) {
			if (action.IsActive (from)) {
				switch (action.action) {
				case "teleport":				
					SetCurrentTile (action.tile);
					break;
				case "rotate":
					SetCurrentDirection ((Direction)(((int)facingDirection + action.GetInteger (0)) % 4));
					break;
				case "lookat":
					SetCurrentDirection (action.GetDirection (0));
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
        transitioning = true;
        if (currentTile.CloseAllDoors())
        {
            StopLookingIntoOneRoom();
        }
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Direction targetDirection;
        CorridorTile.GetRotation(facingDirection, rotateRight, out targetDirection);
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

}
