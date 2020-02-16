using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisSnapper : MonoBehaviour
{
    public float x_rotation;
    public Transform axisParent;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public OVRGrabber grabHand;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //When the collision occurs, parent it to the design space and then take the position and rotation of this item and copy it to the axis 
    void OnTriggerEnter(Collider collider)
    {
        //Release it!!!
        AxisConnector slider = collider.gameObject.GetComponent<AxisConnector>();

        if (slider)
        {
            grabHand.ForceRelease(slider.grabObject);
            //Reparent it to the axes collection
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.SetParent(axisParent);
            //Snap it to the right position
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.localPosition = originalPosition;
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.localRotation = originalRotation;
        }
    }
    void OnTriggerExit(Collider collider) {
        //axisParent the slider to the world space again
        AxisConnector slider = collider.gameObject.GetComponent<AxisConnector>();

        if (slider)
        {
            //grabHand.ForceRelease(slider.grabObject);
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.SetParent(axisParent.parent.parent);
        }
    }
}
