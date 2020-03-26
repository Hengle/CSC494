using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignSpaceCreator : MonoBehaviour
{
    public OVRInput.Controller controller;
    public DesignSpace DSTemplate;
    SavedSpaceManager savedSpaceManager;
    // Start is called before the first frame update
    void Start()
    {
        savedSpaceManager = SavedSpaceManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Create a completely empty attribute space on the saved spaces list
        if (OVRInput.GetDown(OVRInput.Button.Four, controller)) {
            DesignSpace clonedDesignSpaceAxes = Instantiate(DSTemplate, new Vector3(0f, 0f, 0f), Quaternion.identity, savedSpaceManager.SavedSpacesCollection.transform);
            
            clonedDesignSpaceAxes.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            clonedDesignSpaceAxes.gameObject.SetActive(true);
            //This space is its own original (will show up as its own row in the table)
            clonedDesignSpaceAxes.originalSpaceIndex = savedSpaceManager.SavedSpaces.Count;
            savedSpaceManager.SavedSpaces.Add(clonedDesignSpaceAxes);
            savedSpaceManager.SavedSpaceLocations.Add(clonedDesignSpaceAxes.transform.localPosition);
            savedSpaceManager.UpdateSpaceContents();
        }
    }
}
