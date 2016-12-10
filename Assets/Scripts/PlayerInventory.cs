using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField, Range(0, 99)]
    int capacityTowels = 5;

    [SerializeField, Range(0, 99)]
    int capacityDND = 5;

    [SerializeField, Range(0, 99)]
    int capacityWetFloor = 5;

    [SerializeField]
    GameObject prefabTowels;

    [SerializeField]
    GameObject prefabDND;

    [SerializeField]
    GameObject prefabWetFloor;

    Queue<GameObject> oldTowels = new Queue<GameObject>();
    Queue<GameObject> oldDND = new Queue<GameObject>();
    Queue<GameObject> oldWetFloor = new Queue<GameObject>();

    public GameObject GetTowel()
    {
        if (oldTowels.Count > 0)
        {
            GameObject go = oldTowels.Dequeue();
            go.SetActive(true);
            return go;

        } else if (capacityTowels > 0)
        {
            capacityTowels--;
            return Instantiate(prefabTowels);
        } else
        {
            return null;
        }
    }

    public void ReturnTowel(GameObject towel)
    {
        towel.transform.SetParent(transform);
        towel.SetActive(false);
        oldTowels.Enqueue(towel);
    }

    public GameObject GetDND()
    {
        if (oldDND.Count > 0)
        {
            GameObject go = oldDND.Dequeue();
            go.SetActive(true);
            return go;
        }
        else if (capacityDND > 0)
        {
            capacityDND--;
            return Instantiate(prefabDND);
        }
        else
        {
            return null;
        }
    }

    public void ReturnDND(GameObject dnd)
    {
        dnd.transform.SetParent(transform);
        dnd.SetActive(false);
        oldDND.Enqueue(dnd);
    }

    public GameObject GETWetFloor()
    {
        if (oldWetFloor.Count > 0)
        {
            GameObject go = oldWetFloor.Dequeue();
            go.SetActive(true);
            return go;
        }
        else if (capacityWetFloor > 0)
        {
            capacityWetFloor--;
            return Instantiate(prefabWetFloor);
        }
        else
        {
            return null;
        }
    }

    public void ReturnWetFloor(GameObject wetFloor)
    {
        wetFloor.transform.SetParent(transform);
        wetFloor.SetActive(false);
        oldWetFloor.Enqueue(wetFloor);
    }
}
