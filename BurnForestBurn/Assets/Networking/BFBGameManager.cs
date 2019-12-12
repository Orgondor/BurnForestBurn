using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BFBGameManager : NetworkBehaviour
{
    public delegate void OnGameStart();
    public delegate void OnGameEnd(int winningTeam);

    public int localPlayerTeam = -1;
    public AreaTreeController[] areaTreeControllers;

    private bool[] teamsReady = { false, false };
    private int[] teamAreas = { 0, 0 };
    private int[] teamAreasDone = { 0, 0 };
    private bool gameStarted = false;
    private List<OnGameStart> onGameStart;
    private List<OnGameEnd> onGameEnd;

    void Start()
    {
        if (onGameStart == null)
            onGameStart = new List<OnGameStart>();
        if (onGameEnd == null)
            onGameEnd = new List<OnGameEnd>();

        foreach (AreaTreeController atc in areaTreeControllers)
        {
            atc.SetOnAreaDone(AreaDone);
            atc.SetOnRegisterTeamArea(RegisterTeamArea);
        }
    }

    void GameStart()
    {
        if (isServer && !gameStarted)
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

    public void AreaDone(int team)
    {
        teamAreasDone[team]++;

        if (teamAreasDone[team] >= teamAreas[team])
        {
            // We have a winner!
            Debug.LogFormat("Team {0} wins!", team);

            foreach (OnGameEnd oge in onGameEnd)
            {
                oge(team);
            }
        }
    }

    public void RegisterTeamArea(int team)
    {
        teamAreas[team]++;
    }

    public void RegisterOnGameStart(OnGameStart ogs)
    {
        if (onGameStart == null)
            onGameStart = new List<OnGameStart>();

        onGameStart.Add(ogs);
    }

    public void RegisterOnGameEnd(OnGameEnd oge)
    {
        if (onGameEnd == null)
            onGameEnd = new List<OnGameEnd>();

        onGameEnd.Add(oge);
    }

    [ClientRpc]
    void RpcDebugOutput(string message)
    {
        Debug.Log(message);
    }

    public void TeamReady(int team, bool ready)
    {
        if (!isServer)
            return;

        RpcDebugOutput(String.Format("teamReady: {0} - {1}", team, ready));

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

        RpcDebugOutput(String.Format("teamReady: allReady - {0}", allReady));
        if (allReady)
        {
            GameStart();
        }
    }
}
