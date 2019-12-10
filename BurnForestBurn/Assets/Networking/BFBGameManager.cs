using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BFBGameManager : NetworkBehaviour
{
    public delegate void OnGameStart();

    public int localPlayerTeam = -1;
    public AreaTreeController[] areaTreeControllers;

    private bool[] teamsReady = { false, false };
    private bool gameStarted = false;
    private List<OnGameStart> onGameStart;

    void Start()
    {
        onGameStart = new List<OnGameStart>();
    }

    void GameStart()
    {
        if (isServer)
        {
            foreach (AreaTreeController atc in areaTreeControllers)
            {
                atc.SpawnTrees();
            }

            gameStarted = true;

            foreach (OnGameStart ogs in onGameStart)
            {
                ogs();
            }
        }
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    public void RegisterOnGameStart(OnGameStart ogs)
    {
        onGameStart.Add(ogs);
    }

    public void TeamReady(int team, bool ready)
    {
        Debug.LogFormat("teamReady: {0} - {1}", team, ready);

        if (team >= 0)
        {
            teamsReady[team] = ready;
        }

        bool allReady = true;
        foreach (bool tr in teamsReady)
        {
            if (!tr)
            {
                allReady = false;
                break;
            }
        }

        Debug.LogFormat("teamReady: allReady - {0}", allReady);
        if (allReady)
        {
            GameStart();
        }
    }
}
