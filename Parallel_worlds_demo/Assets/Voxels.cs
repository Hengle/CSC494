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

            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(0, uvs);

            Vector2 uv = uvs[0];
            uv.x -= 0.5f;
            uv.y -= 0.5f;

            uv *= 2f;

            Color color = texture.GetPixel((int) (texture.width * uv.x), (int) (texture.height * uv.y));

            //Set the colour to something 
            //renderer.material = new Material(shader);
            //renderer.material.SetColor("_Color", Color.red);



            //texture.GetPixel()
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
