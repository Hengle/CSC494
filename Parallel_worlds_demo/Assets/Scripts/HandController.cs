using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class HandController : MonoBehaviour
{
    public OVRInput.Controller controller;
    public PlayerController player;

    float2 oldThumbstickPos;

    public OVRGrabber grabber;

    bool isRayCasting = false;
    GameObject pointedAt = null;

    public LineRenderer rayVisual;

    // Update is called once per frame
    void Update()
    {
        PlayerController player = PlayerController.instance;

        transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);

        float2 thumbstickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
        float2 delta = thumbstickPos - oldThumbstickPos;

        player.position += new float3(-delta.y, 0f, delta.x) * 0.1f;

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
            hit.collider.gameObject.transform.localScale = Vector3.zero;
            pointedAt = hit.collider.gameObject;
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

