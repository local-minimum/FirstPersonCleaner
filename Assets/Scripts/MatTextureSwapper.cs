using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatTextureSwapper : MonoBehaviour {

    [SerializeField]
    Texture[] alternatives;

    Texture defaultTex;

    [SerializeField]
    MeshRenderer mRend;

    [SerializeField]
    int matIndex = 0;

    Material mat;

    [SerializeField]
    float minGlitchDuration = 0.4f;

    [SerializeField]
    float maxGlitchDuration = 1f;

    [SerializeField]
    float perSecondProbability = 0;

    [SerializeField]
    bool autoSpawn = false;

    void Start()
    {
        mat = mRend.materials[matIndex];
        defaultTex = mat.GetTexture("_MainTex");
    }

    public void Glitch()
    {
        StartCoroutine(_Glitch());
    }

    IEnumerator<WaitForSeconds> _Glitch()
    {
        mat.SetTexture("_MainTex", alternatives[Random.Range(0, alternatives.Length)]);
        yield return new WaitForSeconds(Random.Range(minGlitchDuration, maxGlitchDuration));
        mat.SetTexture("_MainTex", defaultTex);
    }

    void Update()
    {
        if (autoSpawn)
        {
            if (Random.value < perSecondProbability * Time.deltaTime)
            {
                Glitch();
            }
        }
    }
}
