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

    //Store the position so that it can be updated every frame??
    Vector3 past_position;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        if (GetComponent<MeshRenderer>() && original.GetComponent<MeshRenderer>())
        {
            GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
            GetComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;
            GetComponent<MeshCollider>().sharedMesh = original.GetComponent<MeshFilter>().mesh;
        }
        parent = transform.parent;
        if (original.GetComponent<MeshRenderer>()) { 
            //Change this to use the bounding box instead of the local scale!!! Not sure how to get a rotation-invariant bounding box though
            //transform.localPosition = original.transform.localScale;

            transform.localPosition = original.GetComponent<MeshRenderer>().bounds.size;
            transform.localScale = original.transform.localScale;
            transform.localRotation = original.transform.localRotation;
            past_position = transform.localPosition;
        }
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

        //Only call this if the parent is the main design space
        if (DesignSpaceManager.instance.GetMainDesignSpace().transform == this.parent.transform) {
            //0.5 is hardcoded so that the representations aren't too big
            SetGlobalScale(original.transform.localScale * 0.1f);

            Vector3 pos = parent.InverseTransformPoint(transform.position);

            //OR store the delta of the bounding box transform and change the original object by the same amount
            Vector3 newPos = transform.localPosition - past_position;
            original.transform.localScale += newPos;

            //Store the delta for how much it's moving and then use that to change the size
            //This is to avoid having the bounding box mess up the scale of the final object
            past_position = transform.localPosition;
        }
    }
}
