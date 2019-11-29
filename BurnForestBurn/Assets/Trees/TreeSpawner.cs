using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class TreeSpawner : NetworkBehaviour
{
    public GameObject treePrefab;
    public int numberOfTrees = 10;
    public Vector2 xRange;
    public Vector2 zRange;

    // Start is called before the first frame update
    void Start()
    {

        if (isServer)
        {
            for (int i = 0; i < numberOfTrees; i++)
            {
                GameObject tree = Instantiate(treePrefab, new Vector3(Random.Range(xRange.x, xRange.y), 0, Random.Range(zRange.x, zRange.y)), Quaternion.identity, transform);
                NetworkServer.Spawn(tree);
            }
        }
    }
}
