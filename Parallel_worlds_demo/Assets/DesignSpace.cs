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

    //Objects and values needed to create a voxelization
    //The source gameobject is the selection that was made by the user
    //GameObject sourceGameObject;
    //MeshVoxelizerUtil.GenerationType generationType = MeshVoxelizerUtil.GenerationType.SeparateVoxels;
    //MeshVoxelizerUtil.VoxelSizeType voxelSizeType = MeshVoxelizerUtil.VoxelSizeType.Subdivision;
    //int subdivisionLevel = 1;
    //public static float absoluteVoxelSize = 10000;
    //public static Precision precision = Precision.Standard;
    //public static UVConversion uvConversion = UVConversion.SourceMesh;

    /*
    public DesignSpace(string x_attr, string y_attr, string z_attr, GameObject axis, List<GameObject> _gameObjectList, GameObject proxyPrefab) {
        //This is the constructor
        this.x_attr = x_attr;
        this.y_attr = y_attr;
        this.z_attr = z_attr;
        this.axis = axis;
        this._gameObjectList = _gameObjectList;
        this.proxyPrefab = proxyPrefab;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame BUT ONLY IF IT'S A MONOBEHAVIOUR!!
    public void animate()
    {
        for (int i = 0; i < voxels.Count; i++)
        {
            Transform child = voxels[i];

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
