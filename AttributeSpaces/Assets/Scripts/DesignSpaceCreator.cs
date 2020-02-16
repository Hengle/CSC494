using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignSpaceCreator : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four, controller)) {
            DesignSpace mainDesignSpace = DesignSpaceManager.instance.GetMainDesignSpace();
            Vector3 newLocation = panel.transform.position + new Vector3(0f, 0.05f, 0f);
            DesignSpace clonedDesignSpaceAxes = Instantiate(mainDesignSpace, newLocation, mainDesignSpace.transform.rotation, mainDesignSpace.transform.parent) as DesignSpace;
        }
    }
}
