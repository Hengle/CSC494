using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableAttribute : MonoBehaviour
{
    public Attribute attributeLabel;
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
            }
            
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

}
