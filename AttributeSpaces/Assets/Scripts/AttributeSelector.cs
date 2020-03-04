using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSelector : MonoBehaviour
{
    public ScrollableList attributeSelection;
    public GameObject GrabbableAttribute;
    public GameObject SecondaryGrabbableAttribute;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit(Collider other)
    {
        //Only do this if the gameobject that's passing through is the grabbable component
        if (other.gameObject == GrabbableAttribute) {

            string attr_string = attributeSelection.SelectedItem();
            GrabbableAttribute.GetComponentInChildren<TextMesh>().text = attr_string;

            //Now set the Attribute portion of the object
            if (attr_string == "Scale X")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.ScaleShiftX;
            }
            else if (attr_string == "Scale Y")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.ScaleShiftY;
            }
            else if (attr_string == "Scale Z")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.ScaleShiftZ;
            }
            else if (attr_string == "Red Shift")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.RedShift;
            }
            else if (attr_string == "Green Shift")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.GreenShift;
            }
            else if (attr_string == "Blue Shift")
            {
                GrabbableAttribute.GetComponent<GrabbableAttribute>().attributeLabel.attributeType = AttributeType.BlueShift;
            }


            //Duplicate another of the grabbable objects under the same parent so that you never run out of them
            GameObject clonedGrabbableAttribute = Instantiate(SecondaryGrabbableAttribute, SecondaryGrabbableAttribute.transform.position, SecondaryGrabbableAttribute.transform.rotation, SecondaryGrabbableAttribute.transform.parent);

            //Secondary grabbable attribute, it's your time to shine!
            SecondaryGrabbableAttribute.GetComponent<OVRGrabbable>().enabled = true;
            SecondaryGrabbableAttribute.GetComponent<BoxCollider>().enabled = true;

            //The previously secondary atribute becomes the primary one
            GrabbableAttribute = SecondaryGrabbableAttribute;
            SecondaryGrabbableAttribute = clonedGrabbableAttribute;




        }


    }
}
