using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour {
    
    public CorridorTile currentTile;

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
                CorridorTile target = currentTile.GetEdge(facingDirection);
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
                StartCoroutine(Rotate(true));
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
            
            transform.position = Vector3.Lerp(startPos, targetPos, walkRefuseAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / refuseDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        transform.position = currentTile.playerPosition;

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
            
            transform.position = Vector3.Lerp(startPos, endPos, walkAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / walkDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        transform.position = target.playerPosition;
        currentTile = target;
        transitioning = false;
    }

    IEnumerator<WaitForSeconds> Rotate(bool rotateRight)
    {
        transitioning = true;
        float start = Time.timeSinceLevelLoad;
        float progress = 0;
        Direction targetRotation;
        float rotationA = CorridorTile.GetRotation(facingDirection, rotateRight, out targetRotation);

        while (progress < 1)
        {

            //transform.position = Vector3.Lerp(startPos, endPos, walkAnim.Evaluate(progress));
            progress = (Time.timeSinceLevelLoad - start) / walkDuration;
            yield return new WaitForSeconds(animSpeed);
        }

        //transform.position = target.playerPosition;

        facingDirection = targetRotation;
        transitioning = false;
    }
}
