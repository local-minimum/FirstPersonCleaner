using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShader : MonoBehaviour {

	public Material mat;
	public Shader shader;
    bool hasGlitch = true;

    [SerializeField]
    float frequency = 0.004f;

    [SerializeField]
    float minFrequency = 0.01f;
    [SerializeField]
    float maxFrequency = 0.3f;

    [SerializeField]
    float freqDelta = 0.002f;
	void Start() {
		mat = new Material(shader);
        startFreq = frequency;
	}

    float startFreq;

    public void ResetFrequency() {
        frequency = startFreq;
        mat.SetFloat("_DispProbability", frequency);
    }

    public void IncreaseFrequency()
    {
        frequency = Mathf.Clamp(frequency + freqDelta, minFrequency, maxFrequency);
        mat.SetFloat("_DispProbability", frequency);
        hasGlitch = true;        
    }

    public void DecreaseFrequency()
    {
        frequency = Mathf.Clamp(frequency - freqDelta, minFrequency, maxFrequency);
        if (frequency <= minFrequency)
        {
            hasGlitch = false;
        }
        mat.SetFloat("_DispProbability", frequency);        
    }

    public void SetMaxGlitchFrequency()
    {
        frequency = maxFrequency;
        mat.SetFloat("_DispProbability", frequency);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (hasGlitch) {
			Graphics.Blit (src, dest, mat);
		} else {
			Graphics.Blit(src, dest);			
		}
	}
}
