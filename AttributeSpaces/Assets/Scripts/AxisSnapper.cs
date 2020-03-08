using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class AxisSnapper : MonoBehaviour
{
    public AxisManager Axes;
    public Transform axisParent;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public OVRGrabber grabHand;
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
            Transform originalGrabbableAttribute = axisConnector.transform;

            //TODO:
            //Check that the axis is compatible with the objects and the rest of the axes

            //Now tell the parent which axis was updated
            if (axisOrdering == 0 && !Axes.DS.x_attr)
            {
                GameObject slider = axisConnector.sliderObject;
                //Release it!!!
                grabHand.ForceRelease(axisConnector.grabObject);
                //Reparent it to the axes collection
                slider.transform.SetParent(axisParent);

                //Slowly have the object float to the right position
                slider.transform.AnimateLocalPosition(originalPosition);
                slider.transform.AnimateLocalRotation(originalRotation);

                slider.GetComponent<Axis>().orientation = 0;
                Axes.UpdateXAxis(slider.GetComponent<Axis>());

                //Hide the old label and show the axis
                originalGrabbableAttribute.gameObject.SetActive(false);
                axisConnector.sliderObject.SetActive(true);

            }
            else if (axisOrdering == 1 && !Axes.DS.y_attr)
            {
                GameObject slider = axisConnector.sliderObject;
                //Release it!!!
                grabHand.ForceRelease(axisConnector.grabObject);
                //Reparent it to the axes collection
                slider.transform.SetParent(axisParent);

                //Slowly have the object float to the right position
                slider.transform.AnimateLocalPosition(originalPosition);
                slider.transform.AnimateLocalRotation(originalRotation);

                slider.GetComponent<Axis>().orientation = 1;
                Axes.UpdateYAxis(slider.GetComponent<Axis>());

                //Hide the old label and show the axis
                originalGrabbableAttribute.gameObject.SetActive(false);
                axisConnector.sliderObject.SetActive(true);

            }
            else if (axisOrdering == 2 && !Axes.DS.z_attr)
            {
                GameObject slider = axisConnector.sliderObject;
                //Release it!!!
                grabHand.ForceRelease(axisConnector.grabObject);
                //Reparent it to the axes collection
                slider.transform.SetParent(axisParent);

                //Slowly have the object float to the right position
                slider.transform.AnimateLocalPosition(originalPosition);
                slider.transform.AnimateLocalRotation(originalRotation);
                slider.GetComponent<Axis>().orientation = 2;
                Axes.UpdateZAxis(slider.GetComponent<Axis>());

                //Hide the old label and show the axis
                originalGrabbableAttribute.gameObject.SetActive(false);
                axisConnector.sliderObject.SetActive(true);
            }
            /*
            //Disable the collider once it snaps on 
            if (this.GetComponent<BoxCollider>())
            {
                this.GetComponent<BoxCollider>().enabled = false;
            }
            if (this.GetComponent<SphereCollider>())
            {
                this.GetComponent<SphereCollider>().enabled = false;
            }
            */


        }


    }
    void OnTriggerExit(Collider collider) {

        //controlGrabbable will be the collider that exits
        //print(collider.name);

        //axisParent the slider to the world space again
        AxisConnector axisConnector = collider.gameObject.GetComponent<AxisConnector>();
        Axis axis = collider.gameObject.GetComponent<Axis>();

        if (axisConnector && axis)
        {
            print("axis exists");
            if (axisOrdering == 0)
            {
                Axes.RemoveXAxis(axisConnector.GetComponent<Axis>());
            }
            else if (axisOrdering == 1)
            {
                Axes.RemoveYAxis(axisConnector.GetComponent<Axis>());
            }
            else if (axisOrdering == 2)
            {
                Axes.RemoveZAxis(axisConnector.GetComponent<Axis>());
            }

            //grabHand.ForceRelease(slider.grabObject);
            collider.gameObject.GetComponent<AxisConnector>().sliderObject.transform.parent = null;
        }
    }
}
