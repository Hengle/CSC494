using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Voxels : MonoBehaviour
{
    public Material referenceMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        // Go through each renderer
        foreach (Transform voxel in transform) {
            MeshRenderer renderer = voxel.GetComponent<MeshRenderer>();
            //renderer.enabled = false;
            MeshFilter meshFilter = voxel.GetComponent<MeshFilter>();
            Material material = renderer.materials[0];
            Texture2D texture = material.GetTexture("_MainTex") as Texture2D;

            Mesh mesh = meshFilter.mesh;

            //UV mapping: https://answers.unity.com/questions/516935/how-does-uv-mapping-via-script-work.html
            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(0, uvs);

            Vector2 uv = uvs[0];

            Color color = texture.GetPixel((int) (texture.width * uv.x), (int) (texture.height * uv.y));
            color.a = 0.3f;

            //Save the current shader
            //Shader shader = renderer.material.shader;
            //float glossiness = material.GetFloat("_Glossiness");
            //float metallic = material.GetFloat("_Metallic");

            //Set the colour of the material -- use the fade material so that you can lerp the alpha when you make the cubes fly towards you
            Material newMaterial = new Material(referenceMaterial);
            //color.a = 0.0f;
            newMaterial.color = color;
            //newMaterial.SetFloat("_Glossiness", glossiness);
            //newMaterial.SetFloat("_Metallic", metallic);

            renderer.material = newMaterial;

            //texture.GetPixel()
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
