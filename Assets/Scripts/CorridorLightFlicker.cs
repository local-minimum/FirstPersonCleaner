using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorLightFlicker : MonoBehaviour {

    [SerializeField]
    Light myLight;

    Material lightMat;

    float onIntencity;

    Color onEmission;
    Color onColor;

    [SerializeField]
    float offColorFactor = 0.5f;

    [SerializeField]
    float offEmissionFactor = 0.1f;

    [SerializeField]
    float minOn = 0.05f;

    [SerializeField]
    float maxOn = 0.4f;

    [SerializeField]
    float minOff = 0.2f;

    [SerializeField]
    float maxOff = 2f;

    void Start () {

        if (myLight == null)
        {
            myLight = GetComponent<Light>();
        }

        lightMat = GetComponent<MeshRenderer>().material;

        onColor = lightMat.GetColor("_Color");
        onEmission = lightMat.GetColor("_EmissionColor");
        onIntencity = myLight.intensity;
        StartCoroutine(flicker());

	}
	
	IEnumerator<WaitForSeconds> flicker()
    {
        bool isOn = true;

        while (true)
        {            

            myLight.intensity = isOn ? onIntencity : 0;
            lightMat.SetColor("_Color", onColor * (isOn ? 1 : offColorFactor));
            lightMat.SetColor("_EmissionColor", onEmission * (isOn ? 1 : offEmissionFactor));

            yield return new WaitForSeconds(!isOn ? Random.Range(minOn, minOff) : Random.Range(minOff, maxOff));

            isOn = !isOn;
        }
    }
}
