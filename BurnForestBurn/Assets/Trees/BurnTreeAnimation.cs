using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[System.Obsolete]
public class BurnTreeAnimation : NetworkBehaviour
{
    public GameObject TreeTop;
    public Material DissolveMaterial;
    private Material m_dissolve;
    public float burnTime = 2f;
    private bool isBurning = false;
    private float dissolveAmount = 0;
    // Start is called before the first frame update
    void Awake()
    {
        m_dissolve = new Material(DissolveMaterial);
    }
    public void destroyAnimation()
    {

        TreeTop.GetComponent<Renderer>().material = m_dissolve;
        m_dissolve.SetFloat("_DissolveAmount", 1f);
        isBurning = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isBurning)
        {
            dissolveAmount = Mathf.Lerp(dissolveAmount, 1, Time.deltaTime / burnTime);
            m_dissolve.SetFloat("_DissolveAmount", dissolveAmount);
            if (dissolveAmount >= 0.9)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
