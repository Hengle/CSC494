using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePanelVisibilityControl : MonoBehaviour
{
    public GameObject SpacePanel;
    public OVRInput.Controller controller;

    static SavedSpaceManager savedSpaceManager;

    // Start is called before the first frame update
    void Start()
    {
        savedSpaceManager = SavedSpaceManager.instance;
        SpacePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Three, controller) || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.0f)
        {
            //TODO take out everything with TEMP in it 
            savedSpaceManager.UpdateSpaceContents();
            SpacePanel.gameObject.SetActive(true);
        }
        else {
            SpacePanel.gameObject.SetActive(false);
        }
    }
}
