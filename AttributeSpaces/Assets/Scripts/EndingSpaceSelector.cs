using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSpaceSelector : MonoBehaviour
{
    public static EndingSpaceSelector instance { get => _instance ?? FindObjectOfType<EndingSpaceSelector>(); }
    static EndingSpaceSelector _instance;

    public DesignSpace endingSpace;

    public List<DesignSpace> hovering;
    // Start is called before the first frame update
    void Start()
    {
        endingSpace = null;
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
