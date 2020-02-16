using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

public delegate void OnAxisUpdatedEvent();

public class Axis : MonoBehaviour
{
    public Transform minVisual, controlVisual, axisHandle;
    public OVRGrabbable controlGrabbable, AxisGrabbable;
    public TextMesh minText, maxText, label;
    public Transform pipe, highlightPipe;

    [NonSerialized] public bool minGrabbed, controlGrabbed, maxGrabbed;

    [NonSerialized] public float minValue = 0f;
    [NonSerialized] public float maxValue = 1f;

    [RangeAttribute(0.0f, 1.0f)]
    [NonSerialized] public float currentValue = 1f;

    HandController hand;

    public OnAxisUpdatedEvent OnAxisUpdated;

    float min;
    float max;
    float currentVal;
    //The attribute that this axis will change
    Attribute attribute;

    void Start()
    {
        controlGrabbable.OnGrabbed += (grabber, collider) => { controlGrabbed = true; hand = grabber.GetComponent<HandController>(); };
        controlGrabbable.OnReleased += (linvel, angvel) => { controlGrabbed = false; controlGrabbable.transform.position = controlVisual.transform.position; };
    }

    // Update is called once per frame
    void Update()
    {
        // Max grabbed
        if (controlGrabbed)
        {
            UpdateMax();
        }

        highlightPipe.position = (controlVisual.position + minVisual.position) / 2f;
        highlightPipe.localScale = highlightPipe.localScale.SetY(0.05f * math.abs(currentValue - minValue));

        controlGrabbable.transform.localScale = (float3)0.01f;
    }

    public void UpdateMax()
    {
        controlVisual.transform.position = controlGrabbable.transform.position;

        float t = math.clamp(controlVisual.transform.localPosition.x, minValue - 0.5f, 0.5f);

        currentValue = t + 0.5f;
        controlVisual.transform.localPosition = new float3(t, 0f, 0f);

        string label = currentValue.ToString("0.00");

        if (maxText.text != label)
        {
            maxText.text = label;
            if (OnAxisUpdated != null) OnAxisUpdated();
        }
    }

    public void Disable()
    {
        enabled = false;
        controlGrabbable.enabled = false;
    }

    public void Enable()
    {
        enabled = true;
        controlGrabbable.enabled = true;
    }

}
