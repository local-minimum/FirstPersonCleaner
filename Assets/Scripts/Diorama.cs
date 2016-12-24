using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamLookMode { DioramaAlignToMain, DioramaLookAtFocus, DioramaCounterRotate, None};
public enum CamFieldOfViewMode { Sync, Static, Relative, RelativeInverted};
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

    float dioramaRayDistMax = 20f;

    MouseController mouseCtrl;

    [SerializeField]
    LayerMask worldDioramaHitLayers;

    void Start()
    {
        mouseCtrl = PlayerWalkController.PlayerCTRL.MouseCtrl;
        mainCam = mouseCtrl.Cam;
        if (dioramaCam != null)
        {
            originalDioramaCamRotation = dioramaCam.transform.rotation;
            originalDioramaCamFoV = dioramaCam.fieldOfView;
        }
    }

	void Update () {
        if (dioramaCam == null)
        {
            return;
        }
        float factor = TranslateDioramaCam();
        SyncDioramaCamProperties(factor);
	}

    string doorName;

    public void SetDoorName(string doorName)
    {
        this.doorName = doorName;
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
            dioramaCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView / distanceFactor, 10, 120);
        }
        else if (camFieldOfViewMode == CamFieldOfViewMode.RelativeInverted)
        {
            dioramaCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView * distanceFactor, 10, 120);
        }
        else {
            dioramaCam.fieldOfView = originalDioramaCamFoV;
        }
    }

    bool HitValidLayer(int hitLayer)
    {
        return worldDioramaHitLayers.value == (worldDioramaHitLayers.value |  (1 << hitLayer));
    }

    public bool RayCastDiorama(out RaycastHit worldHit, out RaycastHit dioramaHit)
    {
        Ray ray = mouseCtrl.MouseRay;
        if (Physics.Raycast(ray, out worldHit, worldRayDistMax))
        {
            if (!HitValidLayer(worldHit.transform.gameObject.layer))
            {
                dioramaHit = new RaycastHit();
                return false;
            }

            //TODO: Something better maybe?
            Vector3 pt = worldHit.transform.InverseTransformPoint(worldHit.point);
            Ray dioramaRay = dioramaCam.ViewportPointToRay(new Vector2(pt.x + 0.5f, pt.y + 0.5f));

            return Physics.Raycast(dioramaRay, out dioramaHit, dioramaRayDistMax, dioramaCam.cullingMask);
        }
        dioramaHit = new RaycastHit();
        return false;
    }

    public bool RayCastDiorama(RaycastHit worldHit, out RaycastHit dioramaHit)
    {

        if (dioramaCam == null || !HitValidLayer(worldHit.transform.gameObject.layer))
        {
            dioramaHit = new RaycastHit();
            return false;
        }

        //TODO: Something better maybe?
        Vector3 pt = worldHit.transform.InverseTransformPoint(worldHit.point);
        Ray dioramaRay = dioramaCam.ViewportPointToRay(new Vector2(pt.x + 0.5f, pt.y + 0.5f));

        return Physics.Raycast(dioramaRay, out dioramaHit, dioramaRayDistMax, dioramaCam.cullingMask);
    }

    void OnDrawGizmos()
    {
        if (mouseCtrl == null || dioramaCam == null)
        {
            return;
        }

        RaycastHit worldHit;
        RaycastHit dioramaHit;

        if (RayCastDiorama(out worldHit, out dioramaHit))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(mainCam.transform.position, worldHit.point);            
            Gizmos.DrawLine(dioramaCam.transform.position, dioramaHit.point);
        }
    }
}
