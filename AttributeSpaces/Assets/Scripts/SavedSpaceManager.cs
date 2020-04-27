using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


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

    public GameObject pastMarker;

    private void Awake()
    {
        _SavedSpaceManagerInstance = this;
        filterManager = FilterManager.instance;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        //If you press the button while holding a space, show all of the past saved space locations
        if (OVRInput.GetDown(OVRInput.Button.Three, controller))
        {
            DesignSpace mainSpace = DesignSpaceManager.instance.GetMainDesignSpace();
            if (mainSpace) {
                drawLocations(mainSpace);
            }
        }
        */
    }
    void OnTriggerEnter(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Add(designSpace);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Remove(designSpace);
        }
    }

    public void SaveSpace(DesignSpace designSpace, bool alwaysActive = false) {

        DesignSpace clone = (DesignSpace)Instantiate(designSpace, transform.parent.localPosition + new Vector3(0f, 0f, -0.1f), transform.parent.localRotation, SavedSpacesCollection.transform);
        clone.originalSpaceIndex = designSpace.originalSpaceIndex;
        clone.isSaveClone = true;
        clone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        clone.directAncestorID = designSpace.DesignSpaceID;

        //Hide the object if it doesn't pass the filter
        if (filterManager.MatchesCriteria(clone) == false & !alwaysActive) {
            clone.gameObject.SetActive(false);
        }

        SavedSpaces.Add(clone);
        SavedSpaceLocations.Add(clone.transform.localPosition);
        //update the position of everything on the table right now and shift it to the left. The most recent things is in the centre by default
        UpdateSpaceContents();

        //For every existing item in the timeline, 
        designSpace.directDescendents.Add(clone);
    }




    private void LateUpdate()
    {
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
                DS.gameObject.SetActive(false);
            }
        }

        //Move the spaces to the right locations on the workbench
        for (int i = 0; i < SavedSpaces.Count; i++)
        {
            if (SavedSpaces[i].gameObject.activeSelf)
            {

                Vector3 newLocation;
                //Show the progression view on a button click
                //if (OVRInput.Get(OVRInput.Button.Three, controller))
                //{
                //   newLocation = new Vector3(0.25f * (float)(activeSpaceCounter), SavedSpaces[i].transform.localPosition.y, 0.0f);
                //}
                //else {
                    newLocation = new Vector3(0.25f * (float)(activeSpaceCounter), 0.0f, 0.0f);
                //}
                
                SavedSpaces[i].transform.AnimateLocalPosition(newLocation);
                SavedSpaces[i].AnimateLocalRotation(SavedSpaceManager.instance.transform.parent.localRotation);
                activeSpaceCounter -= 1;
            }

            SavedSpaceLocations[i] = SavedSpaces[i].transform.localPosition;
        }
    }
}
