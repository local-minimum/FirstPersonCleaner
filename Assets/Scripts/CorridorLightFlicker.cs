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
    float minFlicker = 0.05f;

    [SerializeField]
    float maxFlicker = 0.4f;

    [SerializeField]
    int minNFlick = 1;

    [SerializeField]
    int maxNFlick = 5;

    [SerializeField]
    float minOff = 0.2f;

    [SerializeField]
    float maxOff = 2f;


    void Start () {

        if (myLight == null)
        {
            myLight = GetComponent<Light>();
        }

        lightMat = GetComponentInParent<MeshRenderer>().material;

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
            for (int i = 0, l = Random.Range(minNFlick, maxNFlick) * 2 - 1; i < l; i++)
            {
                myLight.intensity = isOn ? onIntencity : 0;
                lightMat.SetColor("_Color", onColor * (isOn ? 1 : offColorFactor));
                lightMat.SetColor("_EmissionColor", onEmission * (isOn ? 1 : offEmissionFactor));

                yield return new WaitForSeconds(Random.Range(minFlicker, maxFlicker));
                isOn = !isOn;
            }

            myLight.intensity = isOn ? onIntencity : 0;
            lightMat.SetColor("_Color", onColor * (isOn ? 1 : offColorFactor));
            lightMat.SetColor("_EmissionColor", onEmission * (isOn ? 1 : offEmissionFactor));

            yield return new WaitForSeconds(Random.Range(minOff, maxOff));

            isOn = true;
        }
    }
}
