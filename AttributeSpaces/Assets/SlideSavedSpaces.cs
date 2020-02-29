using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SlideSavedSpaces : MonoBehaviour
{
    SavedSpaceManager SavedSpaces;
    OVRGrabbable grabbable;
    bool grabbed = false;
    bool withinProximity = false;
    BoxCollider grabbableCollider;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        grabbableCollider = GetComponent<BoxCollider>();
        SavedSpaces = SavedSpaceManager.instance;
        grabbable.OnGrabbed += Grabbed;
        grabbable.OnReleased += Released;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Move each of the existing spaces over by a unit
        SavedSpaces.SavedSpacesCollection.transform.localPosition += new Vector3(transform.localPosition.x, 0.0f, 0.0f);
        transform.localRotation = Quaternion.identity;
       
        if (!GetComponent<OVRGrabbable>().isGrabbed)
        {
            //snap it back to 0
            transform.localPosition = new Vector3(0f, 0f, 0f);
            transform.localRotation = Quaternion.identity;
        }
        */


        if (grabbed)
        {
            SavedSpaces.SavedSpacesCollection.transform.position = grabbable.transform.position;
            SavedSpaces.SavedSpacesCollection.transform.localPosition = SavedSpaces.SavedSpacesCollection.transform.localPosition.SetY(0f);
            SavedSpaces.SavedSpacesCollection.transform.localPosition = SavedSpaces.SavedSpacesCollection.transform.localPosition.SetZ(0f);
        }
        else
        {
            //Snap back to the original position
            grabbable.transform.position = SavedSpaces.SavedSpacesCollection.transform.position + new Vector3(0f, 0.12f, 0f);
            grabbable.transform.rotation = SavedSpaces.SavedSpacesCollection.transform.rotation;

            grabbableCollider.center = grabbableCollider.center.SetX(-SavedSpaces.SavedSpacesCollection.transform.localPosition.x / grabbable.transform.localScale.x);
        }



    }

    void Grabbed(OVRGrabber hand, Collider collider)
    {

        grabbed = true;
    }

    void Released(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        grabbed = false;
    }
}
