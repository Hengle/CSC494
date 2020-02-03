using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colour_changer : MonoBehaviour
{
    public GameObject cube;
    public GameObject point;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get the Renderer component from the new cube
        var cubeRenderer = cube.GetComponent<Renderer>();

        //Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", new Vector4(point.GetComponent<Transform>().position.x*(float)5.0, point.GetComponent<Transform>().position.y * (float)5.0, point.GetComponent<Transform>().position.z * (float)5.0, (float)(1.0)));
    }
}
