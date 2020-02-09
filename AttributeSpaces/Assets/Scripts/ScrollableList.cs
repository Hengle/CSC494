using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

public delegate void OnIndexChangedEvent();

public class ScrollableList : MonoBehaviour
{
    public TextMesh textMesh, chevronMesh;
    public OVRGrabbable grabbable;
    public Transform background;
    public string[] items;

    BoxCollider grabbableCollider;

    bool grabbed = false;

    // Scroll properties
    float increment = 0.036f;

    // Item properties
    [NonSerialized] public int index = 0;

    // Hand that grabbed most recently (to provide haptic feedback)
    HandController hand;
    HandController left, right;

    bool withinProximity = false;

    public OnIndexChangedEvent OnIndexChanged;

    // Start is called before the first frame update
    void Start()
    {
        grabbable.OnGrabbed += Grabbed;
        grabbable.OnReleased += Released;

        UpdateText();

        left = PlayerController.instance.leftHand;
        right = PlayerController.instance.rightHand;

        grabbableCollider = grabbable.GetComponent<BoxCollider>();
    }

    public void SetItems(string[] items)
    {
        this.items = items;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed) 
        {            
            textMesh.transform.position = grabbable.transform.position;
            textMesh.transform.localPosition = textMesh.transform.localPosition.SetX(0f);
            textMesh.transform.localPosition = textMesh.transform.localPosition.SetZ(0f);
        }
        else
        {
            grabbable.transform.position = textMesh.transform.position;
            grabbable.transform.rotation = textMesh.transform.rotation;

            grabbableCollider.center = grabbableCollider.center.SetY(-textMesh.transform.localPosition.y / grabbable.transform.localScale.y);
        }

        
        float y = textMesh.transform.localPosition.y;
        y = RoundToClosest(y, increment);

        // Snap y if not grabbing
        if (!grabbed)
            textMesh.transform.localPosition = math.lerp(textMesh.transform.localPosition, textMesh.transform.localPosition.SetY(y), 0.1f);

        int newIndex = (int) math.round(y / increment);
        newIndex = math.clamp(newIndex, 0, items.Length - 1);

        SetIndex(newIndex);

        bool newWithinProximity = (!(left.grabber.grabbedObject != null) && WithinProximity(left.transform.position)) || 
                                  (!(right.grabber.grabbedObject != null) && WithinProximity(right.transform.position));
        
        withinProximity = newWithinProximity;

        textMesh.transform.localPosition = textMesh.transform.localPosition.SetY(math.clamp(textMesh.transform.localPosition.y, 0f, (items.Length - 1) * increment));

        // Fade out if hand is not close
        if (withinProximity) 
        {
            textMesh.SetAlpha(0.05f);
        }
        else
        {
            textMesh.SetAlpha(0f);
        }
    }

    void LateUpdate()
    {

    }

    void Grabbed(OVRGrabber hand, Collider collider) 
    {
        if (!withinProximity) return;

        this.hand = hand.GetComponent<HandController>();
        grabbed = true;
    }

    void Released(Vector3 linearVelocity, Vector3 angularVelocity) 
    {
        grabbed = false;
    }

    float RoundToClosest(float x, float t) 
    {
        float down = x - x % t;
        float up = down + t;
        return x - down <= up - x ? down : up;
    }

    void SetIndex(int i) 
    {
        if (i != index)
        {
            index = i;

            UpdateText();
            //hand?.Vibrate(0.1f, 0.2f);

            if (OnIndexChanged != null) OnIndexChanged();
        }
    }

    void UpdateText() 
    {
        string text = "";

        for (int i = 0; i < items.Length; i++) 
        {
            if (i == index)
            {
                text += $"<color=0xfff>{items[i]}</color>\n";
            }
            else
            {
                text += $"{items[i]}\n";
            }
        }

        textMesh.text = text;
    }

    bool WithinProximity(float3 position)
    {
        return grabbed || math.distancesq(transform.position, position) < 0.02f;
    }

    public string SelectedItem() 
    {
        return items[index];
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

    public void Disable()
    {
        enabled = false;
        grabbable.enabled = false;
    }

    public void Enable()
    {
        enabled = true;
        grabbable.enabled = true;
    }
}
