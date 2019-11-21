﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;
    public int numberOfTrees = 10;
    public Vector2 xRange;
    public Vector2 zRange;

    // Start is called before the first frame update
    void Start()
    {
        if (treePrefab)
        {
            for (int i = 0; i < numberOfTrees; i++)
            {
                Instantiate(treePrefab, new Vector3(Random.Range(xRange.x, xRange.y), 0, Random.Range(zRange.x, zRange.y)), Quaternion.identity, transform);
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = Color.yellow;

    //    Gizmos.DrawLine(transform.position + new Vector3(xRange.x, 0, zRange.x), transform.position + new Vector3(xRange.y, 0, zRange.x));
    //    Gizmos.DrawLine(transform.position + new Vector3(xRange.y, 0, zRange.x), transform.position + new Vector3(xRange.y, 0, zRange.y));
    //    Gizmos.DrawLine(transform.position + new Vector3(xRange.y, 0, zRange.y), transform.position + new Vector3(xRange.x, 0, zRange.y));
    //    Gizmos.DrawLine(transform.position + new Vector3(xRange.x, 0, zRange.y), transform.position + new Vector3(xRange.x, 0, zRange.x));
    //}
}
