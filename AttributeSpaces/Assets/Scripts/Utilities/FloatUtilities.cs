using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utilities {

public static class FloatUtilities 
{
    public static float Map(this float value, float a, float b, float c, float d)
    {
        return (value - a) * (d - c) / (b - a) + c;
    }

    public static float Normalize(this float value, float a, float b)
    {
        return (value - a) * (b - a);
    }

    public static int RoundOff(this float x, int interval) 
    {
        int number = (int) x;
        int remainder = number % interval;
        number += (remainder < interval / 2) ? -remainder : (interval - remainder);
        return number;
    }
}}