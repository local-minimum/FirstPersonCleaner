using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVNoise : MonoBehaviour {

    [SerializeField]
    Texture2D tex;

    [SerializeField]
    int pxWidth = 60;

    [SerializeField]
    int pxHeight = 40;

    [SerializeField]
    float updateP = 0.5f;

    Light tvLight;

    [SerializeField]
    AnimationCurve tvLightPower;

    [SerializeField]
    float flickerRate = 1;

    [SerializeField]
    float flickerMagnitude = 4;

    bool tvIsOn = true;

    void Start () {
        tex = new Texture2D(pxWidth, pxHeight);        
        tex.name = "TV noise Tex";
        UpdateTex();
        GetComponent<MeshRenderer>().materials[1].SetTexture("_MainTex", tex);
        tvLight = GetComponentInChildren<Light>();
	}

	
    void Update()
    {
        if (tvIsOn)
        {
            tvLight.intensity = tvLightPower.Evaluate(Time.timeSinceLevelLoad % flickerRate) * flickerMagnitude;
        }
    }

    void LateUpdate () {
        if (tvIsOn)
        {
            UpdateTex();
        }
	}

    void UpdateTex()
    {
        for (int x=0; x<pxWidth; x++)
        {
            for (int y=0; y<pxHeight; y++)
            {
                if (Random.value < updateP)
                {
                    tex.SetPixel(x, y, tex.GetPixel(x, y) == Color.black ? Color.white : Color.black);
                }
            }
        }
        tex.Apply();
    }

    public void TurnOffTV()
    {
        tvIsOn = false;
        for (int x = 0; x < pxWidth; x++)
        {
            for (int y = 0; y < pxHeight; y++)
            {

                tex.SetPixel(x, y, Color.black);
                
            }
        }
        tex.Apply();
        tvLight.enabled = false;
        Material mat = GetComponent<MeshRenderer>().materials[1];
        mat.color = Color.black;        
        mat.SetColor("_EmissionColor", Color.black);
        enabled = false;
    }
}
