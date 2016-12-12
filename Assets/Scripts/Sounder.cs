using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounder : MonoBehaviour {

    [SerializeField]
    AudioSource speaker;

    [SerializeField]
    AudioClip[] alternativeSoundBites;

    [SerializeField, Range(0, 1)]
    float volume = 1;

    [SerializeField]
    float sequenceDelay;

    [SerializeField]
    float probabilityPlay = 0.5f;

    [SerializeField]
    bool emit;

	void Start () {
        if (speaker == null)
        {
            speaker = GetComponent<AudioSource>();
            if (speaker == null)
            {
                speaker = gameObject.AddComponent<AudioSource>();
            }
        }
	}
	
    public void ProbabilityPlayOne()
    {
        if (Random.value < probabilityPlay)
        {
            PlayOne();
        }
    }

    static void DebugTree(Transform t)
    {
        if (t.parent)
        {
            DebugTree(t.parent);           
        }
        Debug.Log(t.name);
    }

	public void PlayOne()
    {

        speaker.PlayOneShot(alternativeSoundBites[Random.Range(0, alternativeSoundBites.Length)], volume);
    }

    public void PlayInSequence(int count)
    {
        StartCoroutine(_sequence(count));
    }

    int playSeed = 0;

    IEnumerator<WaitForSeconds> _sequence(float count)
    {
        playSeed++;
        int mySeed = playSeed;

        while (count > 0 && mySeed == playSeed)
        {
            PlayOne();
            yield return new WaitForSeconds(sequenceDelay);
            count--;

        }
    }

    void Update()
    {
        if (emit)
        {
            emit = false;
            PlayOne();
        }
    }
}
