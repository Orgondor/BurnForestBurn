using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

[System.Obsolete]
public class ClickHandler : NetworkBehaviour
{
    private int team = -1;
    private BFBGameManager gameManager = null;
    private bool gameEnded = false;
    private NetworkManager networkManager = null;
    private float score = 0;
    private float displayScore = 0;

    public Button readyButton;
    public Button restartButton;
    public TextMeshProUGUI gameEndedText;
    public TextMeshProUGUI scoreText;
    public float scoreSpeed = 75.0f;
    public float scorePerTree = 25.0f;

    // For debug on PC
    private bool clickBegan = true;

    void Start()
    {
        if (isServer)
        {
            GameObject temp = GameObject.Find("GameManager");

            if (temp)
            {
                gameManager = temp.GetComponent<BFBGameManager>();
                if (gameManager)
                {
                    RpcDebugOutput(System.String.Format("Register clickhandler local: {0}", isLocalPlayer));
                    gameManager.RegisterOnGameStart(OnGameStart);
                    gameManager.RegisterOnGameEnd(OnGameEnd);
                    SetupTeam();
                }
                else
                    Debug.Log("ERROR!!!!");
            }
            else
                Debug.Log("ERROR 2!!!!");


            temp = GameObject.Find("BFBNetworkingManager");
            if (temp)
            {
                networkManager = temp.GetComponent<NetworkManager>();
            }

            if (!networkManager)
            {
                Debug.Log("NetworkManager not found");
            }
        }

        if (isLocalPlayer)
        {
            readyButton.interactable = true;
            readyButton.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (displayScore < score)
            {
                displayScore += scoreSpeed * Time.deltaTime;
                if (displayScore > score)
                {
                    displayScore = score;
                }
            }
            scoreText.SetText(((int)System.Math.Round(displayScore)).ToString());

            if (!gameEnded)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit rayhit = new RaycastHit();

                    if (Physics.Raycast(ray, out rayhit))
                    {
                        GameObject hitObject = rayhit.collider.gameObject;

                        if (hitObject.tag == "tree")
                        {
                            CmdDestroyTree(hitObject);
                        }
                    }
                }

                // For debug on PC
                else if (Input.GetMouseButtonDown(0) && clickBegan)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit rayhit = new RaycastHit();
                    clickBegan = false;

                    if (Physics.Raycast(ray, out rayhit))
                    {
                        GameObject hitObject = rayhit.collider.gameObject;

                        if (hitObject.tag == "tree")
                        {
                            CmdDestroyTree(hitObject);
                        }
                    }
                }
                else if (!Input.GetMouseButtonDown(0))
                    clickBegan = true;
            }
        }
    }

    void SetupTeam()
    {
        if (!isServer)
            return;

        if (isLocalPlayer)
        {
            gameManager.localPlayerTeam = Random.Range(0, 2);
            team = gameManager.localPlayerTeam;
        }
        else if (gameManager.localPlayerTeam >= 0)
        {
            team = 1 - gameManager.localPlayerTeam;
            RpcSetClientTeam(team);
        }
    }

    [ClientRpc]
    void RpcSetClientTeam(int teamId)
    {
        team = teamId;
    }

    public void ReadyClicked()
    {
        CmdReadyClicked();
    }

    public void RestartClicked()
    {
        CmdRestartClicked();
    }

    void OnGameStart()
    {
        if (!isServer)
            return;

        RpcDebugOutput(System.String.Format("OnGameStart local: {0}", isLocalPlayer));
        RpcOnGameStart();
    }

    void OnGameEnd(int winningTeam)
    {
        if (!isServer)
            return;

        RpcOnGameEnd(winningTeam == team);
    }

    [ClientRpc]
    void RpcOnGameEnd(bool winner)
    {
        gameEnded = true;
        if (isLocalPlayer)
        {
            gameEndedText.SetText(winner ? "You Won" : "You Lost");
            gameEndedText.gameObject.SetActive(true);

            if (isServer)
            {
                restartButton.gameObject.SetActive(true);
                restartButton.interactable = true;
            }
        }
    }

    [ClientRpc]
    void RpcOnGameStart()
    {
        if (isLocalPlayer)
        {
            readyButton.interactable = false;
            readyButton.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcDebugOutput(string message)
    {
        Debug.Log(message);
    }

    [Command]
    void CmdReadyClicked()
    {
        if (!isServer)
            return;

        if (team < 0)
            SetupTeam();
        gameManager.TeamReady(team, true);
    }

    [Command]
    void CmdRestartClicked()
    {
        if (!isServer)
            return;

        networkManager.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    [ClientRpc]
    void RpcDestroyTree(GameObject tree)
    {
        TreeBehaviour tb = tree.GetComponent<TreeBehaviour>();
        tb.DestroyTree();

        if (isLocalPlayer)
        {
            score += scorePerTree;
        }
    }

    [ClientRpc]
    void RpcDebugTree(int treeTeam, int playerTeam)
    {
        Debug.LogFormat("Cut attempt, teams - Tree: {0} Player: {1}", treeTeam, playerTeam);
    }

    [Command]
    void CmdDestroyTree(GameObject tree)
    {
        if (!isServer)
            return;

        TreeBehaviour tb = tree.GetComponent<TreeBehaviour>();

        RpcDebugTree(tb.team, team);
        if (tb.team == team)
        {
            RpcDestroyTree(tree);
        }
    }
}
