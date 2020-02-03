using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities {

public static class TransformUtilities 
{
    public static void ScaleAround(this Transform transform, Vector3 pivot, Vector3 scale)
    {
        transform.position = (scale.x / transform.localScale.x) * (transform.position - pivot) + pivot; 
        transform.localScale = scale;
    }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3 (globalScale.x / Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.000001f), 
                                            globalScale.y / Mathf.Max(Mathf.Abs(transform.lossyScale.y), 0.000001f),  
                                            globalScale.z / Mathf.Max(Mathf.Abs(transform.lossyScale.z), 0.000001f));
    }

    public static void AnimatePosition(this Transform obj, float3 position) 
    {
        AnimationHelper animationHelper = AnimationHelper.instance;
        animationHelper.positions.AddOrSet(obj, position);
    }

    public static void AnimateScale(this Transform obj, float3 scale) 
    {
        AnimationHelper animationHelper = AnimationHelper.instance;
        animationHelper.scales.AddOrSet(obj, scale);
    }

    public static void AnimateRotation(this Transform obj, quaternion rotation) 
    {
        AnimationHelper animationHelper = AnimationHelper.instance;
        animationHelper.rotations.AddOrSet(obj, rotation);
    }

    public static void AnimateLocalPosition(this Transform obj, float3 position) 
    {
        AnimationHelper animationHelper = AnimationHelper.instance;
        animationHelper.localPositions.AddOrSet(obj, position);
    }

    public static void AnimateLocalRotation(this Transform obj, quaternion rotation) 
    {
        AnimationHelper animationHelper = AnimationHelper.instance;
        animationHelper.localRotations.AddOrSet(obj, rotation);
    }

    public static void AnimatePosition(this MonoBehaviour obj, float3 position)     => AnimatePosition(obj.transform, position);
    public static void AnimateScale   (this MonoBehaviour obj, float3 scale)        => AnimateScale(obj.transform, scale);
    public static void AnimateRotation(this MonoBehaviour obj, quaternion rotation) => AnimateRotation(obj.transform, rotation);

    public static void AnimateLocalPosition(this MonoBehaviour obj, float3 position)     => AnimateLocalPosition(obj.transform, position);
    public static void AnimateLocalRotation(this MonoBehaviour obj, quaternion rotation) => AnimateLocalRotation(obj.transform, rotation);

}


public class AnimationHelper : MonoBehaviour 
{
    public static AnimationHelper instance {get => _instance ?? new GameObject().AddComponent<AnimationHelper>();}
    static AnimationHelper _instance = null;

    [NonSerialized] public Dictionary<Transform, float3> positions = new Dictionary<Transform, float3>();
    [NonSerialized] public Dictionary<Transform, float3> scales = new Dictionary<Transform, float3>();
    [NonSerialized] public Dictionary<Transform, quaternion> rotations = new Dictionary<Transform, quaternion>();

    [NonSerialized] public Dictionary<Transform, float3> localPositions = new Dictionary<Transform, float3>();
    [NonSerialized] public Dictionary<Transform, quaternion> localRotations = new Dictionary<Transform, quaternion>();

    void Awake()
    {
        _instance = this;
        name = "Animation Helper";
    }

    void Update()
    {
        UpdatePositions();
        UpdateScales();
        UpdateRotations();

        UpdateLocalPositions();
        UpdateLocalRotations();
    }

    void UpdatePositions()
    {
        List<Transform> completed = new List<Transform>();

        foreach ((Transform tf, float3 target) in positions)
        {
            if (tf == null) 
            {
                completed.Add(tf);
                continue;
            }
            
            tf.position = math.lerp(tf.position, target, 0.1f);

            if (math.lengthsq((float3) tf.position - target) < 1e-4)
            {
                tf.position = target;
                completed.Add(tf);
            }
        }

        foreach (Transform tf in completed)
        {
            positions.Remove(tf);
        }
    }

    void UpdateScales()
    {
        List<Transform> completed = new List<Transform>();

        foreach ((Transform tf, float3 target) in scales)
        {
            if (tf == null) 
            {
                completed.Add(tf);
                continue;
            }

            tf.localScale = math.lerp(tf.localScale, target, 0.1f);

            if (math.lengthsq((float3) tf.localScale - target) < 1e-4)
            {
                tf.localScale = target;
                completed.Add(tf);
            }
        }

        foreach (Transform trans in completed)
        {
            scales.Remove(trans);
        }
    }

    void UpdateRotations()
    {
        List<Transform> completed = new List<Transform>();

        foreach ((Transform tf, Quaternion target) in rotations)
        {
            if (tf == null) 
            {
                completed.Add(tf);
                continue;
            }

            tf.rotation = math.slerp(tf.rotation, target, 0.1f);

            if (Quaternion.Angle(tf.rotation, target) < 2f)
            {
                transform.rotation = target;
                completed.Add(tf);
            }
        }

        foreach (Transform tf in completed)
        {
            rotations.Remove(tf);
        }
    }

    void UpdateLocalPositions()
    {
        List<Transform> completed = new List<Transform>();

        foreach ((Transform tf, float3 target) in localPositions)
        {
            if (tf == null) 
            {
                completed.Add(tf);
                continue;
            }

            tf.localPosition = math.lerp(tf.localPosition, target, 0.1f);

            if (math.lengthsq((float3) tf.localPosition - target) < 1e-4)
            {
                tf.localPosition = target;
                completed.Add(tf);
            }
        }

        foreach (Transform tf in completed)
        {
            localPositions.Remove(tf);
        }
    }

    void UpdateLocalRotations()
    {
        List<Transform> completed = new List<Transform>();

        foreach ((Transform tf, Quaternion target) in localRotations)
        {
            if (tf == null) 
            {
                completed.Add(tf);
                continue;
            }

            tf.localRotation = math.slerp(tf.localRotation, target, 0.1f);

            if (Quaternion.Angle(tf.localRotation, target) < 2f)
            {
                transform.localRotation = target;
                completed.Add(tf);
            }
        }

        foreach (Transform tf in completed)
        {
            localRotations.Remove(tf);
        }
    }
}

}