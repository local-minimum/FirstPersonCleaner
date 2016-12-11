using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShader : MonoBehaviour {

	public Material mat;
	public Shader shader;
	void Start() {
		mat = new Material(shader);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, mat);
	}
}
