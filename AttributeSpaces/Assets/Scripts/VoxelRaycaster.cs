using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelRaycaster : MonoBehaviour
{
    public OVRInput.Controller controller;

    DesignSpaceManager DSManager;

    bool isRayCasting = false;
    GameObject pointedAt = null;

    //For the voxel-based workflow
    public LineRenderer voxelRayVisual;

    // Start is called before the first frame update
    void Start()
    {
        DSManager = DesignSpaceManager.instance;
    }

    void Update()
    {

        if (OVRInput.GetDown(OVRInput.Button.Two, controller))
        {
            StartRayCasting();
        }

        if (OVRInput.GetUp(OVRInput.Button.Two, controller))
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
        voxelRayVisual.enabled = true;
    }

    void StopRayCasting()
    {
        isRayCasting = false;
        voxelRayVisual.enabled = false;

    }

    void UpdateRayCasting()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            //Get rid of this later! This just makes the thing disappear
            //hit.collider.gameObject.transform.localScale = Vector3.zero;

            //pointedAt is the object that you want to select
            pointedAt = hit.collider.gameObject;

            //Only add if it's a voxelizable object and there's a currently selected design space
            if (pointedAt.GetComponent<Voxelizable>() && DSManager.GetMainDesignSpace())
            {
                DSManager.GetMainDesignSpace().SelectObject(pointedAt);
            }

            voxelRayVisual.SetPosition(0, transform.position);
            voxelRayVisual.SetPosition(1, hit.point);

        }
        else
        {
            voxelRayVisual.SetPosition(0, transform.position);
            voxelRayVisual.SetPosition(1, transform.position + transform.forward);
        }
    }

}
