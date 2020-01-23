using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class VoxelRenderer : MonoBehaviour
{
    // Editable attributes
    [Range(0.0001f, 0.2f)] public float size;
    public Shader shader;
    // Private attributes
    Material material;

    Vector4[] points, colours;
    
    ComputeBuffer pointsBuffer, colourBuffer;

    public GameObject Voxels;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        material.EnableKeyword("_COMPUTE_BUFFER");

        List <GameObject> children = new List<GameObject>();
        foreach (Transform child in Voxels.transform) {
            children.Add(child.gameObject);
        }
        
        int N = children.Count;
        points = new Vector4[N];
        colours = new Vector4[N];

        for (int i = 0; i < N; i++)
        {
            points[i] = children[i].transform.position;
            points[i].w = 1f;

            Color c = children[i].GetComponent<Renderer>().material.color;
            colours[i] = new Vector4(c.r, c.g, c.b, c.a);
        }
        pointsBuffer = new ComputeBuffer(N, 4 * sizeof(float));
        pointsBuffer.SetData(points);

        colourBuffer = new ComputeBuffer(N, 4 * sizeof(float));
        colourBuffer.SetData(colours);

        /*
        int N = 100_000;
        points = new Vector4[N];
        colors = new Vector4[N];

        for (int i = 0; i < N; i++)
        {
            // (-0.5, 0.5)

            points[i] = 20f * f(100 * (float) i / N);
            points[i].w = 1f;

            Color c = Color.HSVToRGB((float) i / N, 0.7f, 0.8f);
            colors[i] = new Vector4(c.r, c.g, c.b, c.a);
        }

        pointsBuffer = new ComputeBuffer(N, 4 * sizeof(float));
        pointsBuffer.SetData(points);

        colorBuffer = new ComputeBuffer(N, 4 * sizeof(float));
        colorBuffer.SetData(colors);
        */
    }

    Vector3 f(float u)
    {
        return new Vector3(Mathf.Sin(u), Mathf.Cos(u * u), Mathf.Sin(2 * u));
    }

    void OnRenderObject()
    {
        // Check the camera condition.
        Camera camera = Camera.current;
        if ((camera.cullingMask & (1 << gameObject.layer)) == 0) return;
        if (camera.name == "Preview Scene Camera") return;

        // Draw points
        material.SetPass(0);
        //material.SetMatrix("_Transform", transform.localToWorldMatrix);
        material.SetMatrix("_Transform", Matrix4x4.identity);
        material.SetBuffer("_PositionBuffer", pointsBuffer);
        material.SetBuffer("_ColorBuffer", colourBuffer);
        material.SetFloat("_PointSize", size);

        Graphics.DrawProceduralNow(MeshTopology.Points, pointsBuffer.count, 1);
    }

    private void OnDestroy()
    {
        colourBuffer.Dispose();
        pointsBuffer.Dispose();
    }
}
