using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utilities;
using MeshVoxelizerUtil;

public class Proxy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject original; // Fish

    //Back up the true original of the object
    [NonSerialized] public GameObject original_backup; 

    OVRGrabbable grabbable;
    Transform parent;

    public DesignSpace parentSpace;
    //Store the position so that it can be updated every frame??
    public Vector3 past_position;

    //Store the initial location of the proxy
    public Vector3 originalLocation;

    public bool isVoxel;
    void Start()
    {
        if (gameObject.GetComponent<Voxel>())
        {
            isVoxel = true;
            originalLocation = transform.localPosition;
            past_position = transform.localPosition;
        }
        else
        {
            isVoxel = false;
            //Save the original object so that you can go back to it, but disable the copy so that it doesn't show
            original_backup = Instantiate(original, original.transform.position, original.transform.rotation);
            original_backup.SetActive(false);

            grabbable = GetComponent<OVRGrabbable>();
            if (GetComponent<MeshRenderer>() && original.GetComponent<MeshRenderer>())
            {
                GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
                GetComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;
                if (GetComponent<MeshCollider>())
                {
                    GetComponent<MeshCollider>().sharedMesh = original.GetComponent<MeshFilter>().mesh;
                }
            }

            transform.localRotation = original.transform.localRotation;
            originalLocation = transform.localPosition;
            past_position = transform.localPosition;
            //Set up how the proxy should look at the very beginning
            //---------------------
            //0.1 is hardcoded so that the representations aren't too big
            SetGlobalScale(original.transform.localScale * 0.1f);
            //------

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
    void LateUpdate()
    {
       
        if (isVoxel)
        {
            /*
            //Update the colour!!!
            //TODO
            getCurrentValue(original);
                //Get the x attr's attribute and then set the mesh material to be that attribute's current value
            if (parentSpace.x_attr) { GetComponent<MeshRenderer>().material.SetFloat("_DeltaRed", parentSpace.x_attr.currentValue);  }
            if (parentSpace.y_attr) { GetComponent<MeshRenderer>().material.SetFloat("_DeltaGreen", parentSpace.y_attr.currentValue); }
            if (parentSpace.z_attr) { GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", parentSpace.z_attr.currentValue); }
            */
        }
       
        //Basically animates the interaction
        //Only call this if the parent is the main design space
        else
        {
            if (DesignSpaceManager.instance.GetMainDesignSpace().transform == this.transform.parent)
            {
                //0.1 is hardcoded so that the representations aren't too big
                SetGlobalScale(original.transform.localScale * 0.1f);

                Vector3 pos = transform.parent.InverseTransformPoint(transform.position);

                //OR store the delta of the bounding box transform and change the original object by the same amount
                Vector3 newPos = transform.localPosition - past_position;

                //TODO you need to update the original object based on where the proxy is!!!
                //original.transform.localScale += newPos;
                if (parentSpace.x_attr) { parentSpace.x_attr.attribute.applyAttributeChange(this, parentSpace.x_attr, 0, parentSpace.x_attr.currentValue); }
                if (parentSpace.y_attr) { parentSpace.y_attr.attribute.applyAttributeChange(this, parentSpace.y_attr, 1, parentSpace.y_attr.currentValue); }
                if (parentSpace.z_attr) { parentSpace.z_attr.attribute.applyAttributeChange(this, parentSpace.z_attr, 2, parentSpace.z_attr.currentValue); }


                //Store the delta for how much it's moving and then use that to change the size
                //This is to avoid having the bounding box mess up the scale of the final object
                past_position = transform.localPosition;
            }
        }


    }
}
