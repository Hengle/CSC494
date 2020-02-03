using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class RecordingCamera : MonoBehaviour
{
    Transform main;

    void Start()
    {
        main = Camera.main.transform;
    }

    void Update()
    {
        transform.position = math.lerp(transform.position, main.position, 0.2f);
        transform.rotation = math.slerp(transform.rotation, main.rotation, 0.2f);
    }
}
