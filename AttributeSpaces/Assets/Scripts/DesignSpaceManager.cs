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
        
        //enable in case it isn't already
        if (_DesignSpaceList[main_index])
        {
            _DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        
        foreach (DesignSpace space in _DesignSpaceList) { 
            space.Animate();
        }

        print(_DesignSpaceList.Count);
        if (OVRInput.GetDown(OVRInput.Button.Three, controller))
        {
            
            if (main_index + 1 >= _DesignSpaceList.Count)
            {
                //_DesignSpaceList[main_index].DisableOutline();
                _DesignSpaceList[main_index].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //Change the parents of each object (the main space gets parented to the world and the other spaces get parented to the panel)
                _DesignSpaceList[0].transform.parent = _DesignSpaceList[main_index].transform.parent;
                _DesignSpaceList[main_index].transform.parent = panelParent.transform;
                main_index = 0;
                _DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            else {
                _DesignSpaceList[main_index].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                _DesignSpaceList[main_index+1].transform.parent = _DesignSpaceList[main_index].transform.parent;
                _DesignSpaceList[main_index].transform.parent = panelParent.transform;
                main_index += 1;
                _DesignSpaceList[main_index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }


    }
}
