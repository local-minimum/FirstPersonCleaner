using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStatsCollector : MonoBehaviour {


#if UNITY_EDITOR
    [SerializeField]
    bool showGUI;
#endif
 
    PlayerWalkController playerWalkCtrl;
    MainShader glitchShader;
    MouseController playerMouseCtrl;

    void Awake()
    {
        playerWalkCtrl = GetComponent<PlayerWalkController>();
        glitchShader = playerWalkCtrl.PlayerCam.GetComponent<MainShader>();
        playerMouseCtrl = glitchShader.GetComponent<MouseController>(); 
    }

    void OnEnable()
    {

        playerWalkCtrl.OnWalk += PlayerWalkCtrl_OnWalk;
        playerWalkCtrl.OnMazeEvent += PlayerWalkCtrl_OnMazeEvent;
        glitchShader.OnGlitchLevelChange += GlitchShader_OnGlitchLevelChange;
        playerMouseCtrl.OnMouseClickEvent += PlayerMouseCtrl_OnMouseClickEvent;

    }

    void OnDisable()
    {
        playerWalkCtrl.OnWalk -= PlayerWalkCtrl_OnWalk;
        playerWalkCtrl.OnMazeEvent -= PlayerWalkCtrl_OnMazeEvent;
        glitchShader.OnGlitchLevelChange -= GlitchShader_OnGlitchLevelChange;
        playerMouseCtrl.OnMouseClickEvent -= PlayerMouseCtrl_OnMouseClickEvent;
    }

    int attemptedEnterDiorama = 0;
    int reversedIntoRoom = 0;
    int turns = 0;
    int successfulWalkForward = 0;
    int successfulWalkBackward = 0;
    int peeksIntoDiorama = 0;
    int inspectedWorkOrder = 0;
    int nothingClicks;
    int cleaned;
    int uncleaned;
    float glitchLvl;
    int mazeTeleportations;
    int mazeRotations;
    int mazeLookats;

    Dictionary<CorridorTile, int> mazeStats = new Dictionary<CorridorTile, int>();


    private void PlayerWalkCtrl_OnMazeEvent(MazeEventTypes eventType, CorridorTile causeTile)
    {
        if (!mazeStats.ContainsKey(causeTile))
        {
            mazeStats[causeTile] = 1;
        } else
        {
            mazeStats[causeTile]++;
        }

        if (eventType == MazeEventTypes.Teleport)
        {
            mazeTeleportations++;
        } else if (eventType == MazeEventTypes.Rotate)
        {
            mazeRotations++;
        } else if (eventType == MazeEventTypes.LookAt) {
            mazeLookats++;
        }
    }

    private void PlayerMouseCtrl_OnMouseClickEvent(MouseInteractionTypes interaction, bool refused, bool returnToNativeState)
    {
        if (interaction == MouseInteractionTypes.Door)
        {
            if (!returnToNativeState)
            {
                peeksIntoDiorama++;
            }
        } else if (interaction == MouseInteractionTypes.WorkOrder)
        {
            if (!returnToNativeState)
            {
                inspectedWorkOrder++;
            }
        } else if (interaction == MouseInteractionTypes.None)
        {
            nothingClicks++;
        } else if (interaction == MouseInteractionTypes.Room)
        {
            if (!refused)
            {
                if (returnToNativeState)
                {
                    uncleaned++;
                } else
                {
                    cleaned++;
                }
            }
        }
    }

    private void GlitchShader_OnGlitchLevelChange(bool isActive, float frequency)
    {
        if (isActive)
        {
            glitchLvl = frequency;
        } else
        {
            glitchLvl = -1;
        }
    }

    private void PlayerWalkCtrl_OnWalk(WalkInstruction instruction, bool refused, bool wasFacingDoor, bool isLookingIntoDoor)
    {
        if (refused)
        {
            if (isLookingIntoDoor && instruction == WalkInstruction.Forward)
            {
                attemptedEnterDiorama++;
            } else if (instruction == WalkInstruction.Reverse)
            {
                reversedIntoRoom++;
            }

        } else if (instruction == WalkInstruction.Forward && !wasFacingDoor)
        {
            successfulWalkForward++;
        } else if (instruction == WalkInstruction.Reverse && !isLookingIntoDoor)
        {
            successfulWalkBackward++;
        } else if (instruction == WalkInstruction.Forward && !isLookingIntoDoor)
        {
            peeksIntoDiorama++;
        } else if (instruction == WalkInstruction.RotateLeft || instruction == WalkInstruction.RotateRight) {
            turns++;
        }
        else
        {
            Debug.Log(string.Format("{0}, refused {1}, was facing door {2}, is peeking {3}", instruction, refused, wasFacingDoor, isLookingIntoDoor));
        }
    }

#if UNITY_EDITOR

    void OnGUI()
    {
        if (showGUI)
        {
            GUILayout.BeginHorizontal();


            GUILayout.BeginVertical();
            GUILayout.Label("Success Forward");
            GUILayout.Label("Success Backward");
            GUILayout.Label("Turns");
            GUILayout.Label("Peeks");
            GUILayout.Label("Fail enter room");
            GUILayout.Label("Reveres into wall");

            GUILayout.Label("Glitch level");

            GUILayout.Label("Cleaned");
            GUILayout.Label("Uncleaned");
            GUILayout.Label("Nothing clicks");

            GUILayout.Label("Maze Teleports");
            GUILayout.Label("Maze Rotations");
            GUILayout.Label("Maze Look at:s");
            GUILayout.Label("Most invoked maze thing");

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(successfulWalkForward.ToString());
            GUILayout.Label(successfulWalkBackward.ToString());
            GUILayout.Label(turns.ToString());
            GUILayout.Label(peeksIntoDiorama.ToString());
            GUILayout.Label(attemptedEnterDiorama.ToString());
            GUILayout.Label(reversedIntoRoom.ToString());
            GUILayout.Label(glitchLvl.ToString());
            GUILayout.Label(cleaned.ToString());
            GUILayout.Label(uncleaned.ToString());
            GUILayout.Label(nothingClicks.ToString());
            GUILayout.Label(mazeTeleportations.ToString());
            GUILayout.Label(mazeRotations.ToString());
            GUILayout.Label(mazeLookats.ToString());

            GUILayout.Label(mazeStats.Values.Any() ? mazeStats.Values.Max().ToString() : "--");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }

#endif
}
