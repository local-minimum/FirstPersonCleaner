using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorLightFlicker : MonoBehaviour {

    Light myLight;
    float onIntencity;
    
	void Start () {
        myLight = GetComponent<Light>();
        onIntencity = myLight.intensity;

        StartCoroutine(flicker());
	}
	
	IEnumerator<WaitForSeconds> flicker()
    {
        bool isOn = true;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 1));
            myLight.intensity = isOn ? 0 : onIntencity;
            isOn = !isOn;
        }
    }
}
