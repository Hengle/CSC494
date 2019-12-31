using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignSpace
{
    string x_attr;
    string y_attr;
    string z_attr;
    //The axis contains the xyz axes and also the different proxy points as children 
    GameObject axis;
    List<GameObject> _gameObjectList;
    GameObject proxyPrefab;

    //Objects and values needed to create a voxelization
    //The source gameobject is the selection that was made by the user
    //GameObject sourceGameObject;
    //MeshVoxelizerUtil.GenerationType generationType = MeshVoxelizerUtil.GenerationType.SeparateVoxels;
    //MeshVoxelizerUtil.VoxelSizeType voxelSizeType = MeshVoxelizerUtil.VoxelSizeType.Subdivision;
    //int subdivisionLevel = 1;
    //public static float absoluteVoxelSize = 10000;
    //public static Precision precision = Precision.Standard;
    //public static UVConversion uvConversion = UVConversion.SourceMesh;


    public DesignSpace(string x_attr, string y_attr, string z_attr, GameObject axis, List<GameObject> _gameObjectList, GameObject proxyPrefab) {
        //This is the constructor
        this.x_attr = x_attr;
        this.y_attr = y_attr;
        this.z_attr = z_attr;
        this.axis = axis;
        this._gameObjectList = _gameObjectList;
        this.proxyPrefab = proxyPrefab;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject getAxis() {
        return this.axis;
    }

    public void SelectObject(GameObject selection)
    {
        if (_gameObjectList.Contains(selection) == false)
        {
            //Add the selected item into the list if it's not already in it
            _gameObjectList.Add(selection);
            AddToDesignSpace(selection);

            //highlight the selected object 
            var outline = selection.AddComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 5f;
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

    }

}
