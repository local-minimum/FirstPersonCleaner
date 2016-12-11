using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class WorkInstructions : MonoBehaviour {

    Animator anim;

	[SerializeField]
	Camera myCamera;

    void Start()
    {
        anim = GetComponentInParent<Animator>();

    }
	public void PickUp()
    {       
			Debug.Log ("PickUp");
			anim.SetTrigger ("Pickup");
			var dof = myCamera.GetComponent<DepthOfField> ();
			dof.focalLength = 0.8f;
    }

	public void PutDown()
    {
			Debug.Log ("PutDown");
		anim.ResetTrigger (Animator.StringToHash("Pickup"));
			anim.SetTrigger ("Putdown");
			var dof = myCamera.GetComponent<DepthOfField> ();
			dof.focalLength = 2.5f;
    }
}
