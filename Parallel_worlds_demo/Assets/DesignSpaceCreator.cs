using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignSpaceCreator : MonoBehaviour
{
    public OVRInput.Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four, controller)) {
            Vector3 offset = new Vector3(0.1f, 0.0f, 0.1f);
            DesignSpace mainDesignSpace = DesignSpaceManager.instance.GetMainDesignSpace();
            GameObject designSpaceAxes = mainDesignSpace.axis;
            GameObject clonedDesignSpaceAxes = Instantiate(designSpaceAxes, designSpaceAxes.transform.localPosition + offset, designSpaceAxes.transform.localRotation) as GameObject;
            clonedDesignSpaceAxes.transform.parent = designSpaceAxes.transform.parent.parent;

            DesignSpace clonedDesignSpace = new DesignSpace();
            clonedDesignSpace.axis = clonedDesignSpaceAxes;
            clonedDesignSpace.x_attr = mainDesignSpace.x_attr;
            clonedDesignSpace.y_attr = mainDesignSpace.y_attr;
            clonedDesignSpace.z_attr = mainDesignSpace.z_attr;
            clonedDesignSpace.axis = mainDesignSpace.axis;
            clonedDesignSpace._gameObjectList = mainDesignSpace._gameObjectList;
            clonedDesignSpace.proxyPrefab = mainDesignSpace.proxyPrefab;

            //Now Add this design space axis to the list of design spaces
            DesignSpaceManager.instance.AddDesignSpaceToList(clonedDesignSpace);

        }
    }
}
