using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class ClickHandler : NetworkBehaviour
{

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
    }

    [ClientRpc]
    void RpcDestroyTree(GameObject tree)
    {

        Debug.Log("RpcDestroyTree");
        TreeBehaviour tb = tree.GetComponent<TreeBehaviour>();
        tb.DestroyTree();
    }

    [Command]
    void CmdDestroyTree(GameObject tree)
    {
        if (!isServer)
            return;

        Debug.Log("CmdDestroyTree");
        RpcDestroyTree(tree);
    }
}
