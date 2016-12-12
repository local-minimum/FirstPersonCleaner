using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {


    [SerializeField]
    Text bottomText;

    [SerializeField]
    string[] bottomInstructions;

    [SerializeField]
    float[] bottomInstructionTimes;

    [SerializeField]
    string loadScene;

    [SerializeField]
    Texture2D defaultCursor;

    [SerializeField]
    Texture2D hoverCursor;

    [SerializeField]
    Vector2 cursorHotspot = new Vector2(0.5f, 0.5f);

    [SerializeField]
    Animator[] playAnimators;

    void Start () {
        StartCoroutine(flickInstructions());
	}

    IEnumerator<WaitForSeconds> flickInstructions()
    {
        if (bottomInstructions.Length != bottomInstructionTimes.Length || bottomInstructions.Length == 0)
        {
            yield break;
        }

        int i =0;
        int l = bottomInstructions.Length;
        while (true)
        {
            yield return new WaitForSeconds(bottomInstructionTimes[i]);
            i++;
            i %= l;
            bottomText.text = bottomInstructions[i];
        }
    }

    public void StartGame()
    {
        StartCoroutine(delayPlay());
    }

    [SerializeField]
    float playDelay = 1.9f;

    IEnumerator<WaitForSeconds> delayPlay()
    {
        for (int i =0; i<playAnimators.Length; i++)
        {
            playAnimators[i].SetTrigger("Play");
        }
        yield return new WaitForSeconds(playDelay);
        SceneManager.LoadScene(loadScene);
    }

    public void HoverEnter()
    {
        Cursor.SetCursor(hoverCursor, cursorHotspot, CursorMode.Auto);
    }

    public void HoverExit()
    {
        Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
    }
}
