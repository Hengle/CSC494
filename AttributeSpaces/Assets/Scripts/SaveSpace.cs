using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpace : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject WorkbenchParent;
    public List<DesignSpace> SavedSpaces = new List<DesignSpace>();
    public OVRGrabber grabHand1, grabHand2;

    public List<DesignSpace> hovering = new List<DesignSpace>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //If something collides with it,     
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller) > 0.0f){
            print("Greater than 0!!!!");
            for (int i = 0; i < SavedSpaces.Count; i++) {
                //Show all of the saved spaces beside the user in a line
                SavedSpaces[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                SavedSpaces[i].transform.position = WorkbenchParent.transform.position + new Vector3(0.3f, 0.3f, -((float)i - (float)i / 2.0f) * 0.6f);
                SavedSpaces[i].transform.localRotation = Quaternion.identity;
                
            }
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Add(designSpace);
        }

        //if (designSpace)
        //{
        //    designSpace.transform.localScale = designSpace.transform.localScale - new Vector3(0.1f, 0.1f, 0.1f);
            
        //    //Make it so that they aren't movable
        //    if (designSpace.x_attr) { designSpace.x_attr.Disable(); }
        //    if (designSpace.y_attr) { designSpace.y_attr.Disable(); }
        //    if (designSpace.z_attr) { designSpace.z_attr.Disable(); }

        //    designSpace.gameObject.transform.parent = WorkbenchParent.transform;

        //    designSpace.transform.localPosition = new Vector3(0f, 0.035f, -0.035f);

        //    SavedSpaces.Add(designSpace);
        //}
    }
    void OnTriggerExit(Collider collider)
    {
        
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Remove(designSpace);
        }

        //Bring it back up to size
        //if (designSpace)
        //{
        //    grabHand1.ForceRelease(designSpace.GetComponent<OVRGrabbable>());
        //    grabHand2.ForceRelease(designSpace.GetComponent<OVRGrabbable>());

        //    designSpace.transform.localScale = designSpace.transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
        //    if (designSpace.x_attr) { designSpace.x_attr.Enable(); }
        //    if (designSpace.y_attr) { designSpace.y_attr.Enable(); }
        //    if (designSpace.z_attr) { designSpace.z_attr.Enable(); }

        //    //Free it from the workbench
        //    designSpace.transform.parent = WorkbenchParent.transform.parent;

        //    SavedSpaces.Remove(designSpace);
        //}

    }
}
