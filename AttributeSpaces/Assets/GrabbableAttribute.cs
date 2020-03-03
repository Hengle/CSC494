using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GrabbableAttribute : MonoBehaviour
{
    public Attribute attributeLabel;
    public Axis axis;
    // Start is called before the first frame update
    OVRGrabbable grabbable;
    public bool isGrabbed;
    FilterManager filterManager;
    SavedSpaceManager savedSpaceManager;
    void Start()
    {
        filterManager = FilterManager.instance;
        savedSpaceManager = SavedSpaceManager.instance;

        isGrabbed = false;
        grabbable = GetComponent<OVRGrabbable>();
        grabbable.OnGrabbed += (grab, pt) =>
        {
            isGrabbed = true;

        };

        grabbable.OnReleased += (grab, pt) =>
        {

            if (filterManager.hovering.Contains(transform.gameObject))
            {
                this.transform.parent = filterManager.transform.parent;

                //Subtract 1 so that it starts to the right of the user
                this.transform.AnimateLocalPosition(new Vector3((filterManager.attributeFilters.Count - 1) * 0.3f, 0.1f, -0.065f));

                filterManager.attributeFilters.Add(GetComponentInChildren<Attribute>());
                this.transform.localRotation = Quaternion.identity;
                savedSpaceManager.UpdateSpaceContents();
            }
            else if (!filterManager.hovering.Contains(transform.gameObject)) {
                this.gameObject.transform.parent = null;
                filterManager.attributeFilters.Remove(GetComponentInChildren<Attribute>());
                savedSpaceManager.UpdateSpaceContents();
            }
            //Make it parentless if it's still trapped in a selector
            else if (transform.parent && transform.parent.GetComponent<AttributeSelector>())
            {
                this.transform.parent = null;
                axis.transform.parent = null;
            }

            isGrabbed = false;

        };
    }

    // Update is called once per frame
    void Update()
    {

    }

}
