using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableAttribute : MonoBehaviour
{
    public Attribute attributeLabel;
    public Axis axis;
    // Start is called before the first frame update
    OVRGrabbable grabbable;
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        grabbable.OnReleased += (grab, pt) =>
        {
            //Make it parentless if it's still trapped in a selector
            if (transform.parent && transform.parent.GetComponent<AttributeSelector>()) {
                this.transform.parent = null;
                axis.transform.parent = null;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        //If it collides with a free design space snapper


        //If it collides with another attribute then create a design space with just that attribute on it 
        //oncollide(){

        //}
        //Hide the existing grabbable attribute's mesh

    }

}
