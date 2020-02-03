using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToDesignSpace : MonoBehaviour
{

    public GameObject DesignSpace;
    float speed;
    List<Transform> voxels = new List<Transform>();
    List<Vector3> colors = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {

        foreach (Transform child in transform)
        {
            voxels.Add(child);
            Color color = child.GetComponent<MeshRenderer>().material.color;
            float R = color.r;
            float G = color.g;
            float B = color.b;
            colors.Add(new Vector3(R, G, B));
        }

        foreach (Transform child in voxels)
        {
            child.SetParent(DesignSpace.transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        for (int i=0; i<voxels.Count; i++)
        {
            
            Transform child = voxels[i];
            
            speed = 0.1f;

            Vector3 colorLocation = colors[i];
            Vector3 newlocation = Vector3.Lerp(child.localPosition, colorLocation, speed);

            /*
             *Old version of the lerp that moves everything by the same speed
            //lerp the position
            child.localPosition = newlocation;
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 0.5f, speed);
            */
            //Move each point a different amount based on the distance from the design space
            float dist = Vector3.Distance(child.localPosition, colorLocation);
            float t = Mathf.Clamp(Mathf.Exp(-5 * dist), 0.05f, 0.2f);
            child.localPosition = Vector3.Lerp(child.localPosition, colorLocation, t);

        }
    }
}
