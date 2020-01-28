using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignSpace: MonoBehaviour
{
    public string x_attr;
    public string y_attr;
    public string z_attr;
    //The axis contains the xyz axes and also the different proxy points as children 
    public GameObject axis;
    public List<GameObject> _gameObjectList;
    public GameObject proxyPrefab;

    float speed;
    List<Transform> voxels = new List<Transform>();
    List<Vector3> colors = new List<Vector3>();

    public List<GameObject> magnetsList = new List<GameObject>();
    public List<GameObject> constraintList = new List<GameObject>();

    public GameObject TEMPmagnet;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame BUT ONLY IF IT'S A MONOBEHAVIOUR!!
    public void Animate()
    {
        if (magnetsList.Contains(TEMPmagnet) == false) {
            magnetsList.Add(TEMPmagnet);
        }
        //Move the voxels that have reached their location into the main list
        for (int i = 0; i < voxels.Count; i++) {
            Transform child = voxels[i];
            if (Vector3.Distance(child.localPosition, colors[i]) < 0.00001)
            {
                child.localPosition = colors[i];
                _gameObjectList.Add(child.gameObject);
                voxels.RemoveAt(i);
                colors.RemoveAt(i);
            }
        }
        for (int i = 0; i < voxels.Count; i++){
            Transform child = voxels[i];

            if (_gameObjectList.Contains(child.gameObject) == false)
            {
                speed = 0.1f;

                Vector3 colorLocation = colors[i];
                Vector3 newlocation = Vector3.Lerp(child.localPosition, colorLocation, speed);

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
        }

        //Move things in _gameObjectList
        for (int i = 0; i < _gameObjectList.Count; i++) {
            //Check all the magnets
            for (int j = 0; j < magnetsList.Count; j++) {
                //If it intersects with the valid region of a magnet, move it

                float distance = Vector3.Distance(magnetsList[j].transform.localPosition, _gameObjectList[i].transform.localPosition);

                float sphereRadius = magnetsList[j].GetComponent<Renderer>().bounds.extents.magnitude;
                float otherRadius = magnetsList[j].GetComponent<SphereCollider>().radius;
                if (distance < sphereRadius) {
                    //lerp it towards the magnet's centre
                    _gameObjectList[i].transform.localPosition = Vector3.Lerp(_gameObjectList[i].transform.localPosition, magnetsList[j].transform.localPosition, 0.09f);
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
                var outline = selection.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = Color.white;
                outline.OutlineWidth = 5f;
            }
            else {
                AddVoxelsToDesignSpace(selection);

                //highlight the selected object in yellow
                var outline = selection.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 5f;

            }
                

        }
        

    }

    //creates the proxy for the object in the design space
    public void AddToDesignSpace(GameObject selection)
    {

        GameObject proxySphere = GameObject.Instantiate(proxyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        proxySphere.GetComponent<Proxy>().original = selection;

        proxySphere.transform.parent = this.axis.transform;

    }

    //Creates a voxelized version of the object then animates it into the current design space box
    public void AddVoxelsToDesignSpace(GameObject selection) {
        //Don't access selection.transform, access the selection's children's transforms!!!
        Voxels voxel_parent = selection.GetComponentInChildren<Voxels>();
        foreach (Transform child in voxel_parent.transform)
        {
            voxels.Add(child);
            Color color = child.GetComponent<MeshRenderer>().material.color;
            float R = color.r;
            float G = color.g;
            float B = color.b;
            colors.Add(new Vector3(R, G, B));
        }

        foreach (Transform child in voxels)
        {
            child.SetParent(this.axis.transform);
        }
    }

}
