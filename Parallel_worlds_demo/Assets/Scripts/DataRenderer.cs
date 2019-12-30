using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class DataRenderer : MonoBehaviour
{
    // Editable attributes
    [Range(0.0001f, 0.2f)] public float size;
    public Shader shader;
    // Private attributes
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        material.EnableKeyword("_COMPUTE_BUFFER");
    }

    void OnRenderObject()
    {
        // Check the camera condition.
        Camera camera = Camera.current;
        if ((camera.cullingMask & (1 << gameObject.layer)) == 0) return;
        if (camera.name == "Preview Scene Camera") return;

        // Draw points
        material.SetPass(0);
        material.SetMatrix("_Transform", transform.localToWorldMatrix);
        //material.SetBuffer("_PositionBuffer", subset.positionBuffer);
        //material.SetBuffer("_ColorBuffer", subset.colorBuffer);
        material.SetFloat("_PointSize", size);

        //Graphics.DrawProceduralNow(MeshTopology.Points, subset.positionBuffer.count, 1);
    }
}
