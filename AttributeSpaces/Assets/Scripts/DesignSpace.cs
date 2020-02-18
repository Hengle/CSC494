using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class DesignSpace : MonoBehaviour
{
    public Axis x_attr;
    public Axis y_attr;
    public Axis z_attr;
    public GameObject spaceBox;
    //The axis contains the xyz axes and also the different proxy points as children 
    public List<GameObject> _gameObjectList;
    public GameObject proxyPrefab;
    public OVRGrabbable controlCube;
    //List of the objects that were used to make each voxel (same length as the voxels)
    public List<GameObject> voxelOriginals;


    float speed;

    List<Vector3> colors = new List<Vector3>();

    public GameObject magnetParent;
    public GameObject constraintParent;

    List<Transform> voxels = new List<Transform>();
    public List<Proxy> proxyList = new List<Proxy>();
    List<GameObject> magnetList = new List<GameObject>();
    List<GameObject> constraintList = new List<GameObject>();

    public bool isMainSpace;

    Vector3 controlCubeLocation;

    public GameObject originCube;
    //Adding the highlight as disabled to begin with
    //Outline outline;

    public int DesignSpaceID;
    // Start is called before the first frame update
    private void Start()
    {
        //Add itself to the Design Space Manager's list of objects
        DesignSpaceID = DesignSpaceManager.instance.AddDesignSpaceToList(this);

        controlCubeLocation = controlCube.transform.position;

        //Set it to be tiny by default
        //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (originCube.transform.GetComponent<MeshRenderer>())
        {
            originCube.transform.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        //Add all the magnets and constraints to the list
        foreach (Transform child in magnetParent.transform) {
            magnetList.Add(child.gameObject);
        }
        foreach (Transform child in constraintParent.transform)
        {
            constraintList.Add(child.gameObject);
        }

        isMainSpace = false;
    }


    // Update is called once per frame BUT ONLY IF IT'S A MONOBEHAVIOUR!!
    public void Update()
    {
        //Update the slider location based on where the box is (but only the one component that changed)                            
        //The range for the slider goes from -0.5 to 0.5 so you need to add an offset
        if (controlCube.isGrabbed)
        {
            if (x_attr)
            {
                x_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.x - 0.5f, 0f, 0f);
                x_attr.UpdateMax();
            }
            if (y_attr) {
                y_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.y - 0.5f, 0f, 0f);
                y_attr.UpdateMax();
            }
            if (z_attr) {
                z_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.z - 0.5f, 0f, 0f);
                z_attr.UpdateMax();
            }
        }
        if ((x_attr && x_attr.controlGrabbable.isGrabbed) || (y_attr && y_attr.controlGrabbable.isGrabbed) || (z_attr && z_attr.controlGrabbable.isGrabbed))
        {
            if (x_attr)
            {
                controlCube.transform.localPosition = controlCube.transform.localPosition.SetX(x_attr.currentValue);
            }
            if (y_attr)
            {
                controlCube.transform.localPosition = controlCube.transform.localPosition.SetY(y_attr.currentValue);
            }
            if (z_attr)
            {
                controlCube.transform.localPosition = controlCube.transform.localPosition.SetZ(z_attr.currentValue);
            }
        }
        else { // default case is to update based on the cube location

            if (x_attr)
            {
                x_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.x - 0.5f, 0f, 0f);
                x_attr.UpdateMax();
            }
            if (y_attr)
            {
                y_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.y - 0.5f, 0f, 0f);
                y_attr.UpdateMax();
            }
            if (z_attr)
            {
                z_attr.controlGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.z - 0.5f, 0f, 0f);
                z_attr.UpdateMax();
            }
        }


        if ((controlCube.isGrabbed) && (controlCubeLocation != controlCube.transform.position))
        {
            //Update the location of the objects based on where the controller is now (if it updated)
            for (int i = 0; i < proxyList.Count; i++)
            {
                Vector3 delta = transform.InverseTransformVector(controlCube.transform.position - controlCubeLocation);
                proxyList[i].transform.localPosition += delta;
                proxyList[i].transform.localPosition = Vector3.Max(proxyList[i].transform.localPosition, Vector3.zero);
                //proxyList[i].GetInstanceID
            }


        }
        else if ((x_attr && x_attr.controlGrabbable.isGrabbed) || (y_attr && y_attr.controlGrabbable.isGrabbed) || (z_attr && z_attr.controlGrabbable.isGrabbed))
        {
            //Update the location of the objects based on where the controller is now (if it updated)
            for (int i = 0; i < proxyList.Count; i++)
            {
                Vector3 delta = transform.InverseTransformVector(controlCube.transform.position - controlCubeLocation);
                proxyList[i].transform.localPosition += delta;
                proxyList[i].transform.localPosition = Vector3.Max(proxyList[i].transform.localPosition, Vector3.zero);
                //proxyList[i].GetInstanceID
            }
        }

        controlCubeLocation = controlCube.transform.position;

        //Update the colour of the mesh based on the control cube location
        if (voxelOriginals.Count > 0) {
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaRed", controlCube.transform.localPosition.x);
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaGreen", controlCube.transform.localPosition.y);
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", controlCube.transform.localPosition.z);
        }

    }

    public void Animate()
    {
        //Move the voxels that have reached their location into the main list
        for (int i = 0; i < voxels.Count; i++) {
            Transform child = voxels[i];
            if (Vector3.Distance(child.localPosition, colors[i]) < 0.00001)
            {
                child.localPosition = colors[i];
                child.gameObject.AddComponent<Proxy>();
                child.gameObject.GetComponent<Proxy>().original = voxelOriginals[i];
                proxyList.Add(child.GetComponent<Proxy>());
                voxels.RemoveAt(i);
                colors.RemoveAt(i);
            }
        }
        for (int i = 0; i < voxels.Count; i++) {
            Transform child = voxels[i];
            speed = 0.1f;

            Vector3 colorLocation = colors[i];
            Vector3 newlocation = Vector3.Lerp(child.localPosition, colorLocation, speed);
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 0.9f, speed);

            /*
                *Old version of the lerp that moves everything by the same speed
            //lerp the position
            child.localPosition = newlocation;
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 0.5f, speed);
            */
            //Move each point a different amount based on the distance from the design space
            float dist = Vector3.Distance(child.localPosition, colorLocation);
            float t = Mathf.Clamp(Mathf.Exp(-5 * dist), 0.05f, 0.2f);
            child.localPosition = Vector3.Lerp(child.localPosition, colorLocation, t);
            child.localPosition = Vector3.Lerp(child.localPosition, colorLocation, t);
        }

        //Move things in _gameObjectList
        for (int i = 0; i < proxyList.Count; i++) {
            //Check all the magnets
            for (int j = 0; j < magnetList.Count; j++) {
                //If it intersects with the valid region of a magnet, move it

                Vector3 magnett = magnetList[j].transform.localPosition;

                float distance = Vector3.Distance(magnetList[j].transform.localPosition, proxyList[i].transform.localPosition);

                float sphereRadius = magnetList[j].GetComponent<Renderer>().bounds.extents.magnitude;
                float otherRadius = magnetList[j].GetComponent<SphereCollider>().radius;

                //Check that it's not too close to one of the constraints
                for (int k = 0; k < constraintList.Count; k++)
                {
                    float constraintRadius = constraintList[k].GetComponent<Renderer>().bounds.extents.magnitude;
                    float constraintDistance = Vector3.Distance(constraintList[k].transform.localPosition, proxyList[i].transform.localPosition);

                    if (distance < sphereRadius && constraintDistance > constraintRadius)
                    {
                        //lerp it towards the magnet's centre

                        //TODO Ohhh the gameobjects list is the oroiginal object! Not the proxy!!! That's why this isn't working

                        proxyList[i].transform.localPosition = Vector3.Lerp(proxyList[i].transform.localPosition, magnetList[j].transform.localPosition, 0.09f);
                    }
                }
            }
        }

    }

    public void SelectObject(GameObject selection)
    {
        if (_gameObjectList.Contains(selection) == false)
        {
            //Add the selected item into the list if it's not already in it
            _gameObjectList.Add(selection);
            if (selection.GetComponent<Voxelizable>() == null)
            {
                AddToDesignSpace(selection);

                //highlight the selected object in white
                if (!selection.GetComponent<Outline>())
                {
                    var whiteOutline = selection.AddComponent<Outline>();
                    whiteOutline.OutlineMode = Outline.Mode.OutlineAll;
                    whiteOutline.OutlineColor = Color.white;
                    whiteOutline.OutlineWidth = 5f;
                }
            }
            else {
                AddVoxelsToDesignSpace(selection);

                if (!selection.GetComponent<Outline>())
                {
                    //highlight the selected object in yellow
                    var yellowOutline = selection.AddComponent<Outline>();
                    yellowOutline.OutlineMode = Outline.Mode.OutlineAll;
                    yellowOutline.OutlineColor = Color.yellow;
                    yellowOutline.OutlineWidth = 5f;
                }
            }


        }


    }

    //creates the proxy for the object in the design space
    public void AddToDesignSpace(GameObject selection)
    {
        GameObject proxySphere = GameObject.Instantiate(proxyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        proxySphere.GetComponent<Proxy>().original = selection;

        proxySphere.transform.parent = this.transform;

        //Get the proxy's original position using teh attributes that you have selected
        float xComponent = x_attr ? x_attr.attribute.getCurrentValue(selection) : 0f;
        float yComponent = y_attr ? y_attr.attribute.getCurrentValue(selection) : 0f;
        float zComponent = z_attr ? z_attr.attribute.getCurrentValue(selection) : 0f;
        proxySphere.transform.localPosition = new Vector3(xComponent, yComponent, zComponent);

        Proxy newProxy = proxySphere.GetComponent<Proxy>();

        newProxy.parentSpace = this;
        //proxySphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        proxyList.Add(newProxy);
    }

    //Creates a voxelized version of the object then animates it into the current design space box
    public void AddVoxelsToDesignSpace(GameObject selection) {
        //Don't access selection.transform, access the selection's children's transforms!!!
        Voxels voxel_parent = selection.GetComponentInChildren<Voxels>();

        foreach (Transform child in voxel_parent.transform)
        {
            voxelOriginals.Add(selection.GetComponentInChildren<Voxels>().originalObject);
            voxels.Add(child);
            Color color = child.GetComponent<MeshRenderer>().material.color;
            float R = color.r;
            float G = color.g;
            float B = color.b;
            colors.Add(new Vector3(R, G, B));
        }

        foreach (Transform child in voxels)
        {
            child.SetParent(this.transform);
        }
    }

    /*
    public void SetGlobalScale(Transform objTransform, Vector3 globalScale)
    {
        //Setting to 1 negates the effects of the parent's transforms
        objTransform.localScale = Vector3.one;
        //lossyscale is global scale
        objTransform.localScale = new Vector3(globalScale.x / Mathf.Max(Mathf.Abs(objTransform.lossyScale.x), 0.000001f),
                                            globalScale.y / Mathf.Max(Mathf.Abs(objTransform.lossyScale.y), 0.000001f),
                                            globalScale.z / Mathf.Max(Mathf.Abs(objTransform.lossyScale.z), 0.000001f));
    }
    */

    //Applies the deltas shown by the space to the objects in the real scene
    public void ApplyToWorld() {
        //Loop through all the objects and apply the transformations based on where the object is in the design space
        //For all of the proxies in this space, look at their position 
        //and apply the transformations to the originals in the world

        Vector3 proxylocation;

        for (int i = 0; i < proxyList.Count; i++)
        {

            proxylocation = proxyList[i].gameObject.transform.localPosition;

            //proxyList[i].original.transform.localScale = proxyList[i].original.transform.localScale + (proxyList[i].transform.localPosition - proxyList[i].originalLocation);
            //set each of the axes of the update 
            //x_attr.attribute.applyAttributeChange(proxy, x_attr, x location in design space);  This will apply the change in the world based on where the location is 
            //Ths command below changes the original object
            if (x_attr) { x_attr.attribute.applyAttributeChange(proxyList[i], x_attr, 0, proxyList[i].past_position.x + (proxyList[i].transform.localPosition.x - proxyList[i].past_position.x)); }
            if (y_attr) { y_attr.attribute.applyAttributeChange(proxyList[i], y_attr, 1, proxyList[i].past_position.y + (proxyList[i].transform.localPosition.y - proxyList[i].past_position.y)); }
            if (z_attr) { z_attr.attribute.applyAttributeChange(proxyList[i], z_attr, 2, proxyList[i].past_position.z + (proxyList[i].transform.localPosition.z - proxyList[i].past_position.z)); }
        }

    }


    public void UnapplyFromWorld() {

        //For all of the proxies in this space, look at their position 
        //and apply the inverse of the transformations to the originals in the world
        Vector3 proxylocation;

        for (int i = 0; i < proxyList.Count; i++)
        {

            //proxyList[i].original.transform.localScale = proxyList[i].original.transform.localScale - (proxyList[i].transform.localPosition-proxyList[i].originalLocation);
            if (x_attr) { x_attr.attribute.applyAttributeChange(proxyList[i], x_attr, 0, proxyList[i].originalLocation.x); }
            if (y_attr) { y_attr.attribute.applyAttributeChange(proxyList[i], y_attr, 1, proxyList[i].originalLocation.y); }
            if (z_attr) { z_attr.attribute.applyAttributeChange(proxyList[i], z_attr, 2, proxyList[i].originalLocation.z); }
        }
    }

    //Update the location of the object after adding a new axis
    public void UpdateLocations() {
        for (int i = 0; i < proxyList.Count; i++)
        {
            //Only show the distribution along axes that exist
            Vector3 location = new Vector3(0, 0, 0);
            if (x_attr)
            {
                //location = location.SetX(originalLoc.x);
                location = location.SetX(x_attr.attribute.getCurrentValue(proxyList[i].original));
            }
            if (y_attr)
            {
                //location = location.SetY(originalLoc.y);
                location = location.SetY(y_attr.attribute.getCurrentValue(proxyList[i].original));
            }
            if (z_attr)
            {
                //location = location.SetZ(originalLoc.z);
                location = location.SetZ(z_attr.attribute.getCurrentValue(proxyList[i].original));
            }
            proxyList[i].transform.localPosition = location;

        }
    }

}
