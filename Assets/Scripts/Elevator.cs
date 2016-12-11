using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

    bool hasBeenTriggered = false;

    [SerializeField]
    float waitForElevatorTime = 2f;

    [SerializeField]
    float waitForDoorsTime = 1.7f;

    [SerializeField]
    float waitDelayCamEffect = 0.4f;

    [SerializeField]
    float waitWhileWalkingIn = 2f;

    [SerializeField]
    float waitFakeLevelLoad = 2f;

    [SerializeField]
    Color buttonPressColor = Color.yellow;

    Animator anim;

    [SerializeField]
    MeshRenderer mRend;

    [SerializeField]
    int buttonMatIndex = 2;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PressForElevator(PlayerWalkController playerCtrl)
    {
        if (hasBeenTriggered)
        {
            return;
        }
        hasBeenTriggered = true;
        StartCoroutine(delayOpenDoors(playerCtrl));
    }

    public IEnumerator<WaitForSeconds> delayOpenDoors(PlayerWalkController playerCtrl)
    {
        Material mat = mRend.materials[buttonMatIndex];
        Color restingColor = mat.color;
        mat.color = buttonPressColor;
        playerCtrl.frozen = true;
        yield return new WaitForSeconds(waitForElevatorTime);
        mat.color = restingColor;
        anim.SetTrigger("OpenDoors");
        yield return new WaitForSeconds(waitForDoorsTime);
        playerCtrl.EnterElevator();
        yield return new WaitForSeconds(waitDelayCamEffect);
        playerCtrl.WhiteOut();
        yield return new WaitForSeconds(waitWhileWalkingIn);
        playerCtrl.StartNextLevel(waitFakeLevelLoad);
    }
}
