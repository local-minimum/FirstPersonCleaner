using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNDdoor : MonoBehaviour {

    [SerializeField]
    Collider doorCollider;

    [SerializeField]
    MeshRenderer dndRenderer;

    bool triggered = false;
    
    float triggerProbability = .5f;

    int requireNBackwards = 20;

    OneRoomDoor oneRoomDoor;

    PlayerWalkController playerCTRL;

    void Start()
    {
        oneRoomDoor = GetComponentInParent<OneRoomDoor>();
   
    }

	void OnEnable()
    {
        if (playerCTRL == null)
        {
            playerCTRL = PlayerWalkController.PlayerCTRL;
        }
        playerCTRL.OnWalk += PlayerCTRL_OnWalk;
    }

    void OnDisable()
    {
        playerCTRL.OnWalk -= PlayerCTRL_OnWalk;
    }

    private void PlayerCTRL_OnWalk(WalkInstruction instruction, bool refused, bool wasFacingDoor, bool isLookingIntoDoor)
    {
        if (!triggered && instruction == WalkInstruction.Reverse && !isLookingIntoDoor && !refused && playerCTRL.PlayerStats.SuccessfulWalkBackward > requireNBackwards)
        {
            if (oneRoomDoor.IsOpen && Random.value < triggerProbability)
            {
                if (playerCTRL.CurrentTile.HasDoor(oneRoomDoor)) {
                    oneRoomDoor.CloseDoor();
                    triggered = true;
                    doorCollider.enabled = false;
                    dndRenderer.enabled = true;
                }
            }
        }
    }
}
