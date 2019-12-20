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
    public GameObject designSpace;

    static List<GameObject> _gameObjectList;

    private void Awake()
    {
        _instance = this;
        _gameObjectList = new List<GameObject>();
    }

    public void SelectObject(GameObject selection)
    {
        if(_gameObjectList.Contains(selection) == false){
            //Add the selected item into the list if it's not already in it
            _gameObjectList.Add(selection);
            AddToDesignSpace(selection);
        }

    }

    //creates the proxy for the object in the design space
    public void AddToDesignSpace(GameObject selection)
    {
        GameObject proxySphere = Instantiate(proxyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        proxySphere.transform.parent = designSpace.transform;
        proxySphere.GetComponent<Proxy>().original = selection;
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
