using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject tree;
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit rayhit = new RaycastHit();

            if (Physics.Raycast(ray, out rayhit))
            {
                tree = GameObject.Find("Tree1");

                GameObject hitObject = rayhit.collider.gameObject;

                //Fetch the Renderer from the GameObject
                Renderer rend = hitObject.GetComponent<Renderer>();

                tree.active = false;
                rend.enabled = false;
            }

        }

    }
}
