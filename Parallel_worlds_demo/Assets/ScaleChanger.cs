using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleChanger : MonoBehaviour
{
    public GameObject origin;
    public GameObject cube;
    public GameObject point;
    //public MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Get the Renderer component from the new cube
        var cubeRenderer = cube.GetComponent<Renderer>();
        var x = point.transform.localPosition.x;
        var y = point.transform.localPosition.y;
        var z = point.transform.localPosition.x;

        cube.transform.localScale = new Vector3(x, y, z);
        //Call SetColor using the shader property name "_Color" and setting the color to red
        //cubeRenderer.material.SetColor("_Color", new Vector4(point.GetComponent<Transform>().position.x * (float)5.0, point.GetComponent<Transform>().position.y * (float)5.0, point.GetComponent<Transform>().position.z * (float)5.0, (float)(1.0)));
    }
}
