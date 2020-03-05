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

    float speed;

    public GameObject magnetParent;
    public GameObject constraintParent;

    List<Proxy> voxels = new List<Proxy>();
    public List<Proxy> proxyList = new List<Proxy>();
    List<GameObject> magnetList = new List<GameObject>();
    List<GameObject> constraintList = new List<GameObject>();

    public bool isMainSpace;

    Vector3 controlCubeLocation;

    public GameObject originCube;

    public int DesignSpaceID;

    OVRGrabbable grabbable;

    SavedSpaceManager SavedSpaces;

    public bool isSaveClone;

    public AxisManager axisManager;

    public DesignSpaceManager designSpaceManager;

    // Start is called before the first frame update
    private void Start()
    {
        designSpaceManager = DesignSpaceManager.instance;
        SavedSpaces = SavedSpaceManager.instance;
        grabbable = GetComponent<OVRGrabbable>();

        grabbable.OnGrabbed += (grab, pt) =>
        {
            //Remove the saved space from the list so that it doesn't get affected by the SS movements after you grab it
            if (SavedSpaces.SavedSpaces.Contains(this))
            {
                //transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                //Disable the design space when it's saved
                SavedSpaces.SavedSpaces.Remove(this);
            }

            designSpaceManager.main_index = DesignSpaceID;
            originCube.transform.GetComponent<MeshRenderer>().material.color = Color.black;


        };

        grabbable.OnReleased += (linvel, angvel) =>
        {
            
            if (SavedSpaces.hovering.Contains(this))
            {
                //If it's within the saved space area and not in the saved spaces list yet, add it to the list
                if (!SavedSpaces.SavedSpaces.Contains(this))
                {

                    UnapplyFromWorld();
                    SavedSpaces.SavedSpaces.Add(this);
                    

                    transform.parent = SavedSpaces.SavedSpacesCollection.transform;
                    transform.localScale = transform.localScale - new Vector3(0.1f, 0.1f, 0.1f);
                    //transform.AnimateLocalPosition(new Vector3(0f, 0.035f, -0.035f));
                    this.transform.localRotation = Quaternion.identity;
                    //LockSpace();


                }
                else { //If you just moved it in the space a little bit, correct the orientation
                    this.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                //Means that they want to add it to the world
                
                SavedSpaces.SavedSpaces.Remove(this);
                transform.parent = null;
                transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                //UnlockSpace();
                ApplyToWorld();
            }
            designSpaceManager.main_index = -1;
            originCube.transform.GetComponent<MeshRenderer>().material.color = Color.white;
        };

        //Add itself to the Design Space Manager's list of objects
        DesignSpaceID = DesignSpaceManager.instance.AddDesignSpaceToList(this);

        controlCubeLocation = controlCube.transform.position;

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

        //Set up the grabb handlers for the control cube
        controlCube.GetComponent<OVRGrabbable>().OnGrabbed += (grab, pt) =>
        {
            //Save a snapshot when the control cube is grabbed because you know that you're about to move it to another location
            SavedSpaces.SaveSpace(this);
        };

        isSaveClone = false;
    }

    // Update is called once per frame BUT ONLY IF IT'S A MONOBEHAVIOUR!!
    public void Update()
    {

        //If this is a clone of another space, don't activate it!
        if (isSaveClone) {
            DesignSpaceManager.instance.RemoveDesignSpaceFromList(this);
        }
        //Don't update if it's not the main space
        if (!isMainSpace) {
            return;
        }
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
        for (int i = 0; i<proxyList.Count; i++)
        {

            if (x_attr)
            {
                x_attr.attribute.applyAttributeChange(proxyList[i], x_attr, 0, controlCube.transform.localPosition.x);
            }
            else if (y_attr)
            {
                y_attr.attribute.applyAttributeChange(proxyList[i], y_attr, 1, controlCube.transform.localPosition.y);

            }
            else if (z_attr)
            {
                z_attr.attribute.applyAttributeChange(proxyList[i], z_attr, 2, controlCube.transform.localPosition.z);
            }

        }

        //Snapping the control sphere to be on just the axes that are defined
        float x_pos = controlCube.transform.localPosition.x;
        float y_pos = controlCube.transform.localPosition.y;
        float z_pos = controlCube.transform.localPosition.z;
        if (!x_attr) {
            x_pos = 0f;
        }
        if (!y_attr)
        {
            y_pos = 0f;
        }
        if (!z_attr)
        {
            z_pos = 0f;
        }
        //Limits the degrees of freedon of the control cube to only the defined axes
        controlCube.transform.localPosition = new Vector3(x_pos, y_pos, z_pos);


    }

    public void Animate()
    {
        //Don't update if it's not the main space
        if (!isMainSpace)
        {
            return;
        }
        //Move the voxels that have reached their location into the main list
        for (int i = 0; i < voxels.Count; i++) {
            Proxy child = voxels[i];
            //Look at the distances between the original voxel and where it should be
            float x_pos = x_attr ? x_attr.attribute.getCurrentValue(child.original, child): 0f;
            float y_pos = y_attr ? y_attr.attribute.getCurrentValue(child.original, child): 0f;
            float z_pos = z_attr ? z_attr.attribute.getCurrentValue(child.original, child): 0f;
            float x_dist = x_attr ? x_pos - child.transform.localPosition.x: Mathf.Infinity;
            float y_dist = y_attr ? y_pos - child.transform.localPosition.y : Mathf.Infinity;
            float z_dist = z_attr ? z_pos - child.transform.localPosition.z : Mathf.Infinity;
            if (x_dist + y_dist + z_dist < 0.000001f)
            {
                child.transform.localPosition = new Vector3(x_pos, y_pos, z_pos);
                proxyList.Add(child);
                voxels.RemoveAt(i);
            }
        }
        for (int i = 0; i < voxels.Count; i++) {
            Proxy child = voxels[i];
            speed = 0.1f;

            float x_pos = x_attr ? x_attr.attribute.getCurrentValue(child.original, child) : 0f;
            float y_pos = y_attr ? y_attr.attribute.getCurrentValue(child.original, child) : 0f;
            float z_pos = z_attr ? z_attr.attribute.getCurrentValue(child.original, child) : 0f;

            Vector3 colorLocation = new Vector3(x_pos, y_pos, z_pos);
            //colorLocation should be replaces by the value of the original object 
            Vector3 newlocation = Vector3.Lerp(child.transform.localPosition, colorLocation, speed);
            child.transform.localScale = Vector3.Lerp(child.transform.localScale, Vector3.one * 0.9f, speed);

            /*
                *Old version of the lerp that moves everything by the same speed
            //lerp the position
            child.localPosition = newlocation;
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 0.5f, speed);
            */
            //Move each point a different amount based on the distance from the design space
            float dist = Vector3.Distance(child.transform.localPosition, colorLocation);

            float t = Mathf.Clamp(Mathf.Exp(-5 * dist), 0.05f, 0.2f);
            child.transform.localPosition = Vector3.Lerp(child.transform.localPosition, colorLocation, t);
            child.transform.localPosition = Vector3.Lerp(child.transform.localPosition, colorLocation, t);
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
        //Make another copy of the original object
        proxySphere.GetComponent<Proxy>().original_backup = GameObject.Instantiate(selection, selection.transform.position, selection.transform.localRotation);
        //Have an invisible backup so that you know what was there originally
        proxySphere.GetComponent<Proxy>().original_backup.SetActive(false);
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
            //create a proxy out of it and add it to child
            child.gameObject.AddComponent<Proxy>();
            child.gameObject.GetComponent<Proxy>().original = selection;
            child.gameObject.GetComponent<Proxy>().parentSpace = this;
            proxyList.Add(child.GetComponent<Proxy>());

            voxels.Add(child.GetComponent<Proxy>());
        }

        foreach (Proxy child in voxels)
        {
            child.transform.SetParent(this.transform);
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
                location = location.SetX(x_attr.attribute.getCurrentValue(proxyList[i].original, proxyList[i]));
            }
            if (y_attr)
            {
                //location = location.SetY(originalLoc.y);
                location = location.SetY(y_attr.attribute.getCurrentValue(proxyList[i].original, proxyList[i]));
            }
            if (z_attr)
            {
                //location = location.SetZ(originalLoc.z);
                location = location.SetZ(z_attr.attribute.getCurrentValue(proxyList[i].original, proxyList[i]));
            }
            proxyList[i].transform.localPosition = location;

        }
    }

    //Locks the design space so that all of the axes aren't add/removable, the sliders aren't slidable and the proxies aren't grabbable
    public void LockSpace() {
        if (x_attr) {
            x_attr.Disable();
        }
        if (y_attr)
        {
            y_attr.Disable();
        }
        if (z_attr)
        {
            z_attr.Disable();
        }
        //Disable all of the axis colliders on the space to make sure you don't accidentally attach or remove stuff
        foreach (Transform axisCollider in axisManager.transform) {
            axisCollider.gameObject.SetActive(false);
        }

        foreach (Proxy proxy in proxyList) {
            if (proxy.GetComponent<Collider>()) {
                proxy.GetComponent<Collider>().enabled = false;
            }
        }

        controlCube.GetComponent<OVRGrabbable>().enabled = false;

    }

    //Unlock the Design space so that the user can interact with it again
    public void UnlockSpace() {
        if (x_attr)
        {
            x_attr.Enable();
        }
        if (y_attr)
        {
            y_attr.Enable();
        }
        if (z_attr)
        {
            z_attr.Enable();
        }
        //Enable all of the colliders on the space again
        foreach (Transform axisCollider in axisManager.transform)
        {
            axisCollider.gameObject.SetActive(true);
        }

        foreach (Proxy proxy in proxyList)
        {
            if (proxy.GetComponent<Collider>())
            {
                proxy.GetComponent<Collider>().enabled = true;
            }
        }

        controlCube.GetComponent<OVRGrabbable>().enabled = true;
    }
    //Checks if this object is compatible with this design space
    bool isCompatibleWithSpace() {
        return true;
    }
}
