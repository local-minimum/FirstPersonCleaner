using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trolly : MonoBehaviour {

    [SerializeField]
    PlayerWalkController playerCtrl;

    [SerializeField]
    Sounder sounder;

    Direction selfDirection = Direction.South;

    Quaternion target;

    [SerializeField]
    Animator walkAnim;

    public void WalkForward()
    {
        walkAnim.SetTrigger("Walk");
    }

    public void UpdateDirection(bool animateTrolley) {
        CorridorTile tile = playerCtrl.CurrentTile;
        Direction playerDirection = playerCtrl.LookDirection;

        if (tile.GetEdge(playerDirection))
        {
            if (playerDirection == selfDirection)
            {
               //Nothing
            } else
            {
                SetDirection(playerDirection, animateTrolley);
                if (animateTrolley)
                {
                    sounder.PlayOne();
                }
            }
        } else if (tile.GetEdge(selfDirection))
        {
            return;
        } else
        {
            Direction[] otherDirections = new Direction[] {
                CorridorTile.GetRighDirection(selfDirection),
                CorridorTile.GetLeftDirection(selfDirection),                
            };
            foreach (Direction direction in otherDirections)
            {
                if (tile.GetEdge(direction))
                {
                    SetDirection(direction, animateTrolley);
                    if (animateTrolley)
                    {
                        sounder.PlayOne();
                    }
                    return;
                }
            }
            SetDirection(CorridorTile.GetInverseDirection(selfDirection), false);
            if (animateTrolley)
            {
                sounder.PlayOne();
            }
        }
    }
    
    void SetDirection(Direction direction, bool animate)
    {
        selfDirection = direction;
        target = Quaternion.AngleAxis((int) direction * 90, Vector3.up);
        if (!animate)
        {
            transform.rotation = target;
        } else
        {
            rotating = true;
        }
    }

    [SerializeField]
    float rotationAttack = 0.15f;

    bool rotating = false; 

    void Update()
    {
        if (rotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, rotationAttack);
            if (Mathf.Abs(Quaternion.Angle(transform.rotation, target)) < 0.01f)
            {
                transform.rotation = target;
                rotating = false;
            }
        }
    }

}
