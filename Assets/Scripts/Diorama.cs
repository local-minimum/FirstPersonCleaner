using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diorama : MonoBehaviour {

    [SerializeField]
    Camera mainCam;

    [SerializeField]
    Transform referenceWorldPosition;

    [SerializeField]
    Camera dioramaCam;

    [SerializeField]
    Transform dioramaReferenecPosition;
    
    [SerializeField]
    Transform screen;

    [SerializeField]
    Transform sceneFocus;

	void Update () {
        TranslateDioramaCam();
        dioramaCam.transform.rotation = mainCam.transform.rotation;
        dioramaCam.fieldOfView = mainCam.fieldOfView;
	}


    void TranslateDioramaCam()
    {
        dioramaCam.transform.position = mainCam.transform.position - referenceWorldPosition.position + dioramaReferenecPosition.position;
    }

}
