using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedSpaceManager : MonoBehaviour
{
    static SavedSpaceManager _SavedSpaceManagerInstance;
    public static SavedSpaceManager instance { get => _SavedSpaceManagerInstance ?? FindObjectOfType<SavedSpaceManager>(); }

    public OVRInput.Controller controller;
    public GameObject WorkbenchParent;
    public List<DesignSpace> SavedSpaces = new List<DesignSpace>();
    public List<Attribute> filterAttributes = new List<Attribute>();
    public List<GameObject> filterObjects = new List<GameObject>();


    public OVRGrabber grabHand1, grabHand2;

    public List<Vector3> SavedSpaceLocations = new List<Vector3>();

    public List<DesignSpace> hovering = new List<DesignSpace>();

    public GameObject SavedSpacesCollection;

    public BoxCollider SlideGrabbable;

    FilterManager filterManager;

    private void Awake()
    {
        _SavedSpaceManagerInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        filterManager = FilterManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

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
    public void SaveSpace(DesignSpace designSpace) {

        //Move all the existing spaces down
        for (int i = 0; i < SavedSpaces.Count; i++) {
            //Move each of the existing spaces over by a unit
            SavedSpaces[i].transform.localPosition += new Vector3(0.4f, 0.0f, 0.0f);
        }

        DesignSpace clone = Instantiate(designSpace, transform.parent.localPosition + new Vector3(0f, 0f, -0.1f), transform.parent.localRotation, SavedSpacesCollection.transform) as DesignSpace;
        clone.isSaveClone = true;

        //Disable the colliders for the axes
        foreach (Transform child in clone.axisManager.transform) {
            child.gameObject.GetComponent<Collider>().enabled = false;
        }
        if (clone.x_attr) { clone.x_attr.GetComponent<Collider>().enabled = false; }
        if (clone.y_attr) { clone.y_attr.GetComponent<Collider>().enabled = false; }
        if (clone.z_attr) { clone.z_attr.GetComponent<Collider>().enabled = false; }

        //Hide the object if it doesn't pass the filter
        if (!filterManager.MatchesCriteria(clone)) {
            clone.gameObject.SetActive(false);
        }
        SavedSpaces.Add(clone);

        
        //update the position of everything on the table right now and shift it to the left. The most recent things is in the centre by default
    }
    private void LateUpdate()
    {
        SlideGrabbable.transform.rotation = Quaternion.identity;
        //Constrain it to only move on the x axis
        SavedSpacesCollection.transform.localPosition = new Vector3(SavedSpacesCollection.transform.localPosition.x, 0f, 0f);
    }

    //Update which items are active and inactive and then fix their locations in space
    public void UpdateSpaceContents()
    {
        int activeSpaceCounter = 0;
        foreach (DesignSpace DS in SavedSpaces)
        {
            if (filterManager.MatchesCriteria(DS) == true)
            {
                DS.gameObject.SetActive(true);
                activeSpaceCounter += 1;
            }
            else
            {
                DS.gameObject.SetActive(true);
            }
        }

        //Move the spaces to the right locations on the workbench
        for (int i = 0; i < SavedSpaces.Count; i++)
        {
            if (SavedSpaces[i].gameObject.activeSelf)
            {
                SavedSpaces[i].transform.localPosition = new Vector3(-0.4f * (float)(activeSpaceCounter), 0.0f, 0.0f);
                activeSpaceCounter -= 1;
            }
        }
    }
}
