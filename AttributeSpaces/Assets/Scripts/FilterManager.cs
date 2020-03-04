using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class FilterManager : MonoBehaviour
{
    static FilterManager _FilterManagerInstance;
    public static FilterManager instance { get => _FilterManagerInstance ?? FindObjectOfType<FilterManager>(); }

    public List<GameObject> hovering = new List<GameObject>();

    SavedSpaceManager SavedSpaces;

    //list of objects in the filter panel
    //It can accept Attributes, Gameobjects and 
    // Start is called before the first frame update
    public List<Attribute> attributeFilters;
    public List<GameObject> objectFilters;
    private void Awake()
    {
        _FilterManagerInstance = this;
        SavedSpaces = SavedSpaceManager.instance;
        attributeFilters = new List<Attribute>();
        objectFilters = new List<GameObject>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (!hovering.Contains(collider.gameObject))
        {
            hovering.Add(collider.gameObject);
        }
        

        //TODO deal with the case where you can filter based on which gameobjects are present in the space!!

    }
    void OnTriggerExit(Collider collider)
    {
        GrabbableAttribute grabbableAttr = collider.gameObject.GetComponent<GrabbableAttribute>();
        Attribute attributeFilter = collider.gameObject.GetComponentInChildren<Attribute>();

        if (hovering.Contains(collider.gameObject)) {
            hovering.Remove(collider.gameObject);
            
        }

        }
    public bool MatchesCriteria(DesignSpace querySpace)
    {
        bool isActive = querySpace.gameObject.activeSelf;
        querySpace.gameObject.SetActive(true);
        //If there are no restrictions, just show everything
        if (attributeFilters.Count == 0 && objectFilters.Count == 0) {
            return true;
        }
        bool matches = false;

        //Check if the given design space contains any of the given attributes
        foreach (Attribute attr in attributeFilters)
        {
            if (querySpace.x_attr && querySpace.x_attr.attribute.attributeType == attr.attributeType)
            {
                matches = true;
                break;
            }
            if (querySpace.y_attr && querySpace.y_attr.attribute.attributeType == attr.attributeType)
            {
                matches = true;
                break;
            }
            if (querySpace.z_attr && querySpace.z_attr.attribute.attributeType == attr.attributeType)
            {
                matches = true;
                break;
            }
        }
        //Check if the given design space has any of the objects represented
        foreach (GameObject sceneObject in objectFilters)
        {
            foreach (Proxy proxy in querySpace.proxyList)
            {
                if (proxy.original == sceneObject)
                {
                    matches = true;
                    break;
                }
            }
        }
        querySpace.gameObject.SetActive(isActive);
        return matches;
    }
}