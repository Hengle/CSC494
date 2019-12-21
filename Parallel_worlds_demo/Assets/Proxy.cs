using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Proxy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject original; // Fish

    OVRGrabbable grabbable;
    Transform parent;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();

        GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
        GetComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;

        parent = transform.parent;

        //Change this to use the bounding box instead of the local scale!!! Not sure how to get a rotation-invariant bounding box though
        //transform.localPosition = original.transform.localScale;
        transform.localPosition = original.GetComponent<MeshRenderer>().bounds.size;
        transform.localScale = original.transform.localScale;
        transform.localRotation = original.transform.localRotation;
        print(original.transform.localScale);

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
        //0.2 is hardcoded so that the representations aren't too big
        SetGlobalScale(original.transform.localScale * 0.2f);

        Vector3 pos = parent.InverseTransformPoint(transform.position);

        //The issue is that the bounding box is overriding the true scale of the object! The coordinates lost information by switching to the bounding box. You should manipulate it based on the bounding box size
        original.transform.localScale = pos;
        //use the bounding box size to scale the object?...

    }
}
