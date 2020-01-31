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
    public GameObject designSpace;

    //static List<GameObject> _gameObjectList;
    static List<DesignSpace> _DesignSpaceList;
    static int main_index;

    public GameObject TEMPmagnet;
    public GameObject TEMPconstraint;
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
        return _DesignSpaceList[main_index];
    }
    private void Awake()
    {
        _instance = this;
        _DesignSpaceList = new List<DesignSpace>();
        main_index = 0;

        //Add the initial default design space
        DesignSpace newSpace = new DesignSpace();

        _DesignSpaceList.Add(newSpace);
        newSpace.x_attr = "Scale x";
        newSpace.y_attr = "Scale y";
        newSpace.z_attr = "Scale z";
        newSpace.axis = designSpace;
        newSpace._gameObjectList = new List<GameObject>();
        newSpace.proxyPrefab = proxyPrefab;
        newSpace.TEMPmagnet = TEMPmagnet;
        newSpace.TEMPconstraint = TEMPconstraint;

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
        foreach (DesignSpace space in _DesignSpaceList) {
            space.Animate();
            
            /*
            if (main_index == i)
            {
                space.outline.enabled = true;
            }
            else {
                space.outline.enabled = false;
            }
            */
            //Also set the design space origin blocks to be the right colours
            i += 1;
        }


    }
}
