#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshVoxelizerUtil
{
    public class MeshVoxelizerWindow : EditorWindow
    {
        [MenuItem("Window/Mesh Voxelizer")]
        static void Init()
        {
            MeshVoxelizerWindow window = (MeshVoxelizerWindow)GetWindow(typeof(MeshVoxelizerWindow), false, "Mesh Voxelizer");
            window.Show();
            if (MeshVoxelizer.centerMaterial == null) MeshVoxelizer.centerMaterial = MeshVoxelizer.defaultMaterial;
            if (MeshVoxelizer.voxelMesh == null) MeshVoxelizer.voxelMesh = MeshVoxelizer.defaultVoxelMesh;
        }

        void OnGUI()
        {
            MeshVoxelizer.sourceGameObject = (GameObject)EditorGUILayout.ObjectField("Source GameObject", MeshVoxelizer.sourceGameObject, typeof(GameObject), true);
            Mesh sourceMesh = null;
            if (MeshVoxelizer.sourceGameObject != null)
            {
                if (MeshVoxelizer.sourceGameObject.GetComponent<MeshRenderer>() != null)
                {
                    sourceMesh = MeshVoxelizer.sourceGameObject.GetComponent<MeshFilter>().sharedMesh;
                }
                else if (MeshVoxelizer.sourceGameObject.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    sourceMesh = MeshVoxelizer.sourceGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                }
            }
            VoxelInt3 sizeInfo = new VoxelInt3();
            float maxAbsoluteVoxelSize = 1.0f;
            if (sourceMesh != null)
            {
                maxAbsoluteVoxelSize = MeshVoxelizer.GetMax(sourceMesh.bounds.size.x, sourceMesh.bounds.size.y, sourceMesh.bounds.size.z);
                Vector3 unit = new Vector3();
                if (MeshVoxelizer.voxelSizeType == VoxelSizeType.Subdivision)
                {
                    unit.x = maxAbsoluteVoxelSize / MeshVoxelizer.subdivisionLevel;
                    unit.y = maxAbsoluteVoxelSize / MeshVoxelizer.subdivisionLevel;
                    unit.z = maxAbsoluteVoxelSize / MeshVoxelizer.subdivisionLevel;
                }
                else
                {
                    unit.x = MeshVoxelizer.absoluteVoxelSize;
                    unit.y = MeshVoxelizer.absoluteVoxelSize;
                    unit.z = MeshVoxelizer.absoluteVoxelSize;
                }
                unit *= 1.00001f;
                sizeInfo.x = Mathf.CeilToInt(sourceMesh.bounds.size.x / unit.x);
                sizeInfo.y = Mathf.CeilToInt(sourceMesh.bounds.size.y / unit.y);
                sizeInfo.z = Mathf.CeilToInt(sourceMesh.bounds.size.z / unit.z);
            }
            if (sizeInfo.x == 0) sizeInfo.x = 1;
            if (sizeInfo.y == 0) sizeInfo.y = 1;
            if (sizeInfo.z == 0) sizeInfo.z = 1;

            MeshVoxelizer.generationType = (GenerationType)EditorGUILayout.EnumPopup("Generation Type", MeshVoxelizer.generationType);
            MeshVoxelizer.voxelSizeType = (VoxelSizeType)EditorGUILayout.EnumPopup("Voxel Size Type", MeshVoxelizer.voxelSizeType);

            string info = "Info: ";
            if (MeshVoxelizer.voxelSizeType == VoxelSizeType.Subdivision)
            {
                MeshVoxelizer.subdivisionLevel = EditorGUILayout.IntSlider("Subdivision Level", MeshVoxelizer.subdivisionLevel, 1, MeshVoxelizer.MAX_SUBDIVISION);
                info += "  W:" + sizeInfo.x + "  L:" + sizeInfo.z + "  H:" + sizeInfo.y;
                if (MeshVoxelizer.subdivisionLevel > 300)
                {
                    info += "\nExtremely high subdivision level may cause unstable result and take very long time to process";
                }
                else if (MeshVoxelizer.subdivisionLevel > 100)
                {
                    info += "\nHigh subdivision level will take longer time to process";
                }
            }
            else
            {
                MeshVoxelizer.absoluteVoxelSize = EditorGUILayout.Slider("Absolute Voxel Size", MeshVoxelizer.absoluteVoxelSize, maxAbsoluteVoxelSize / MeshVoxelizer.MAX_SUBDIVISION, maxAbsoluteVoxelSize);
                info += "W:" + sizeInfo.x + " L:" + sizeInfo.z + " H:" + sizeInfo.y;
                if (MeshVoxelizer.absoluteVoxelSize < maxAbsoluteVoxelSize / 300.0f)
                {
                    info += "\nExtremely small voxel size may cause unstable result and take very long time to process";
                }
                else if (MeshVoxelizer.absoluteVoxelSize < maxAbsoluteVoxelSize / 100.0f)
                {
                    info += "\nSmall voxel size will take longer time to process";
                }
            }
            MeshVoxelizer.precision = (Precision)EditorGUILayout.EnumPopup("Precision", MeshVoxelizer.precision);

            MeshVoxelizer.uvConversion = (UVConversion)EditorGUILayout.EnumPopup("UV Conversion Type", MeshVoxelizer.uvConversion);
            if (MeshVoxelizer.generationType == GenerationType.SingleMesh)
            {
                if (sourceMesh != null && sourceMesh.vertices.Length == sourceMesh.boneWeights.Length)
                {
                    EditorGUI.BeginDisabledGroup(MeshVoxelizer.optimization);
                    MeshVoxelizer.boneWeightConversion = EditorGUILayout.Toggle("Bone Weight Conversion", MeshVoxelizer.boneWeightConversion);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.BeginDisabledGroup(MeshVoxelizer.modifyVoxel);
                MeshVoxelizer.backfaceCulling = EditorGUILayout.Toggle("Backface Culling", MeshVoxelizer.backfaceCulling);
                MeshVoxelizer.optimization = EditorGUILayout.Toggle("Optimization", MeshVoxelizer.optimization);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                MeshVoxelizer.fillCenter = (FillCenterMethod)EditorGUILayout.EnumPopup("Fill Center Space", MeshVoxelizer.fillCenter);
                EditorGUI.BeginDisabledGroup(MeshVoxelizer.fillCenter == FillCenterMethod.None);
                MeshVoxelizer.centerMaterial = (Material)EditorGUILayout.ObjectField("Center Material", MeshVoxelizer.centerMaterial, typeof(Material), true);
                EditorGUI.EndDisabledGroup();
            }
            MeshVoxelizer.approximation = EditorGUILayout.Toggle("Approximation", MeshVoxelizer.approximation);

            MeshVoxelizer.modifyVoxel = EditorGUILayout.BeginToggleGroup("Modify Voxel", MeshVoxelizer.modifyVoxel);
            MeshVoxelizer.voxelMesh = (Mesh)EditorGUILayout.ObjectField("Voxel Mesh", MeshVoxelizer.voxelMesh, typeof(Mesh), true);
            MeshVoxelizer.voxelScale = EditorGUILayout.Slider("Voxel Scale", MeshVoxelizer.voxelScale, 0.01f, 1.0f);
            EditorGUILayout.EndToggleGroup();

            if (sourceMesh == null) info = "Please select a valid source GameObject";
            EditorGUILayout.HelpBox(info, MessageType.None);
            EditorGUI.BeginDisabledGroup(sourceMesh == null);
            if (GUILayout.Button("Voxelize Mesh"))
            {
                MeshVoxelizer.VoxelizeMesh();
                EditorGUIUtility.ExitGUI();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    [CustomEditor(typeof(VoxelGroup))]
    public class VoxelGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            VoxelGroup voxelGroup = (VoxelGroup)target;

            if (GUILayout.Button("Rebuild Voxels"))
            {
                voxelGroup.RebuildVoxels();
            }

            if (GUILayout.Button("Reset Voxels"))
            {
                voxelGroup.ResetVoxels();
            }
        }
    }
}
#endif