using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class WorkInstructions : MonoBehaviour {

    Animator anim;

	[SerializeField]
	Camera myCamera;

    bool isShowing;

    void Start()
    {
        anim = GetComponentInParent<Animator>();

    }

	public void PickUp()
    {       
		anim.SetTrigger ("Pickup");
		var dof = myCamera.GetComponent<DepthOfField> ();
		dof.focalLength = 0.8f;
        isShowing = true;
    }

	public void PutDown()
    {
		anim.ResetTrigger (Animator.StringToHash("Pickup"));
		anim.SetTrigger ("Putdown");
		var dof = myCamera.GetComponent<DepthOfField> ();
		dof.focalLength = 2.5f;
        isShowing = false;
    }

    public void Toggle()
    {
        if (isShowing)
        {
            PutDown();
        } else
        {
            PickUp();
        }
    }
}
