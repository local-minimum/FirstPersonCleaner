using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angler : MonoBehaviour {

    static List<Angler> anglers = new List<Angler>();

    [SerializeField]
    GameObject lure;

    [SerializeField]
    int minFlickers = 1;

    [SerializeField]
    int maxFlickers = 3;

    [SerializeField]
    AnimationCurve flickerDurations;

    [SerializeField]
    AnimationCurve flickerIntermissions;

    [SerializeField]
    float flickerProbPerSec = 0.05f;

    public static void HideLures()
    {
        for (int i=0, l=anglers.Count; i< l; i++)
        {
            anglers[i].HideLure();
        }
    }

	void Start () {
        anglers.Add(this);
        lure.SetActive(false);
	}
	    
    void HideLure()
    {
        lure.SetActive(false);
        continueLure = false;
    }

    bool continueLure = false;

	void Update () {
	    if (!continueLure && Random.value < flickerProbPerSec * Time.deltaTime)
        {
            StartCoroutine(Flicker());
        }	
	}

    IEnumerator<WaitForSeconds> Flicker()
    {
        continueLure = true;
        for (int i = 0, l = Random.Range(minFlickers, maxFlickers); i <l; i++)
        {
            if (continueLure)
            {
                lure.SetActive(true);
                yield return new WaitForSeconds(flickerDurations.Evaluate(Random.value));
            }

            if (continueLure)
            {
                lure.SetActive(false);
                yield return new WaitForSeconds(flickerIntermissions.Evaluate(Random.value));
            }
        }
        continueLure = false;
    }
}
