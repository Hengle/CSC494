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

    //static List<GameObject> _gameObjectList;
    static List<DesignSpace> _DesignSpaceList;
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
        if (_DesignSpaceList.Count > 0)
        {
            return _DesignSpaceList[main_index];
        }
        else {
            print("No design spaces!");
            return null;
        }
        
    }
    private void Awake()
    {
        _instance = this;
        _DesignSpaceList = new List<DesignSpace>();
        main_index = 0;

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
        
        //enable in case it isn't already
        if (_DesignSpaceList[main_index])
        {
            //_DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (_DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>())
            {
                _DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
        
        foreach (DesignSpace space in _DesignSpaceList) { 
            space.Animate();
        }

        if (OVRInput.GetDown(OVRInput.Button.Three, controller))
        {
            int new_index;
            if (main_index + 1 >= _DesignSpaceList.Count)
            {
                new_index = 0;
            }
            else {
                new_index = main_index + 1;
            }

            //Don't do anything if there is only 1 space
            if (main_index != new_index)
            {
                
                //_DesignSpaceList[main_index].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                if (_DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>())
                {
                    _DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>().material.color = Color.white;
                }

                _DesignSpaceList[main_index].isMainSpace = false;
                _DesignSpaceList[main_index].UnapplyFromWorld();

                main_index = new_index;

                _DesignSpaceList[main_index].isMainSpace = true;
                //_DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                if (_DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>())
                {
                    _DesignSpaceList[main_index].originCube.transform.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                _DesignSpaceList[main_index].ApplyToWorld();
            }

        }


    }
}
