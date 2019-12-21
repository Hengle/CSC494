using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;

public class DesignSpaceManager : MonoBehaviour
{
    
    public static DesignSpaceManager instance { get => _instance ?? FindObjectOfType<DesignSpaceManager>(); }
    static DesignSpaceManager _instance;

    public GameObject proxyPrefab;
    public GameObject designSpace;

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
    public DesignSpace getMainDesignSpace() {
        return _DesignSpaceList[main_index];
    }
    private void Awake()
    {
        _instance = this;
        _DesignSpaceList = new List<DesignSpace>();
        main_index = 0;
        _DesignSpaceList.Add(new DesignSpace("Scale x", "Scale y", "Scale z", designSpace, new List<GameObject>(), proxyPrefab));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
