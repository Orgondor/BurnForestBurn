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
    private int team = -1;
    public delegate void OnAreaDone(int team);
    public delegate void RegisterTeamArea(int team);
    private OnAreaDone onAreaDone;
    private RegisterTeamArea registerTeamArea;

    public void SetOnAreaDone(OnAreaDone oad)
    {
        onAreaDone = oad;
    }

    public void SetOnRegisterTeamArea(RegisterTeamArea rta)
    {
        registerTeamArea = rta;
    }

    public void SpawnTrees()
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
                team = tb.team;
                NetworkServer.Spawn(tree);
            }

            if (team >= 0)
            {
                registerTeamArea(team);
            }
            else
                Debug.LogError("No team found for area!");
        }
    }

    public void treeHarvested()
    {
        numHarvested++;
        Debug.LogFormat("Tree Harvested {0}/{1}", numHarvested, numberOfTrees);

        if (numHarvested == numberOfTrees)
        {
            areaDoneObject.SetActive(true);

            if (isServer)
            {
                onAreaDone(team);
            }
        }
    }
}
