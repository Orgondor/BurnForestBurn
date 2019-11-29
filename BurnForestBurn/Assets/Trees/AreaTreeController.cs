using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class AreaTreeController : NetworkBehaviour
{
    public GameObject treePrefab;
    public int numberOfTrees = 10;
    public Transform areaTransform;
    public GameObject areaDoneObject;
    public float imageTargetScale = 6.7f;
    private int numHarvested = 0;
    
    void Start()
    {
        if (isServer && treePrefab)
        {
            for (int i = 0; i < numberOfTrees; i++)
            {
                float x = areaTransform.position.x + imageTargetScale * Random.Range(-areaTransform.localScale.x * 0.5f, areaTransform.localScale.x * 0.5f);
                float z = areaTransform.position.z + imageTargetScale * Random.Range(-areaTransform.localScale.z * 0.5f, areaTransform.localScale.z * 0.5f);
                GameObject tree = Instantiate(treePrefab, new Vector3(x, 0, z), Quaternion.identity);
                TreeBehaviour tb = tree.GetComponent<TreeBehaviour>();
                tb.parentNetId = netId;
                tb.atc = this;
                NetworkServer.Spawn(tree);
            }
        }
    }

    public void treeHarvested()
    {
        numHarvested++;
        Debug.Log("Tree Harvested");

        if (numHarvested == numberOfTrees)
        {
            areaDoneObject.SetActive(true);
        }
    }
}
