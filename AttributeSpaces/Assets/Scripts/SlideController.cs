using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SlideController : MonoBehaviour
{
    public GameObject objToFollow;
    public GameObject AttributeSpaceCollection;
    OVRGrabbable grabbable;
    bool grabbed = false;
    BoxCollider grabbableCollider;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        grabbableCollider = GetComponent<BoxCollider>();
        grabbable.OnGrabbed += Grabbed;
        grabbable.OnReleased += Released;
    }

    // Update is called once per frame
    void Update()
    {

        if (grabbed)
        {
            AttributeSpaceCollection.transform.position = grabbable.transform.position;
            AttributeSpaceCollection.transform.localPosition = AttributeSpaceCollection.transform.localPosition.SetY(0f);
            AttributeSpaceCollection.transform.localPosition = AttributeSpaceCollection.transform.localPosition.SetZ(0f);
        }
        else
        {
            //Snap back to the original position
            grabbable.transform.position = AttributeSpaceCollection.transform.position + new Vector3(0f, -0.009f, 0.077f);
            grabbableCollider.center = grabbableCollider.center.SetX(-AttributeSpaceCollection.transform.localPosition.x / grabbable.transform.localScale.x);
        }



    }

    void Grabbed(OVRGrabber hand, Collider collider)
    {

        grabbed = true;
    }

    void Released(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        grabbed = false;
        grabbable.transform.rotation = new Quaternion(0.0f, objToFollow.transform.rotation.y, 0.0f, objToFollow.transform.rotation.w);
    }
}
