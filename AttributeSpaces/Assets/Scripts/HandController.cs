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
    public bool isGrabbing { get => grabber.grabbedObject != null; }
    // Update is called once per frame
    void Update()
    {
        PlayerController player = PlayerController.instance;

        transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);

        float2 thumbstickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
        float2 delta = thumbstickPos - oldThumbstickPos;

        player.position += new float3(-delta.y, 0f, delta.x) * 0.1f;

        if (OVRInput.GetDown(OVRInput.Button.Two, controller)) {
            if(isGrabbing){
                grabber.GrabbableRelease(Vector3.zero, Vector3.zero);
            }
        }

    }

}

