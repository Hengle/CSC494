using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


/*
public interface IAttribute
{
    //The default min and max are the limits intrinsic to the attribute
    //For example, it's 0 to 1 for RGB values
    float defaultMin { get; set; }
    float defaultMax { get; set; }

    //These are the movable bounds for the object
    float min { get; set; }
    float max { get; set; }

    void SetAttributeToValue(float attrValue);
}

    //Implementing
    //float IAttribute.defaultMin
    //{
    //    get { return 0f; }
    //    set { defaultMin = value; }
    //}
 */

public enum AttributeType
{
    //All of the possible attributes that we want
    RedRGB, GreenRGB, BlueRGB, ScaleShiftX, ScaleShiftY, ScaleShiftZ, RedShift, GreenShift, BlueShift,

    /*Each SceneObject has a list of Attributes
     * Each Attribute is a monobehaviour with an enum that tells you which specific one it is
     */
}
public class Attribute : MonoBehaviour
{
    //Enums let you choose which attribute it is 

    public float defaultMin;
    public float defaultMax;
    public float min;
    public float max;
    public AttributeType attribute;

    private void Start()
    {
        defaultMin = 0f;
        defaultMax = 1.0f;
        min = 0f;
        max = 1.0f;
        //attribute = AttributeType.ScaleShiftX;
    }
    public void applyAttributeChange(Proxy proxy, Axis x_attr, int axisInd, float newValue) {
        /* Inputs: 
         * proxy: The proxy on which the changes will be applied
         * int axisInd: the index of the axis that this attribute is on
         * newValue: the new value to set this attribute to have 
         */
        //Default the attribute to a random attribute
        
        Vector3 currentPosition = proxy.transform.localPosition;

        print("Called with: " + attribute);

        //This switch allows you to have multiple actions depending on the type of the attribute
        switch (attribute) {
            case AttributeType.ScaleShiftX:
                print("Case 1");
                GetComponent<TextMesh>().text = "Scale Shift X";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the x value of the scale
                //scale = original bounding box x
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetX(newValue);
                print("X axis!!!");
                break;

            case AttributeType.ScaleShiftY:
                GetComponent<TextMesh>().text = "Scale Shift Y";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the x value of the scale
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetY(newValue);
                break;

            case AttributeType.ScaleShiftZ:
                GetComponent<TextMesh>().text = "Scale Shift Z";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the x value of the scale
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetZ(newValue);
                break;

            case AttributeType.GreenRGB:
                break;
            default:
                break;
        }

    }
    public float getCurrentValue(GameObject originalObject)
    {
        switch (attribute)
        {
            case AttributeType.ScaleShiftX:
                //Just change the x value of the scale
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.x: 0f;

            case AttributeType.ScaleShiftY:
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.y : 0f;

            case AttributeType.ScaleShiftZ:
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.z : 0f;

            case AttributeType.GreenRGB:
                return 0f;
                break;
            default:
                return 0f;
                break;
        }
    }

}


