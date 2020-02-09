using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

public delegate void OnRangeSliderUpdatedEvent();

public class RangeSlider : MonoBehaviour
{
    public Transform minVisual, maxVisual;
    public OVRGrabbable minGrabbable, maxGrabbable;
    public TextMesh minText, maxText, label;
    public Transform pipe, highlightPipe;

    [NonSerialized] public bool minGrabbed, maxGrabbed;

    [NonSerialized] public float minValue = 0f;
    [NonSerialized] public float maxValue = 1f;

    HandController hand;

    public OnRangeSliderUpdatedEvent OnRangeSliderUpdated;

    void Start()
    {
        //minGrabbable.OnGrabbed += (grabber, collider) => {minGrabbed = true; hand = grabber.GetComponent<HandController>();};
        maxGrabbable.OnGrabbed += (grabber, collider) => {maxGrabbed = true; hand = grabber.GetComponent<HandController>();};

        //minGrabbable.OnReleased += (linvel, angvel) => {minGrabbed = false;};
        maxGrabbable.OnReleased += (linvel, angvel) => {maxGrabbed = false; maxGrabbable.transform.position = maxVisual.transform.position; };
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Min grabbed
        if (minGrabbed)
        {
            min.transform.position = minGrabbable.transform.position;

            float t = math.clamp(min.transform.localPosition.x, -0.5f, maxValue - 0.6f);

            minValue = t + 0.5f;
            min.transform.localPosition = new float3(t, 0f, 0f);

            string label = minValue.ToString("0.00");

            if (minText.text != label)
            {
                //hand?.Vibrate(0.1f, 0.1f);
                minText.text = label;

                if (OnRangeSliderUpdated != null) OnRangeSliderUpdated();
            }
        }
        else
        {
            minGrabbable.transform.position = min.transform.position;
        }
        */

        // Max grabbed
        if (maxGrabbed)
        {
            UpdateMax();
        }

        highlightPipe.position = (maxVisual.position + minVisual.position) / 2f;
        highlightPipe.localScale = highlightPipe.localScale.SetY(0.05f * math.abs(maxValue - minValue));

        //minGrabbable.transform.localScale = (float3) 0.01f;
        maxGrabbable.transform.localScale = (float3) 0.01f;
    }

    public void UpdateMax()
    {
        maxVisual.transform.position = maxGrabbable.transform.position;

        float t = math.clamp(maxVisual.transform.localPosition.x, minValue - 0.4f, 0.5f);

        maxValue = t + 0.5f;
        maxVisual.transform.localPosition = new float3(t, 0f, 0f);

        string label = maxValue.ToString("0.00");

        if (maxText.text != label)
        {
            //hand?.Vibrate(0.1f, 0.1f);
            maxText.text = label;
            if (OnRangeSliderUpdated != null) OnRangeSliderUpdated();
        }
    }

    public void Disable()
    {
        enabled = false;
        minGrabbable.enabled = false;
        maxGrabbable.enabled = false;
    }

    public void Enable()
    {
        enabled = true;
        minGrabbable.enabled = true;
        maxGrabbable.enabled = true;
    }

    public void Hide()
    {
        GetComponent<SetVisiblity>().SetVisibility(false);
        Disable();
    }

    public void Show()
    {
        GetComponent<SetVisiblity>().SetVisibility(true);
        Enable();
    }
}
