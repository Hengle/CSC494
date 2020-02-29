using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePanelVisibilityControl : MonoBehaviour
{
    public GameObject SpacePanel;
    public OVRInput.Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        SpacePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.0f)
        {
            //this.transform.parent.gameObject.SetActive(true);
            SpacePanel.gameObject.SetActive(true);
        }
        else {
            SpacePanel.gameObject.SetActive(false);
        }
    }
}
