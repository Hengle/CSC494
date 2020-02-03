using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utilities {

public static class VectorUtilities 
{
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        if (a == b) return 0f;
        
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return math.dot(AV, AB) / math.dot(AB, AB);
    }

    public static Vector3 Round(this Vector3 v, float decimalPlace = 1)
    {
        return new Vector3(math.round(v.x * decimalPlace) / decimalPlace, 
                          math.round(v.y * decimalPlace) / decimalPlace, 
                          math.round(v.z * decimalPlace) / decimalPlace);
    }


    public static Vector3 SetX(this Vector3 v, float x) 
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y) 
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z) 
    {
        return new Vector3(v.x, v.y, z);
    }
    public static bool OneOf(this bool3 b) 
    {
        int x = b.x ? 1 : 0;
        int y = b.y ? 1 : 0;
        int z = b.z ? 1 : 0;

        return x + y + z == 1;
    }

    public static Vector3 random(Vector3 a, Vector3 b)
    {
        return new Vector3(UnityEngine.Random.Range(a.x, b.x), 
                          UnityEngine.Random.Range(a.y, b.y), 
                          UnityEngine.Random.Range(a.z, b.z));
    }

    public static Vector3[] ToVector3(this Vector3[] values) 
    {
        Vector3[] output = new Vector3[values.Length];
        
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = values[i];
        }

        return output;
    }

    public static Vector3 pow3(Vector3 v, int p) 
    {
        return new Vector3(math.pow(v.x, p), math.pow(v.y, p), math.pow(v.z, p));
    }

    public static float max3(Vector3 v) 
    {
        return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
    }

}}