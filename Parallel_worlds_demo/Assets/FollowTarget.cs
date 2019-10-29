using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    public bool position;

    public Vector3 rotation;

    [Range(0, 1)]
    public float speed = 100;

    // Update is called once per frame
    void Update()
    {
        if (position)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed);
        }

        //if (rotation)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speed);
        //}
    }
}
