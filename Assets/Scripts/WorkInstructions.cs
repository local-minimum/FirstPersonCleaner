using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkInstructions : MonoBehaviour {

    Animator anim;

    void Start()
    {
        anim = GetComponentInParent<Animator>();

    }
	public void PickUp()
    {       
        anim.SetTrigger("Pickup");
    }

    public void PutDown()
    {
        anim.SetTrigger("Putdown");
    }
}
