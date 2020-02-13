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
            DesignSpace clonedDesignSpaceAxes = Instantiate(mainDesignSpace, panel.transform.position, mainDesignSpace.transform.rotation, mainDesignSpace.transform.parent) as DesignSpace;
        }
    }
}
