using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Voxels : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Go through each renderer
        foreach (Transform voxel in transform) {
            MeshRenderer renderer = voxel.GetComponent<MeshRenderer>();
            MeshFilter meshFilter = voxel.GetComponent<MeshFilter>();
            Material material = renderer.materials[0];
            Texture2D texture = material.GetTexture("_MainTex") as Texture2D;

            Mesh mesh = meshFilter.mesh;

            //UV mapping: https://answers.unity.com/questions/516935/how-does-uv-mapping-via-script-work.html
            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(0, uvs);

            Vector2 uv = uvs[0];
            //uv.x -= 0.5f;
            //uv.y -= 0.5f;

            //uv *= 2f;

            Color color = texture.GetPixel((int) (texture.width * uv.x), (int) (texture.height * uv.y));

            //Save the current shader
            Shader shader = renderer.material.shader;
            float glossiness = material.GetFloat("_Glossiness");
            float metallic = material.GetFloat("_Metallic");

            //Remove the current material
            Destroy(renderer.GetComponent<Material>());

            print(color*255.0f);

            //Set the colour of the material
            Material newMaterial = new Material(shader);
            newMaterial.color = color;
            newMaterial.SetFloat("_Glossiness", glossiness);
            newMaterial.SetFloat("_Metallic", metallic);

            Material[] mats = renderer.materials;
            int numMats = mats.Length;
            mats[numMats-1] = newMaterial;
            renderer.materials = mats;

            //texture.GetPixel()
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
