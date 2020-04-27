using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;

public class DesignSpaceManager : MonoBehaviour
{
    public OVRInput.Controller controller;
    
    public static DesignSpaceManager instance { get => _instance ?? FindObjectOfType<DesignSpaceManager>(); }
    static DesignSpaceManager _instance;

    public GameObject proxyPrefab;

    public  List<DesignSpace> _DesignSpaceList;

    public int main_index;

    public GameObject panelParent;
    /*
     * This class has the master list of design spaces
     * 
     * 
     * Design space manager class:
     * static list of the different design spaces available
     * int for the index of the design space that you are currently showing
     * 
     * 
     * 
    */
    public DesignSpace GetMainDesignSpace() {
        if (main_index != -1)
        {
            return _DesignSpaceList[main_index];
        }
        return null;
    }
    private void Awake()
    {
        _instance = this;
        _DesignSpaceList = new List<DesignSpace>();
        main_index = -1;

    }
    public int AddDesignSpaceToList(DesignSpace newSpace) {
        _DesignSpaceList.Add(newSpace);
        //Return the index of the newly added space
        return _DesignSpaceList.Count -1;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //If there's just one space then set it to be the default
        if (_DesignSpaceList.Count == 1)
        {
            _DesignSpaceList[0].isMainSpace = true;
        }

        //Enable in case it isn't already. This should already be done in DesignSpace.cs
        if (main_index != -1 && _DesignSpaceList[main_index])
        {
            //_DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (_DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>())
            {
                _DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>().material.color = Color.black;
            }
            _DesignSpaceList[main_index].isMainSpace = true;
        }
        
        for (int i = 0; i< _DesignSpaceList.Count; i++) {

            _DesignSpaceList[i].Animate();

            if (_DesignSpaceList[i].isMainSpace == true)
            {
                _DesignSpaceList[i].x_attr?.Enable();
                _DesignSpaceList[i].y_attr?.Enable();
                _DesignSpaceList[i].z_attr?.Enable();
            }
            else {
                _DesignSpaceList[i].x_attr?.Disable();
                _DesignSpaceList[i].y_attr?.Disable();
                _DesignSpaceList[i].z_attr?.Disable();
            }

        }

    }
}
