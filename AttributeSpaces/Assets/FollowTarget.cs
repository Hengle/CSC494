﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}