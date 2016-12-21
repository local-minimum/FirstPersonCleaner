using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamLookMode { DioramaAlignToMain, DioramaLookAtFocus, DioramaCounterRotate, None};
public enum CamFieldOfViewMode { Sync, Static, Relative};
public enum CamTranslateMode {Relative, RelativeScaled, FixedPlaneRelative, FixedPlaneInverseScale};
public class Diorama : MonoBehaviour {
    
    Camera mainCam;

    [SerializeField]
    Transform referenceWorldPosition;

    [SerializeField]
    Camera dioramaCam;

    [SerializeField]
    Transform dioramaReferenecPosition;
    
    [SerializeField]
    Transform worldFocus;

    [SerializeField]
    Transform sceneFocus;

    [SerializeField]
    CamLookMode camLookMode = CamLookMode.None;

    Quaternion originalDioramaCamRotation;

    [SerializeField]
    CamFieldOfViewMode camFieldOfViewMode = CamFieldOfViewMode.Relative;

    float originalDioramaCamFoV;

    [SerializeField]
    CamTranslateMode camTranslateMode = CamTranslateMode.FixedPlaneRelative;

    float worldRayDistMax = 8f;

    MouseController mouseCtrl;

    [SerializeField]
    LayerMask worldDioramaHitLayers;

    void Start()
    {
        mouseCtrl = PlayerWalkController.PlayerCTRL.MouseCtrl;
        mainCam = mouseCtrl.Cam;
        originalDioramaCamRotation = dioramaCam.transform.rotation;
        originalDioramaCamFoV = dioramaCam.fieldOfView;
    }

	void Update () {
        float factor = TranslateDioramaCam();
        SyncDioramaCamProperties(factor);
	}

    float TranslateDioramaCam()
    {
        float factor = Vector3.Distance(dioramaReferenecPosition.position, sceneFocus.position) / Vector3.Distance(referenceWorldPosition.position, worldFocus.position);
        
        Vector3 realWorldRefOffset = (mainCam.transform.position - referenceWorldPosition.position);
        Vector3 worldRefVector = (referenceWorldPosition.position - worldFocus.position);
        Vector3 refViewDirection = worldRefVector.normalized;
        Vector3 distanceVector = Vector3.Dot(realWorldRefOffset, refViewDirection) * refViewDirection;
        Vector3 orthoVector = realWorldRefOffset - distanceVector;
        float distance = distanceVector.magnitude;

        if (camTranslateMode == CamTranslateMode.RelativeScaled)
        {
            dioramaCam.transform.position = distanceVector * factor - orthoVector + dioramaReferenecPosition.position;
        } else if (camTranslateMode == CamTranslateMode.Relative)
        {
            dioramaCam.transform.position = distanceVector - orthoVector + dioramaReferenecPosition.position;
        } else if (camTranslateMode == CamTranslateMode.FixedPlaneRelative)
        {
            dioramaCam.transform.position = dioramaReferenecPosition.position - orthoVector;
        } else
        {
            dioramaCam.transform.position = dioramaReferenecPosition.position - orthoVector / distance;
        }


        return distance / worldRefVector.magnitude;
    }

    void SyncDioramaCamProperties(float distanceFactor)
    {
        if (camLookMode == CamLookMode.DioramaAlignToMain)
        {
            dioramaCam.transform.rotation = mainCam.transform.rotation;
        }
        else if (camLookMode == CamLookMode.DioramaLookAtFocus) {
            dioramaCam.transform.LookAt(sceneFocus);
        } else if (camLookMode == CamLookMode.DioramaCounterRotate)
        {
            dioramaCam.transform.rotation = Quaternion.Inverse(mainCam.transform.rotation);
        } else
        {
            dioramaCam.transform.rotation = originalDioramaCamRotation;
        }

        if (camFieldOfViewMode == CamFieldOfViewMode.Sync)
        {
            dioramaCam.fieldOfView = mainCam.fieldOfView;
        }
        else if (camFieldOfViewMode == CamFieldOfViewMode.Relative)
        {
            dioramaCam.fieldOfView = mainCam.fieldOfView / distanceFactor;
        }
        else {
            dioramaCam.fieldOfView = originalDioramaCamFoV;
        }
    }

    void OnDrawGizmos()
    {
        if (mouseCtrl == null)
        {
            return;
        }

        Ray ray = mouseCtrl.MouseRay;
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, worldRayDistMax, worldDioramaHitLayers))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, hit.point);

            //Calculate rotation of world cam to ray direction with regards to world cam forward
            Quaternion worldRotation = Quaternion.FromToRotation(mainCam.transform.forward, ray.direction);            
            
            //Get Relevant cam to cam rotation

            //Scale orignal rotation with relation to FoV of both.

            //Get new direction


            Ray dioramaRay = new Ray(dioramaCam.transform.position, ray.direction);
            Gizmos.DrawLine(dioramaRay.origin, dioramaRay.GetPoint(10));
        }
    }
}
