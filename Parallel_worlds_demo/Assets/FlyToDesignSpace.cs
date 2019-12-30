using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToDesignSpace : MonoBehaviour
{
    public GameObject original;
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
            child.SetParent(DesignSpace.transform);

    }
    // Update is called once per frame
    void Update()
    {
        
        for (int i=0; i<voxels.Count; i++)
        {
            Transform child = voxels[i];
            speed = 0.05f;
            //speed = 2.0f * Time.deltaTime;

            //Determine where to put the item in the design space using the r g b components
            //Material material = child.GetComponent<MeshRenderer>().material;
            //float R = material.color.r;
            //float G = material.color.g;
            //float B = material.color.b;

            //Vector3 colorLocation = new Vector3(R, G, B);
            Vector3 colorLocation = colors[i];
            Vector3 newlocation = Vector3.Lerp(child.localPosition, colorLocation, speed);

            //lerp the position
            child.localPosition = newlocation;
            child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 0.5f, speed);




            //Material material = child.GetComponent<MeshRenderer>().material;
            //Color newColour = material.color;

            //newColour.a = Mathf.Lerp(newColour.a, 1.0f, speed);

            //material.color = newColour;


            //Check if the final location has been reached and parent it to the design space if it has reached it

        }
    }
}
