using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum AttributeType
{
    //All of the possible attributes that we want
    RedRGB, GreenRGB, BlueRGB, ScaleShiftX, ScaleShiftY, ScaleShiftZ, RedShift, GreenShift, BlueShift, NumLeaves, Duplicates

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
    public AttributeType attributeType;

    private void Start()
    {
        defaultMin = 0f;
        defaultMax = 1.0f;
        min = 0f;
        max = 1.0f;
    }
    public void applyAttributeChange(Proxy proxy, Axis x_attr, int axisInd, float newValue) {
        /* Inputs: 
         * proxy: The proxy on which the changes will be applied
         * int axisInd: the index of the axis that this attribute is on
         * newValue: the new value to set this attribute to have 
         */
        //Default the attribute to a random attribute
        
        Vector3 currentPosition = proxy.transform.localPosition;

        //This switch allows you to have multiple actions depending on the type of the attribute
        switch (attributeType) {
            case AttributeType.ScaleShiftX:
                GetComponent<TextMesh>().text = "Scale Shift X";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the x value of the scale
                //scale = original bounding box x
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetX(proxy.original_backup.transform.localScale.x + newValue);
                break;

            case AttributeType.ScaleShiftY:
                GetComponent<TextMesh>().text = "Scale Shift Y";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the y value of the scale
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetY(proxy.original_backup.transform.localScale.y + newValue);
                break;

            case AttributeType.ScaleShiftZ:
                GetComponent<TextMesh>().text = "Scale Shift Z";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Just change the z value of the scale
                proxy.original.transform.localScale = proxy.original.transform.localScale.SetZ(proxy.original_backup.transform.localScale.z + newValue);
                break;

            case AttributeType.RedShift:
                GetComponent<TextMesh>().text = "Red Shift";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //Set the proxy to look the same as the original
                proxy.original.GetComponent<MeshRenderer>().material.SetFloat("_DeltaRed", newValue);
                proxy.GetComponent<MeshRenderer>().material.SetFloat("_DeltaRed", newValue);
                break;
            case AttributeType.GreenShift:
                GetComponent<TextMesh>().text = "Green Shift";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                proxy.original.GetComponent<MeshRenderer>().material.SetFloat("_DeltaGreen", newValue);
                proxy.GetComponent<MeshRenderer>().material.SetFloat("_DeltaGreen", newValue);
                break;
            case AttributeType.BlueShift:
                GetComponent<TextMesh>().text = "Blue Shift";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                proxy.original.GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", newValue);
                proxy.GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", newValue);
                break;
            case AttributeType.NumLeaves:
                //Adds a bunch of duplicates to the scene at a regular distance
                GetComponent<TextMesh>().text = "# Leaves";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                //TODO write a script that gets called to increase the # of leaves on a plant. Maybe just fake it by unhiding the leaves in a list in order?
                break;
            case AttributeType.Duplicates:
                //Adds a bunch of duplicates to the scene at a regular distance
                GetComponent<TextMesh>().text = "Duplicates";
                defaultMin = 0f;
                defaultMax = 1.0f;
                min = 0f;
                max = 1.0f;
                proxy.original.GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", newValue);
                proxy.GetComponent<MeshRenderer>().material.SetFloat("_DeltaBlue", newValue);
                break;
            default:
                break;
        }

    }
    public float getCurrentValue(GameObject originalObject, Proxy proxy=null)
    {
        switch (attributeType)
        {
            case AttributeType.ScaleShiftX:
                //Just change the x value of the scale
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.x: 0f;

            case AttributeType.ScaleShiftY:
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.y : 0f;

            case AttributeType.ScaleShiftZ:
                return originalObject.GetComponent<Renderer>() ? originalObject.GetComponent<MeshFilter>().mesh.bounds.size.z : 0f;

            case AttributeType.RedShift:
                return originalObject.GetComponent<MeshRenderer>() ? proxy.GetComponent<MeshRenderer>().material.color.r + originalObject.GetComponent<MeshRenderer>().material.GetFloat("_DeltaRed"): 0f;
                break;
            case AttributeType.GreenShift:
                return originalObject.GetComponent<MeshRenderer>() ? proxy.GetComponent<MeshRenderer>().material.color.g + originalObject.GetComponent<MeshRenderer>().material.GetFloat("_DeltaGreen") : 0f;
                break;
            case AttributeType.BlueShift:
                return originalObject.GetComponent<MeshRenderer>() ? proxy.GetComponent<MeshRenderer>().material.color.b + originalObject.GetComponent<MeshRenderer>().material.GetFloat("_DeltaBlue") : 0f;
                break;
            default:
                return 0f;
                break;
        }
    }

}


