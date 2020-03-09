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
            savedSpaceManager.SavedSpaces.Add(clonedDesignSpaceAxes);
            savedSpaceManager.UpdateSpaceContents();
        }
    }
}
