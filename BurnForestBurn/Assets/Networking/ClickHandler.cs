using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class ClickHandler : NetworkBehaviour
{
    private int team = -1;
    private BFBGameManager gameManager = null;

    // For debug on PC
    private bool clickBegan = true;

    bool foundTag = false;
    bool gotGameManager = false;

    void Start()
    {
        if (isServer)
        {
            GameObject temp = GameObject.Find("GameManager");
            if (temp)
            {
                gameManager = temp.GetComponent<BFBGameManager>();
                foundTag = true;
                if (gameManager)
                {
                    gotGameManager = true;
                    if (isLocalPlayer)
                    {
                        gameManager.localPlayerTeam = Random.Range(0, 2);
                        team = gameManager.localPlayerTeam;
                    }
                    else
                        team = 1 - gameManager.localPlayerTeam;
                }
            }

            RpcDebugStart(foundTag, gotGameManager);
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

    [ClientRpc]
    void RpcDebugStart(bool foundTag, bool gotGameManager)
    {
        Debug.LogFormat("Start - foundTag: {0} gotGameManager: {1}", foundTag, gotGameManager);
    }

    [ClientRpc]
    void RpcSetTeam(int teamId)
    {
        if (isLocalPlayer)
        {
            Debug.LogFormat("Local!!!", teamId);
        }
        Debug.LogFormat("team set {0}", teamId);
        team = teamId;
    }

    [Command]
    void CmdSetTeam()
    {
        if (!isServer)
            return;

        team = 1 - gameManager.localPlayerTeam;
        RpcSetTeam(team);
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
            Debug.Log("Destroying tree");
            RpcDestroyTree(tree);
        }
        else
            Debug.Log("tapped tree for other team");
    }
}
