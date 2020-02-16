using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisSnapper : MonoBehaviour
{
    public AxisManager Axes;
    public Transform axisParent;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public OVRGrabber grabHand;
    //The design space that these colliders are part of
    public int axisOrdering;

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
        AxisConnector axisConnector = collider.gameObject.GetComponent<AxisConnector>();

        if (axisConnector)
        {
            GameObject slider = axisConnector.sliderObject;
            //Release it!!!
            grabHand.ForceRelease(axisConnector.grabObject);
            //Reparent it to the axes collection
            slider.transform.SetParent(axisParent);
            //Snap it to the right position
            slider.transform.localPosition = originalPosition;
            slider.transform.localRotation = originalRotation;

            //Now tell the parent which axis was updated
            if (axisOrdering == 0){
                Axes.UpdateXAxis(slider.GetComponent<Axis>());
                print("It was X");
            }
            else if (axisOrdering == 1){
                Axes.UpdateYAxis(slider.GetComponent<Axis>());
                print("It was Y");
            }
            else if (axisOrdering == 2) {
                Axes.UpdateZAxis(slider.GetComponent<Axis>());
                print("It was Z");
            }
        }


    }
    void OnTriggerExit(Collider collider) {
        //axisParent the slider to the world space again
        AxisConnector slider = collider.gameObject.GetComponent<AxisConnector>();

        if (slider)
        {
            if (axisOrdering == 0)
            {
                Axes.RemoveXAxis(slider.GetComponent<Axis>());
            }
            else if (axisOrdering == 1)
            {
                Axes.RemoveYAxis(slider.GetComponent<Axis>());
            }
            else if (axisOrdering == 2)
            {
                Axes.RemoveZAxis(slider.GetComponent<Axis>());
            }

            //grabHand.ForceRelease(slider.grabObject);
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.SetParent(axisParent.parent.parent);
        }
    }
}
