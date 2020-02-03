using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

// Maintains a global transform scale of one (for texts, use font-size or character-size to vary size)
[ExecuteInEditMode()]
public class MaintainGlobalScale : MonoBehaviour
{
    bool facingCamera;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.lossyScale != Vector3.one)
        {
            transform.SetGlobalScale(Vector3.one);

            if (facingCamera)
            {
                transform.localScale = transform.localScale.SetX(-math.abs(transform.localScale.x));
            }
        }
    }
}
