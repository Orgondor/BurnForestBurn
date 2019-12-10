using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Obsolete]
public class ClickHandler : NetworkBehaviour
{
    private int team = -1;
    private BFBGameManager gameManager = null;

    public Button readyButton;

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
                    gameManager.RegisterOnGameStart(OnGameStart);
                    if (isLocalPlayer)
                    {
                        gameManager.localPlayerTeam = Random.Range(0, 2);
                        team = gameManager.localPlayerTeam;
                    }
                    else
                        team = 1 - gameManager.localPlayerTeam;
                }
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
        if (Input.GetMouseButtonDown(0) && clickBegan)
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

        if (!Input.GetMouseButtonDown(0))
            clickBegan = true;
    }

    public void ReadyClicked()
    {
        CmdReadyClicked();
    }

    void OnGameStart()
    {
        if (!isServer)
            return;

        RpcOnGameStart();
    }

    [ClientRpc]
    void RpcOnGameStart()
    {
        readyButton.interactable = false;
        readyButton.gameObject.SetActive(false);
    }

    [Command]
    void CmdReadyClicked()
    {
        if (!isServer)
            return;

        gameManager.TeamReady(team, true);

        // For debug only
        //gameManager.TeamReady(1 - team, true);
    }

    [ClientRpc]
    void RpcDestroyTree(GameObject tree)
    {
        TreeBehaviour tb = tree.GetComponent<TreeBehaviour>();
        tb.DestroyTree();
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
