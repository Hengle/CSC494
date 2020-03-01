using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePanelVisibilityControl : MonoBehaviour
{
    public GameObject SpacePanel;
    public OVRInput.Controller controller;

    static SavedSpaceManager savedSpaceManager;

    public Attribute TEMPAttr;
    public List<Attribute> TEMPList;

    // Start is called before the first frame update
    void Start()
    {
        TEMPList = new List<Attribute>();
        TEMPList.Add(TEMPAttr);
        savedSpaceManager = SavedSpaceManager.instance;
        SpacePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.0f)
        {
            savedSpaceManager.UpdateSpaceContents(TEMPList);
            SpacePanel.gameObject.SetActive(true);
        }
        else {
            SpacePanel.gameObject.SetActive(false);
        }
    }
}
