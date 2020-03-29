using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToCameraView : MonoBehaviour
{
    public Camera objToFollow;
    public OVRInput.Controller LeftController;
    public OVRInput.Controller RightController;
    //public MeshRenderer renderer;
    // Start is called before the first frame update
    Vector3 offset;
    void Start()
    {
        offset = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, LeftController) > 0.0f && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, RightController) > 0.0f)
        {

            this.transform.position = objToFollow.transform.position + objToFollow.transform.forward;
            this.transform.rotation = new Quaternion(0.0f, objToFollow.transform.rotation.y, 0.0f, objToFollow.transform.rotation.w);

        }
    }
}
