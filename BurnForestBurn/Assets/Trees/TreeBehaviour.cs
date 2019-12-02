using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TreeBehaviour : NetworkBehaviour
{
    public AreaTreeController atc;
    private BurnTreeAnimation bt;
    [SyncVar]
    public NetworkInstanceId parentNetId;

    public override void OnStartClient()
    {
        bt = gameObject.GetComponent<BurnTreeAnimation>();
        GameObject parentObject = ClientScene.FindLocalObject(parentNetId);

        atc = parentObject.GetComponent<AreaTreeController>();
    }

    public void DestroyTree()
    {
        Debug.Log("Destroy tree!");

        if (atc)
        {
            atc.treeHarvested();
            bt.destroyAnimation();
            
            //gameObject.SetActive(false);
            
        }
    }
}
