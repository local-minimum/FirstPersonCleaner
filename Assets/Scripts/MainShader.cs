using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GlithLevelChange(bool isActive, float frequency);

public class MainShader : MonoBehaviour {

    public event GlithLevelChange OnGlitchLevelChange;

    [SerializeField]
    KeyCode muteEffectsToggle = KeyCode.G;

    [SerializeField]
    Animator canvasAnimator;

	Material mat;

    [SerializeField]
	Shader shader;

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

    bool glitchIsActive = true;

    public void ResetFrequency() {
        frequency = startFreq;
        mat.SetFloat("_DispProbability", frequency);
        if (OnGlitchLevelChange != null)
        {
            OnGlitchLevelChange(glitchIsActive, frequency);
        }
    }

    public void IncreaseFrequency()
    {
        frequency = Mathf.Clamp(frequency + freqDelta, minFrequency, maxFrequency);
        mat.SetFloat("_DispProbability", frequency);
        hasGlitch = true;
        if (OnGlitchLevelChange != null)
        {
            OnGlitchLevelChange(glitchIsActive, frequency);
        }
    }

    public void DecreaseFrequency()
    {
        frequency = Mathf.Clamp(frequency - freqDelta, minFrequency, maxFrequency);
        if (frequency <= minFrequency)
        {
            hasGlitch = false;
        }
        mat.SetFloat("_DispProbability", frequency);
        if (OnGlitchLevelChange != null)
        {
            OnGlitchLevelChange(glitchIsActive, frequency);
        }
    }

    public void SetMaxGlitchFrequency()
    {
        frequency = maxFrequency;
        mat.SetFloat("_DispProbability", frequency);
        if (OnGlitchLevelChange != null)
        {
            OnGlitchLevelChange(glitchIsActive, frequency);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (hasGlitch && glitchIsActive) {
			Graphics.Blit (src, dest, mat);
		} else {
			Graphics.Blit(src, dest);			
		}
	}

    void Update()
    {
        if (Input.GetKeyDown(muteEffectsToggle))
        {
            glitchIsActive = !glitchIsActive;

            canvasAnimator.ResetTrigger("GlitchOn");
            canvasAnimator.ResetTrigger("GlitchOff");
            canvasAnimator.SetTrigger(glitchIsActive ? "GlitchOn" : "GlitchOff");

            if (OnGlitchLevelChange != null)
            {
                OnGlitchLevelChange(glitchIsActive, frequency);
            }
        }
    }
}
