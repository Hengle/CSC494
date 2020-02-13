using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class DesignSpace: MonoBehaviour
{
    public RangeSlider x_attr;
    public RangeSlider y_attr;
    public RangeSlider z_attr;
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

    // Start is called before the first frame update
    private void Start()
    {
        //Add itself to the Design Space Manager's list of objects
        DesignSpaceManager.instance.AddDesignSpaceToList(this);
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

            x_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.x - 0.5f, 0f, 0f);
            x_attr.UpdateMax();

            y_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.y - 0.5f, 0f, 0f);
            y_attr.UpdateMax();

            z_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.z - 0.5f, 0f, 0f);
            z_attr.UpdateMax();

        }
        else if (x_attr.maxGrabbable.isGrabbed || y_attr.maxGrabbable.isGrabbed || z_attr.maxGrabbable.isGrabbed)
        {
            controlCube.transform.localPosition = new Vector3(x_attr.maxValue, y_attr.maxValue, z_attr.maxValue);
        }
        else { // default case is to update based on the cube location
            x_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.x - 0.5f, 0f, 0f);
            x_attr.UpdateMax();

            y_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.y - 0.5f, 0f, 0f);
            y_attr.UpdateMax();

            z_attr.maxGrabbable.transform.localPosition = new Vector3(controlCube.transform.localPosition.z - 0.5f, 0f, 0f);
            z_attr.UpdateMax();
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
        else if ((x_attr.maxGrabbable.isGrabbed || y_attr.maxGrabbable.isGrabbed || z_attr.maxGrabbable.isGrabbed))
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
        if(voxelOriginals.Count > 0) {
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaRed", controlCube.transform.localPosition.x);
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaGreen", controlCube.transform.localPosition.y);
            voxelOriginals[0].GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", controlCube.transform.localPosition.z);
        }

        //Update the view of the objects in the world if it's the main space
        //Only call this if the parent is the main design space



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
        for (int i = 0; i < voxels.Count; i++){
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
        //proxySphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        proxyList.Add(proxySphere.GetComponent<Proxy>());
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

    public void SetGlobalScale(Transform objTransform, Vector3 globalScale)
    {
        //Setting to 1 negates the effects of the parent's transforms
        objTransform.localScale = Vector3.one;
        //lossyscale is global scale
        objTransform.localScale = new Vector3(globalScale.x / Mathf.Max(Mathf.Abs(objTransform.lossyScale.x), 0.000001f),
                                            globalScale.y / Mathf.Max(Mathf.Abs(objTransform.lossyScale.y), 0.000001f),
                                            globalScale.z / Mathf.Max(Mathf.Abs(objTransform.lossyScale.z), 0.000001f));
    }

    //Applies the deltas shown by the space to the objects in the real scene
    public void ApplyToWorld() {
        //Loop through all the objects and apply the transformations based on where the object is in the design space
        //For all of the proxies in this space, look at their position 
        //and apply the transformations to the originals in the world
        
        Vector3 proxylocation;

        for (int i = 0; i < proxyList.Count; i++) {

            proxylocation = proxyList[i].gameObject.transform.localPosition;

            proxyList[i].original.transform.localScale = proxyList[i].original.transform.localScale + (proxyList[i].transform.localPosition - proxyList[i].originalLocation);

        }
        
    }


    public void UnapplyFromWorld() {
        
        //For all of the proxies in this space, look at their position 
        //and apply the inverse of the transformations to the originals in the world
        Vector3 proxylocation;

        for (int i = 0; i < proxyList.Count; i++) {

            proxyList[i].original.transform.localScale = proxyList[i].original.transform.localScale - (proxyList[i].transform.localPosition-proxyList[i].originalLocation);
        }
        
    }
}
