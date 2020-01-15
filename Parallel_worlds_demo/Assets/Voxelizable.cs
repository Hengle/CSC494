using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxelizable : MonoBehaviour
{
    public Voxels voxels;
    new MeshRenderer renderer;
    /* For the grabbing, maybe just write your own thing to change the position of the voxel in the position list if it's within a certain distance of the cursor
     * Get rid of the FlyToDesignSpace script for now
     * 
     * 
     * Combine this and Voxels.
     * When the program first starts up, voxelize everything and then parent them to the original object. 
     * Add a renderer as a child of the original object
     * Loop through all of the children and save the position and colour into a list to be used in the renderer
     * Destroy the voxels to speed things up
     * 
     * 
     * 
     */



    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        Texture2D texture = (Texture2D)material.GetTexture("_MainTex");

        Texture2D newTexture = new Texture2D(texture.width, texture.height);

        print(texture.GetPixel(0, 0) + " " + texture.width + " " + texture.height);

        Color[] pixels = texture.GetPixels();
        //print(texture.GetPixel(0, 0));
        //for (int i = 0; i < pixels.Length; i++)
        //{
        //    pixels[i].r *= 0.5f;
        //    pixels[i].g *= 0.5f;
        //    pixels[i].b *= 0.5f;
        //}

        
        newTexture.SetPixels(pixels);
        newTexture.Apply();
        material.SetTexture("_MainTex", newTexture);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
