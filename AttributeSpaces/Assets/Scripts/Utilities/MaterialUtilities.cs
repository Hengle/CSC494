using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utilities {

public static class MaterialUtilities 
{
    public static void SetAlpha(this GameObject obj, float alpha)
    {
        Material material = obj.GetComponent<MeshRenderer>().material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
    }

    public static void SetMaterial(this GameObject obj, Material material)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        renderer.material = material;
    }

    public static void SetAlpha(this TextMesh text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    public static Color Brighten(this Color color, float t) 
    {
        return new Color(color.r * t, color.g * t, color.b * t, color.a);
    }

    public static Color Lighten(this Color color, float t) 
    {
        return new Color(color.r + t, color.g + t, color.b + t, color.a);
    }

    public static Color SetAlpha(this Color color, float a) 
    {
        return new Color(color.r, color.g, color.b, a);
    }
}}