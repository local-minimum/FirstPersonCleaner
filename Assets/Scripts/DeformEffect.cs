using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformEffect : MonoBehaviour {

    [SerializeField]
    Transform[] effectSources;

    [SerializeField]
    float effectScale = 1f;

    [SerializeField]
    MeshFilter deformee;

    [SerializeField]
    float minSpeed = 1;

    [SerializeField]
    float maxSpeed = 2;

    [SerializeField]
    float proximityPower = 2f;

    Mesh mesh;

    Vector3[] originalVerts;

    float[] effectTracks;

    float[] effectTrackSpeeds;

	void Start () {
        mesh = deformee.mesh;
        originalVerts = mesh.vertices;

        effectTracks = new float[effectSources.Length];
        effectTrackSpeeds = new float[effectSources.Length];
        for (int i=0; i<effectSources.Length; i++)
        {
            effectTracks[i] = Random.value * 100;
            effectTrackSpeeds[i] = Random.Range(minSpeed, maxSpeed);
        }
	}
	
	void Update () {
        Deform();
	}

    void Deform()
    {
        int l = effectTracks.Length;

        float[] magnitudes = new float[l];
        Vector3[] localPositions = new Vector3[l];

        for (int j=0; j< l; j++)
        {
            magnitudes[j] = Mathf.PerlinNoise(Time.timeSinceLevelLoad * effectTrackSpeeds[j], effectTracks[j]);
            localPositions[j] = deformee.transform.InverseTransformPoint(effectSources[j].position);
        }


        int n = originalVerts.Length;
        Vector3[] deformedMesh = new Vector3[n];

        for (int i=0; i< n; i++)
        {
            Vector3 v = originalVerts[i];
            Vector3 off = Vector3.zero;
            for (int j=0; j< l; j++)
            {
                Vector3 d = v - localPositions[j];
                off += d.normalized * magnitudes[j] / Mathf.Pow(Mathf.Max(1, d.sqrMagnitude), proximityPower);
            }

            deformedMesh[i] = v + off;
        }

        mesh.vertices = deformedMesh;
    }
}
