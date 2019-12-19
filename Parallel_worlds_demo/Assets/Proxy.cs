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

        transform.localPosition = original.transform.localScale;


        //instead, maybe have a list of objects in another class and as you go through that list of objects, you create a proxy in the given axis object with the right parts for each object. 
        //later change this to just having the selector change the items in that list

        /*


        */

        /*
        grabbable = GetComponent<OVRGrabbable>();

        GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
        GetComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;
        //var cubeRenderer = proxy.GetComponent<Renderer>();

        parent = transform.parent;
        //initialize the point to the position that matches up with where the item is in space
        transform.localPosition = parent.InverseTransformPoint(original.transform.position);
        */
    }
    public void SetGlobalScale(Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.000001f),
                                            globalScale.y / Mathf.Max(Mathf.Abs(transform.lossyScale.y), 0.000001f),
                                            globalScale.z / Mathf.Max(Mathf.Abs(transform.lossyScale.z), 0.000001f));
    }

    // Update is called once per frame
    void Update()
    {
        //Get the Renderer component from the new cube

        SetGlobalScale(original.transform.localScale * 0.2f);

        Vector3 pos = parent.InverseTransformPoint(transform.position);

        original.transform.localScale = pos;
        
        //transform.localPosition = parent.InverseTransformPoint(original.transform.position);

        //InverseTransformPoint takes it from world space to local space
        //original.transform.localPosition = parent.InverseTransformPoint(transform.localPosition);
    }
}
