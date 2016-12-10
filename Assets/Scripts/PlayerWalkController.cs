using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour {

    [SerializeField]
    Transform movementTransform;

    [SerializeField]
    Transform lookTransform;

    [SerializeField]
    CorridorTile currentTile;

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

    public void SetCurrentTile(CorridorTile tile)
    {
        currentTile = tile;
        movementTransform.position = currentTile.playerPosition;

    }

    public void SetCurrentDirection(Direction direction)
    {

    }

    float animSpeed = 0.01f;


    bool transitioning = false;

	void Update () {
        if (!transitioning)
        {
            if (Input.GetButtonDown("walkForward"))
            {
                CorridorTile target = currentTile.GetEdge(facingDirection);
                if (target == null)
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
                if (target == null)
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

        movementTransform.position = target.playerPosition;
        currentTile = target;
        transitioning = false;
    }

    IEnumerator<WaitForSeconds> Rotate(bool rotateRight)
    {
        transitioning = true;
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Direction targetDirection;
        CorridorTile.GetRotation(facingDirection, rotateRight, out targetDirection);
        Quaternion startRotation = lookTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(CorridorTile.GetLookDirection(targetDirection), Vector3.up);
        while (progress < 1)
        {
            
            lookTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotateAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / walkDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        lookTransform.rotation = targetRotation;
        facingDirection = targetDirection;
        transitioning = false;
    }
}
