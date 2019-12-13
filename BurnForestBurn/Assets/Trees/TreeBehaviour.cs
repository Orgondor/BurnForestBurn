using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TreeBehaviour : NetworkBehaviour
{
    public AreaTreeController atc;

    [SyncVar]
    public NetworkInstanceId parentNetId;

    public int team = 0;

    public override void OnStartClient()
    {
        GameObject parentObject = ClientScene.FindLocalObject(parentNetId);

        atc = parentObject.GetComponent<AreaTreeController>();
    }

    public void DestroyTree()
    {
        if (atc)
        {
            atc.treeHarvested();
            Debug.Log(gameObject.name);
            if (gameObject.name == "Leaftree(Clone)") 
            {
                gameObject.GetComponent<BurnTreeAnimation>().destroyAnimation();
            }
            else { 
            gameObject.SetActive(false);
            }
        }
    }
}
