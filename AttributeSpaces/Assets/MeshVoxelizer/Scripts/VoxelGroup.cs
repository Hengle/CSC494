using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshVoxelizerUtil
{
    public enum UVConversion
    {
        None, SourceMesh, VoxelMesh
    }

    [ExecuteInEditMode]
    public class VoxelGroup : MonoBehaviour
    {
        public Mesh voxelMesh = null;
        public Material[] voxelMaterials;
        public Material centerMaterial;
        public UVConversion uvType = UVConversion.SourceMesh;
        [Range(0.0f, 1.0f)] public float voxelScale;
        [HideInInspector] public float ratio = 1.0f;
        [HideInInspector] public Voxel[] voxels;
        [HideInInspector] public Vector3[] voxelPosition;
        [HideInInspector] public Vector2[] uvs;
        [HideInInspector] public int[] submesh;
        [HideInInspector] public GameObject[] centerVoxels;
        [HideInInspector] public Vector3[] centerVoxelPosition;

        Mesh m_mesh = null;

        private void Awake()
        {
            m_mesh = new Mesh();
        }

        void Start()
        {
            RebuildVoxels();
        }

        public void RebuildVoxels()
        {
            if (voxelMesh == null) return;
            UpdateVoxel();
            if (uvType == UVConversion.SourceMesh)
            {
                for (int i = 0; i < voxels.Length; ++i)
                {
                    if (voxels[i] == null) { CreateVoxel(i); }
                    voxels[i].UpdateVoxel(m_mesh, uvs[i]);
                }
            }
            else
            {
                for (int i = 0; i < voxels.Length; ++i)
                {
                    if (voxels[i] == null) { CreateVoxel(i); }
                    voxels[i].GetComponent<MeshFilter>().sharedMesh = m_mesh;
                }
            }
            if (centerVoxels != null)
            {
                for (int i = 0; i < centerVoxels.Length; ++i)
                {
                    if (centerVoxels[i] == null) { CreateCenterVoxel(i); }
                    centerVoxels[i].GetComponent<MeshRenderer>().sharedMaterial = centerMaterial;
                }
            }
        }

        public void ResetVoxels()
        {
            for (int i = 0; i < voxels.Length; ++i)
            {
                if (voxels[i] == null) continue;
                voxels[i].transform.localPosition = voxelPosition[i];
                voxels[i].transform.localScale = Vector3.one;
                voxels[i].transform.localRotation = Quaternion.identity;
            }
            if (centerVoxels != null)
            {
                for (int i = 0; i < centerVoxels.Length; ++i)
                {
                    if (centerVoxels[i] == null) continue;
                    centerVoxels[i].transform.localPosition = centerVoxelPosition[i];
                    centerVoxels[i].transform.localScale = Vector3.one;
                    centerVoxels[i].transform.localRotation = Quaternion.identity;
                }
            }
        }

        public void CreateVoxel(int i)
        {
            GameObject voxelObject = new GameObject("voxel");
            voxelObject.AddComponent<MeshFilter>();
            voxelObject.AddComponent<MeshRenderer>().sharedMaterial = voxelMaterials[submesh[i]];
            voxelObject.transform.parent = transform;
            voxelObject.transform.localPosition = voxelPosition[i];
            voxels[i] = voxelObject.AddComponent<Voxel>();
        }

        public void CreateCenterVoxel(int i)
        {
            GameObject voxelObject = new GameObject("center voxel");
            voxelObject.AddComponent<MeshFilter>().sharedMesh = m_mesh;
            voxelObject.AddComponent<MeshRenderer>().sharedMaterial = centerMaterial;
            voxelObject.transform.parent = transform;
            voxelObject.transform.localPosition = centerVoxelPosition[i];
            centerVoxels[i] = voxelObject;
        }

        void UpdateVoxel()
        {
            m_mesh.Clear();
            Vector3[] vertices = voxelMesh.vertices;
            Vector2[] uvs = voxelMesh.uv;
            float r = ratio * voxelScale;
            if (uvType == UVConversion.None)
            {
                for (int i = 0; i < vertices.Length; ++i)
                {
                    vertices[i] *= r;
                    uvs[i] = Vector2.zero;
                }
            }
            else
            {
                for (int i = 0; i < vertices.Length; ++i)
                {
                    vertices[i] *= r;
                }
            }
            m_mesh.vertices = vertices;
            m_mesh.uv = uvs;
            m_mesh.normals = voxelMesh.normals;
            m_mesh.triangles = voxelMesh.triangles;
        }

        private void OnDestroy()
        {
            DestroyImmediate(m_mesh);
        }
    }
}