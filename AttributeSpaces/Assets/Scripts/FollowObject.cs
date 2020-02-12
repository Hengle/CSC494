using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    /*
    public GameObject objToFollow;
    public GameObject MovingObj;
    //public MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Get the Renderer component from the new cube
        var cubeRenderer = MovingObj.GetComponent<Renderer>();
        var x = objToFollow.transform.position.x;
        var y = objToFollow.transform.position.y;
        var z = objToFollow.transform.position.x;

        MovingObj.transform.position = new Vector3(x, y, z);
        //Call SetColor using the shader property name "_Color" and setting the color to red
        //cubeRenderer.material.SetColor("_Color", new Vector4(point.GetComponent<Transform>().position.x * (float)5.0, point.GetComponent<Transform>().position.y * (float)5.0, point.GetComponent<Transform>().position.z * (float)5.0, (float)(1.0)));
    }
    */

    public GameObject objToFollow;
    //public MeshRenderer renderer;
    // Start is called before the first frame update
    Vector3 offset;
    void Start()
    {
        offset = objToFollow.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Update with the delta between the movements
        transform.position = objToFollow.transform.position - offset + new Vector3(5f, 0f, 0);
        //Call SetColor using the shader property name "_Color" and setting the color to red
        //cubeRenderer.material.SetColor("_Color", new Vector4(point.GetComponent<Transform>().position.x * (float)5.0, point.GetComponent<Transform>().position.y * (float)5.0, point.GetComponent<Transform>().position.z * (float)5.0, (float)(1.0)));
    }

}
