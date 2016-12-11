using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShader : MonoBehaviour {

	public Material mat;
	public Shader shader;
	public bool hasGlitch;
	void Start() {
		mat = new Material(shader);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (hasGlitch) {
			Graphics.Blit (src, dest, mat);
		} else {
			Graphics.Blit(src, dest);			
		}
	}
}
