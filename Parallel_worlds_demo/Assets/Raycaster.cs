using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public OVRInput.Controller controller;

    DesignSpaceManager DSManager;

    bool isRayCasting = false;
    GameObject pointedAt = null;

    public LineRenderer rayVisual;
   

    // Start is called before the first frame update
    void Start()
    {
        DSManager = DesignSpaceManager.instance;
    }

    void Update()
    {
        
        if (OVRInput.GetDown(OVRInput.Button.One, controller))
        {
            StartRayCasting();
        }

        if (OVRInput.GetUp(OVRInput.Button.One, controller))
        {
            StopRayCasting();
        }

        if (isRayCasting)
        {
            UpdateRayCasting();
        }
        
    }
    
    void StartRayCasting()
    {
        isRayCasting = true;
        rayVisual.enabled = true;
    }

    void StopRayCasting()
    {
        isRayCasting = false;
        rayVisual.enabled = false;

        if (pointedAt != null)
        {
        }
    }

    void UpdateRayCasting()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            //Get rid of this later! This just makes the thing disappear
            //hit.collider.gameObject.transform.localScale = Vector3.zero;

            //pointedAt is the object that you want to select
            pointedAt = hit.collider.gameObject;

            //Make sure that you aren't hitting something in the axis
            if ((pointedAt.gameObject.transform.parent != DSManager.designSpace.transform) && (pointedAt.gameObject.transform != DSManager.designSpace.transform)){
                DSManager.SelectObject(pointedAt);
            }

            rayVisual.SetPosition(0, transform.position);
            rayVisual.SetPosition(1, hit.point);


        }
        else
        {
            rayVisual.SetPosition(0, transform.position);
            rayVisual.SetPosition(1, transform.position + transform.forward);
        }
    }
   
}
