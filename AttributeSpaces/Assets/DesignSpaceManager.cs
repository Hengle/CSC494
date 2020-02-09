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
    static int main_index;

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
        if (_DesignSpaceList.Count > 0) {
            return _DesignSpaceList[main_index];
        }
        return null;
    }
    private void Awake()
    {
        _instance = this;
        _DesignSpaceList = new List<DesignSpace>();
        main_index = 0;

    }
    public void AddDesignSpaceToList(DesignSpace newSpace) {
        _DesignSpaceList.Add(newSpace);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        //Call the update function for each of the design spaces in that list
        foreach (DesignSpace space in _DesignSpaceList)
        {
            space.Animate();

            if (space.outline) {
                if (main_index == i)
                {
                    space.outline.enabled = true;
                }
                else
                {
                    space.outline.enabled = false;
                }
            }
            //Also set the design space origin blocks to be the right colours
            i += 1;
        }


    }
}
