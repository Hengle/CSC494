using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Proxy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject original; // Fish
    public Transform follow;

    OVRGrabbable grabbable;
    Transform parent;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();

        GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
        GetComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;

        //var cubeRenderer = proxy.GetComponent<Renderer>();

        parent = transform.parent;
    }


    // Update is called once per frame
    void Update()
    {
        //Get the Renderer component from the new cube
        
        //This is only updating on release
        //var x = proxy.transform.localPosition.x;
        //var y = proxy.transform.localPosition.y;
        //var z = proxy.transform.localPosition.x;

        Vector3 pos = parent.InverseTransformPoint(transform.position);

        original.transform.localScale = pos*2.0f;
    }
}
