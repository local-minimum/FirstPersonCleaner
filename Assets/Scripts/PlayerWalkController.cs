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

            }
            else if (Input.GetButtonDown("rotateRight"))
            {

            }
        }
	}

    IEnumerator<WaitForSeconds> RefuseWalk()
    {
        transitioning = true;
        float start = Time.timeSinceLevelLoad;
        float end = start + refuseDuration;
        float t = start;
        Vector3 startPos = currentTile.playerPosition;
        Vector3 targetPos = startPos + CorridorTile.GetLookDirection(facingDirection);
        while (t < end)
        {
            
            transform.position = Vector3.Lerp(startPos, targetPos, walkRefuseAnim.Evaluate(t));
            t = Time.timeSinceLevelLoad;
            yield return new WaitForSeconds(animSpeed);
        }

        transform.position = currentTile.playerPosition;

        transitioning = false;
    }

    IEnumerator<WaitForSeconds> Walk(CorridorTile target)
    {
        transitioning = true;
        float start = Time.timeSinceLevelLoad;
        float end = start + walkDuration;
        float t = start;
        Vector3 startPos = currentTile.playerPosition;
        Vector3 endPos = target.playerPosition;

        while (t < end)
        {
            
            transform.position = Vector3.Lerp(startPos, endPos, walkAnim.Evaluate(t));
            t = Time.timeSinceLevelLoad;
            yield return new WaitForSeconds(animSpeed);
        }

        transform.position = target.playerPosition;
        currentTile = target;
        transitioning = false;
    }
}
