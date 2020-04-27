using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSpaceSelector : MonoBehaviour
{

    public static StartingSpaceSelector instance { get => _instance ?? FindObjectOfType<StartingSpaceSelector>(); }
    static StartingSpaceSelector _instance;

    public DesignSpace startingSpace;

    public List<DesignSpace> hovering;
    // Start is called before the first frame update
    void Start()
    {
        startingSpace = null;
        hovering = new List<DesignSpace>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Add(designSpace);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        DesignSpace designSpace = collider.gameObject.GetComponent<DesignSpace>();

        if (designSpace)
        {
            hovering.Remove(designSpace);
        }

    }


}
