using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trolly : MonoBehaviour {

    [SerializeField]
    PlayerWalkController playerCtrl;

    Direction selfDirection = Direction.South;

    Quaternion target;

    public void UpdateDirection() {
        CorridorTile tile = playerCtrl.CurrentTile;
        Direction playerDirection = playerCtrl.LookDirection;
        if (tile.GetEdge(playerDirection))
        {
            SetDirection(playerDirection);
        } else if (tile.GetEdge(selfDirection))
        {
            SetDirection(selfDirection);
        } else
        {
            Direction[] otherDirections = new Direction[] {
                CorridorTile.GetRighDirection(selfDirection),
                CorridorTile.GetLeftDirection(selfDirection),
                CorridorTile.GetInverseDirection(selfDirection),
            };
            foreach (Direction direction in otherDirections)
            {
                if (tile.GetEdge(direction))
                {
                    SetDirection(direction);
                    break;
                }
            }
        }
    }

    void SetDirection(Direction direction)
    {
        selfDirection = direction;
        target = Quaternion.AngleAxis((int) direction * 90, Vector3.up);
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.2f);
    }

}
