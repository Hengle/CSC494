using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticSphere : MonoBehaviour
{
    /*For all of the voxels parented to the design space, move them towards this magnet if it is within a certain distance of the sphere
     * Stop moving if it is in the same region as a constraint
     * 
     */

    public GameObject magnet;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        radius = magnet.GetComponent<Renderer>().bounds.extents.magnitude;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Tells you if the object is in the field of the magnet and will be affected by it
    public bool IsInField(GameObject object1) {
        float distance = Vector3.Distance(magnet.transform.position, object1.transform.position);
        return distance <= radius;
    }
}
