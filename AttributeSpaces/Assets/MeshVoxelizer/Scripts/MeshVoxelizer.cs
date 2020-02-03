#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace MeshVoxelizerUtil
{
    public enum Precision
    {
        Low, Standard, High, VeryHigh
    }

    public enum VoxelSizeType
    {
        Subdivision, AbsoluteSize
    }

    public enum GenerationType
    {
        SingleMesh, SeparateVoxels
    }

    public enum FillCenterMethod
    {
        None, ScanlineXAxis, ScanlineYAxis, ScanlineZAxis
    }

    public struct VoxelInt2
    {
        public int x, y;

        public VoxelInt2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public string ToKey()
        {
            return GetKey(x, y);
        }

        public static string GetKey(int x, int y)
        {
            return x.ToString() + ":" + y.ToString();
        }
    }

    public struct VoxelInt3
    {
        public int x, y, z;

        public VoxelInt3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public string ToKey()
        {
            return GetKey(x, y, z);
        }

        public static string GetKey(int x, int y, int z)
        {
            return x.ToString() + ":" + y.ToString() + ":" + z.ToString();
        }
    }

    public class VoxelData
    {
        public VoxelInt3 position;
        public Vector3 center;
        public Vector3 vPoint;
        public int index;
        public int subMesh;
        public Vector3 ratio;
        public int verticeCount = 0;
        public int sampleCount = 0;
        public VoxelData v_forward = null;
        public VoxelData v_up = null;
        public VoxelData v_back = null;
        public VoxelData v_down = null;
        public VoxelData v_left = null;
        public VoxelData v_right = null;
        public bool forward = false;
        public bool up = false;
        public bool back = false;
        public bool down = false;
        public bool left = false;
        public bool right = false;

        public void UpdateNormal(Vector3 normal)
        {
            back =    back || normal.z <= 0.0f;
            forward = forward || normal.z >= 0.0f;
            left =    left || normal.x <= 0.0f;
            right =   right || normal.x >= 0.0f;
            up =      up || normal.y >= 0.0f;
            down =    down || normal.y <= 0.0f;
        }
    }

    public class SlicePointData
    {
        public VoxelInt2 position;
        public int subMesh;
        public VoxelInt2 size = new VoxelInt2(1, 1);
        public Vector3[] vertices = new Vector3[4];
        public List<Vector2> uvCoord = new List<Vector2>();

        public void CombineV(SlicePointData data)
        {
            vertices[1] = data.vertices[1];
            vertices[2] = data.vertices[2];
            uvCoord.Add(data.uvCoord[0]);
            size.y++;
        }

        public void CombineH(SlicePointData data)
        {
            vertices[2] = data.vertices[2];
            vertices[3] = data.vertices[3];
            uvCoord.AddRange(data.uvCoord);
            size.x++;
        }
    }

    public class VoxelizationData
    {
        public Mesh sourceMesh = null;
        public Material[] sourceMaterials = null;
        public bool isMeshRenderer = true;
        public Vector3 unitSize = Vector3.one;
        public Vector3 unitVoxelRatio = Vector3.one;
        public VoxelInt3 totalUnit = new VoxelInt3();
        public Vector3 origin = Vector3.zero;
        public float stepSize = 1.0f;
    }

    public class OptimizationData
    {
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceBack = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceForward = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceLeft = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceRight = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceUp = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public Dictionary<int, Dictionary<string, SlicePointData>> sliceDown = new Dictionary<int, Dictionary<string, SlicePointData>>();
        public int textureSize = 0;
        public Dictionary<string, Vector2> textureData = new Dictionary<string, Vector2>();
    }

    public class ResultData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<List<int>> triangles = new List<List<int>>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uv = new List<Vector2>();
        public List<Vector2> uv2 = new List<Vector2>();
        public List<Vector2> uv3 = new List<Vector2>();
        public List<Vector2> uv4 = new List<Vector2>();
        public List<BoneWeight> boneWeights = new List<BoneWeight>();
        public Mesh voxelizedMesh = null;
        public Material[] voxelizedMaterials = null;
    }

    [ExecuteInEditMode]
    public class MeshVoxelizer
    {
        public const int MAX_SUBDIVISION = 500;
        public const float SAMPLE_THRESHOLD = 0.05f;
        public const float APPROXIMATION_THRESHOLD = 0.90f;

        //common
        public static GameObject sourceGameObject = null;
        public static GenerationType generationType = GenerationType.SingleMesh;
        public static VoxelSizeType voxelSizeType = VoxelSizeType.Subdivision;
        public static int subdivisionLevel = 1; 
        public static float absoluteVoxelSize = 10000;
        public static Precision precision = Precision.Standard;
        public static UVConversion uvConversion = UVConversion.SourceMesh;

        public static bool modifyVoxel = false;
        public static Mesh voxelMesh = null;
        public static float voxelScale = 1.0f;

        //single mesh
        public static bool boneWeightConversion = true;
        public static bool backfaceCulling = false;
        public static bool optimization = false;
        public static bool approximation = false;

        //separate voxels
        public static FillCenterMethod fillCenter = FillCenterMethod.None;
        public static Material centerMaterial = null;

        //default objects
        static Mesh m_defaultVoxelMesh = null;
        static Material m_defaultMaterial = null;
        public static Mesh defaultVoxelMesh
        {
            get
            {
                if (m_defaultVoxelMesh == null)
                {
                    m_defaultVoxelMesh = (Mesh)Resources.Load("DefaultVoxelCube", typeof(Mesh));
                }
                return m_defaultVoxelMesh;
            }
        }
        public static Material defaultMaterial
        {
            get
            {
                if (m_defaultMaterial == null)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    m_defaultMaterial = go.GetComponent<MeshRenderer>().sharedMaterial;
                    GameObject.DestroyImmediate(go);
                }
                return m_defaultMaterial;
            }
        }
        
        //=================================================================================
        //main functions
        //=================================================================================

        public static void VoxelizeMesh()
        {
            if (sourceGameObject == null) return;

            VoxelizationData vData = new VoxelizationData();
            if (!Initialization(vData)) { EditorUtility.ClearProgressBar(); return; }

            Dictionary<string, VoxelData> voxelDict = new Dictionary<string, VoxelData>();
            if (!AnalyzeMesh(vData, voxelDict)) { EditorUtility.ClearProgressBar(); return; }
            if (!ProcessVoxelData(vData, voxelDict)) { EditorUtility.ClearProgressBar(); return; }

            //separate voxels
            if (generationType == GenerationType.SeparateVoxels)
            {
                List<Vector3> centerVoxels = new List<Vector3>();
                if (!FillCenterSpace(vData, voxelDict, centerVoxels)) { EditorUtility.ClearProgressBar(); return; }
                if (!GenerateVoxels(vData, voxelDict, centerVoxels)) { EditorUtility.ClearProgressBar(); return; }
            }

            //single mesh
            else
            {
                if (!CullFaces(voxelDict)) { EditorUtility.ClearProgressBar(); return; }
                ResultData rData = new ResultData();
                for (int i = 0; i < vData.sourceMesh.subMeshCount; ++i) rData.triangles.Add(new List<int>());
                if (optimization && !modifyVoxel)
                {
                    OptimizationData oData = new OptimizationData();
                    if (!DoOptimization(vData, voxelDict, oData)) { EditorUtility.ClearProgressBar(); return; }
                    if (!GenerateMeshVertices(oData, rData)) { EditorUtility.ClearProgressBar(); return; }
                    if (!GenerateMeshUVs(oData, rData)) { EditorUtility.ClearProgressBar(); return; }
                    if (!GenerateMeshMaterials(vData, oData, rData)) { EditorUtility.ClearProgressBar(); return; }
                }
                else
                {
                    if (!GenerateMeshVertices(vData, voxelDict, rData)) { EditorUtility.ClearProgressBar(); return; }
                    if (!GenerateMeshUVs(vData, voxelDict, rData)) { EditorUtility.ClearProgressBar(); return; }
                    rData.voxelizedMaterials = vData.sourceMaterials;
                    if (!GenerateMeshBoneWeights(vData, voxelDict, rData)) { EditorUtility.ClearProgressBar(); return; }
                }
                GenerateResult(vData, rData);
            }

            EditorUtility.ClearProgressBar();
        }

        static bool Initialization(VoxelizationData vData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Initializing... ", 0)) { return false; }
#endif
            if (sourceGameObject.GetComponent<MeshRenderer>() != null)
            {
                vData.sourceMesh = sourceGameObject.GetComponent<MeshFilter>().sharedMesh;
                vData.sourceMaterials = sourceGameObject.GetComponent<MeshRenderer>().sharedMaterials;
                vData.isMeshRenderer = true;
            }
            else if (sourceGameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                vData.sourceMesh = sourceGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                vData.sourceMaterials = sourceGameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
                vData.isMeshRenderer = false;
            }
            else return false;

            float max = GetMax(vData.sourceMesh.bounds.size.x, vData.sourceMesh.bounds.size.y, vData.sourceMesh.bounds.size.z);
            if (voxelSizeType == VoxelSizeType.Subdivision)
            {
                vData.unitSize.x = max / subdivisionLevel;
                vData.unitSize.y = max / subdivisionLevel;
                vData.unitSize.z = max / subdivisionLevel;
            }
            else
            {
                vData.unitSize.x = absoluteVoxelSize;
                vData.unitSize.y = absoluteVoxelSize;
                vData.unitSize.z = absoluteVoxelSize;
            }
            vData.unitSize *= 1.00001f;

            vData.totalUnit.x = Mathf.CeilToInt(vData.sourceMesh.bounds.size.x / vData.unitSize.x);
            vData.totalUnit.y = Mathf.CeilToInt(vData.sourceMesh.bounds.size.y / vData.unitSize.y);
            vData.totalUnit.z = Mathf.CeilToInt(vData.sourceMesh.bounds.size.z / vData.unitSize.z);
            if (vData.totalUnit.x == 0) vData.totalUnit.x = 1;
            if (vData.totalUnit.y == 0) vData.totalUnit.y = 1;
            if (vData.totalUnit.z == 0) vData.totalUnit.z = 1;

            if (modifyVoxel)
            {
                if (voxelMesh == null) voxelMesh = defaultVoxelMesh;
                vData.unitVoxelRatio.x = vData.unitSize.x * voxelScale / voxelMesh.bounds.size.x;
                vData.unitVoxelRatio.y = vData.unitSize.y * voxelScale / voxelMesh.bounds.size.y;
                vData.unitVoxelRatio.z = vData.unitSize.z * voxelScale / voxelMesh.bounds.size.z;
            }
            else
            {
                voxelMesh = defaultVoxelMesh;
                vData.unitVoxelRatio.x = vData.unitSize.x / voxelMesh.bounds.size.x;
                vData.unitVoxelRatio.y = vData.unitSize.y / voxelMesh.bounds.size.y;
                vData.unitVoxelRatio.z = vData.unitSize.z / voxelMesh.bounds.size.z;
            }

            Vector3 offset = new Vector3();
            offset.x = vData.sourceMesh.bounds.size.x > vData.unitSize.x ? 0.0f : (vData.unitSize.x - vData.sourceMesh.bounds.size.x) * 0.5f;
            offset.y = vData.sourceMesh.bounds.size.y > vData.unitSize.y ? 0.0f : (vData.unitSize.y - vData.sourceMesh.bounds.size.y) * 0.5f;
            offset.z = vData.sourceMesh.bounds.size.z > vData.unitSize.z ? 0.0f : (vData.unitSize.z - vData.sourceMesh.bounds.size.z) * 0.5f;
            vData.origin = vData.sourceMesh.bounds.min - offset;

            switch (precision)
            {
                case Precision.Low:
                    vData.stepSize = vData.unitSize.x * 0.25f;
                    break;
                case Precision.High:
                    vData.stepSize = vData.unitSize.x * 0.0625f;
                    break;
                case Precision.VeryHigh:
                    vData.stepSize = vData.unitSize.x * 0.03125f;
                    break;
                default:
                    vData.stepSize = vData.unitSize.x * 0.125f;
                    break;
            }

            return true;
        }

        static bool AnalyzeMesh(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Analyzing mesh... ", 0)) { return false; }
            int counter = 0;
            int total = vData.sourceMesh.triangles.Length / 3;
            int rem = Mathf.CeilToInt(total * 0.05f);
#endif
            for (int subMesh = 0; subMesh < vData.sourceMesh.subMeshCount; ++subMesh)
            {
                int start = (int)vData.sourceMesh.GetIndexStart(subMesh);
                int end = start + (int)vData.sourceMesh.GetIndexCount(subMesh);
                for (int i = start; i < end; i += 3)
                {
#if !DISABLE_PROGRESSBAR
                    counter++;
                    if (counter % rem == 0)
                        if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Analyzing mesh... ", (float)counter / total)) { voxelDict = null; return false; }
#endif
                    Vector3 p0 = vData.sourceMesh.vertices[vData.sourceMesh.triangles[i]];
                    Vector3 p1 = vData.sourceMesh.vertices[vData.sourceMesh.triangles[i + 1]];
                    Vector3 p2 = vData.sourceMesh.vertices[vData.sourceMesh.triangles[i + 2]];

                    //Vector3 normal = (vData.sourceMesh.normals[vData.sourceMesh.triangles[i]]
                    //                 + vData.sourceMesh.normals[vData.sourceMesh.triangles[i + 1]]
                    //                 + vData.sourceMesh.normals[vData.sourceMesh.triangles[i + 2]]) / 3.0f;

                    Vector3 v01 = p1 - p0;
                    Vector3 v12 = p2 - p1;
                    Vector3 v20 = p0 - p2;
                    Vector3 dir01 = v01.normalized;
                    Vector3 dir12 = v12.normalized;
                    Vector3 dir20 = v20.normalized;
                    Vector3 normal = Vector3.Cross(v01, -v20).normalized;
                    normal.x = Mathf.Abs(normal.x) < 0.00001f ? 0.0f : normal.x;
                    normal.y = Mathf.Abs(normal.y) < 0.00001f ? 0.0f : normal.y;
                    normal.z = Mathf.Abs(normal.z) < 0.00001f ? 0.0f : normal.z;

                    float len01 = v01.magnitude;
                    float len02 = v20.magnitude;
                    float len12 = v12.magnitude;
                    float pos01 = 0.0f;
                    while (pos01 < len01)
                    {
                        float pos02 = 0.0f;
                        float r = 1.0f - pos01 / len01;
                        while (pos02 < len02 * r)
                        {
                            Vector3 p = p0 + dir01 * pos01 - dir20 * pos02;
                            Vector3 ratio = new Vector3(Vector3.Cross((p - p1), v12).magnitude, Vector3.Cross((p - p2), v20).magnitude, Vector3.Cross((p - p0), v01).magnitude);
                            CheckPoint(voxelDict, p, ratio, i, subMesh, normal, vData.unitSize, vData.origin);
                            pos02 += vData.stepSize;
                        }
                        pos01 += vData.stepSize;
                    }

                    pos01 = 0.0f;
                    while (pos01 < len12)
                    {
                        Vector3 p = p1 + dir12 * pos01;
                        Vector3 ratio = new Vector3(0.0f, Vector3.Cross((p - p2), v20).magnitude, Vector3.Cross((p - p0), v01).magnitude);
                        CheckPoint(voxelDict, p, ratio, i, subMesh, normal, vData.unitSize, vData.origin);
                        pos01 += vData.stepSize;
                    }
                    CheckPoint(voxelDict, p2, new Vector3(0.0f, 0.0f, 1.0f), i, subMesh, normal, vData.unitSize, vData.origin);
                }
            }
            return true;
        }

        static void CheckPoint(Dictionary<string, VoxelData> voxelDict, Vector3 p, Vector3 ratio, int index, int subMesh, Vector3 normal, Vector3 unitSize, Vector3 origin)
        {
            p = p - origin;
            VoxelInt3 pos = new VoxelInt3();
            pos.x = Mathf.FloorToInt(p.x / unitSize.x);
            pos.y = Mathf.FloorToInt(p.y / unitSize.y);
            pos.z = Mathf.FloorToInt(p.z / unitSize.z);
            string key = pos.ToKey();
            if (voxelDict.ContainsKey(key))
            {
                Vector3 v = p - voxelDict[key].center;
                if (v.sqrMagnitude < voxelDict[key].vPoint.sqrMagnitude)
                {
                    voxelDict[key].vPoint = v;
                    voxelDict[key].ratio = ratio;
                    voxelDict[key].index = index;
                    voxelDict[key].subMesh = subMesh;
                }
                voxelDict[key].sampleCount++;
                voxelDict[key].UpdateNormal(normal);
            }
            else
            {
                VoxelData voxel = new VoxelData();
                voxel.position = pos;
                voxel.center.x = unitSize.x * (pos.x + 0.5f);
                voxel.center.y = unitSize.y * (pos.y + 0.5f);
                voxel.center.z = unitSize.z * (pos.z + 0.5f);
                voxel.vPoint = p - voxel.center;
                voxel.ratio = ratio;
                voxel.index = index;
                voxel.subMesh = subMesh;
                voxel.sampleCount = 1;
                voxel.UpdateNormal(normal);
                voxelDict.Add(key, voxel);
            }
        }

        static bool ProcessVoxelData(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Processing voxels... ", 0)) { return false; }
#endif
            float avg = voxelDict.First().Value.sampleCount;
            foreach (var v in voxelDict)
            {
                v.Value.center += vData.origin;
                avg = (avg + v.Value.sampleCount) * 0.5f;
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x, v.Value.position.y, v.Value.position.z - 1), out v.Value.v_back);
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x, v.Value.position.y, v.Value.position.z + 1), out v.Value.v_forward);
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x - 1, v.Value.position.y, v.Value.position.z), out v.Value.v_left);
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x + 1, v.Value.position.y, v.Value.position.z), out v.Value.v_right);
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x, v.Value.position.y + 1, v.Value.position.z), out v.Value.v_up);
                voxelDict.TryGetValue(VoxelInt3.GetKey(v.Value.position.x, v.Value.position.y - 1, v.Value.position.z), out v.Value.v_down);
            }

            List<string> approxList = new List<string>();
            List<string> discardList = new List<string>();
            float minSample = avg * SAMPLE_THRESHOLD;
            float bound = vData.unitSize.x * 0.5f * APPROXIMATION_THRESHOLD;
            foreach (var v in voxelDict)
            {
                if (v.Value.sampleCount < minSample)
                {
                    discardList.Add(v.Key);
                    continue;
                }

                if (approximation)
                {
                    bool replace = false;
                    bool remove = true;
                    if (v.Value.vPoint.z > bound && v.Value.back)
                    {
                        replace = true;
                        remove = remove && v.Value.v_forward != null;
                    }
                    if (v.Value.vPoint.z < -bound && v.Value.forward)
                    {
                        replace = true;
                        remove = remove && v.Value.v_back != null;
                    }
                    if (v.Value.vPoint.x > bound && v.Value.left)
                    {
                        replace = true;
                        remove = remove && v.Value.v_right != null;
                    }
                    if (v.Value.vPoint.x < -bound && v.Value.right)
                    {
                        replace = true;
                        remove = remove && v.Value.v_left != null;
                    }
                    if (v.Value.vPoint.y < -bound && v.Value.up)
                    {
                        replace = true;
                        remove = remove && v.Value.v_down != null;
                    }
                    if (v.Value.vPoint.y > bound && v.Value.down)
                    {
                        replace = true;
                        remove = remove && v.Value.v_up != null;
                    }
                    if (replace && remove)
                    {
                        approxList.Add(v.Key);
                    }
                }
            }
            foreach (var v in discardList)
            {
                if (voxelDict[v].v_forward != null) { voxelDict[v].v_forward.v_back = null; }
                if (voxelDict[v].v_back != null) { voxelDict[v].v_back.v_forward = null; }
                if (voxelDict[v].v_left != null) { voxelDict[v].v_left.v_right = null; }
                if (voxelDict[v].v_right != null) { voxelDict[v].v_right.v_left = null; }
                if (voxelDict[v].v_up != null) { voxelDict[v].v_up.v_down = null; }
                if (voxelDict[v].v_down != null) { voxelDict[v].v_down.v_up = null; }
                voxelDict.Remove(v);
            }
            foreach (var v in approxList)
            {
                if (voxelDict[v].v_forward != null) { voxelDict[v].v_forward.back = true; voxelDict[v].v_forward.v_back = null; }
                if (voxelDict[v].v_back != null) { voxelDict[v].v_back.forward = true; voxelDict[v].v_back.v_forward = null; }
                if (voxelDict[v].v_left != null) { voxelDict[v].v_left.right = true; voxelDict[v].v_left.v_right = null; }
                if (voxelDict[v].v_right != null) { voxelDict[v].v_right.left = true; voxelDict[v].v_right.v_left = null; }
                if (voxelDict[v].v_up != null) { voxelDict[v].v_up.down = true; voxelDict[v].v_up.v_down = null; }
                if (voxelDict[v].v_down != null) { voxelDict[v].v_down.up = true; voxelDict[v].v_down.v_up = null; }
                voxelDict.Remove(v);
            }

            discardList.Clear();
            approxList.Clear();

            return true;
        }

        static bool FillCenterSpace(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, List<Vector3> centerVoxels)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Filling center space... ", 0)) { return false; }
#endif
            switch (fillCenter)
            {
                case FillCenterMethod.ScanlineXAxis:
                    for (int y = 0; y < vData.totalUnit.y; ++y)
                    {
                        for (int z = 0; z < vData.totalUnit.z; ++z)
                        {
                            List<int> indice = new List<int>();
                            for (int x = 0; x < vData.totalUnit.x; ++x)
                            {
                                string key = VoxelInt3.GetKey(x, y, z);
                                if (voxelDict.ContainsKey(key)) indice.Add(x);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                string start = VoxelInt3.GetKey(indice[i], y, z);
                                if (voxelDict[start].right) continue;
                                string end = VoxelInt3.GetKey(indice[i + 1], y, z);
                                if (voxelDict[end].left) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = vData.unitSize.x * (j + 0.5f);
                                    pos.y = vData.unitSize.y * (y + 0.5f);
                                    pos.z = vData.unitSize.z * (z + 0.5f);
                                    pos += vData.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                case FillCenterMethod.ScanlineYAxis:
                    for (int z = 0; z < vData.totalUnit.z; ++z)
                    {
                        for (int x = 0; x < vData.totalUnit.x; ++x)
                        {
                            List<int> indice = new List<int>();
                            for (int y = 0; y < vData.totalUnit.y; ++y)
                            {
                                string key = VoxelInt3.GetKey(x, y, z);
                                if (voxelDict.ContainsKey(key)) indice.Add(y);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                string start = VoxelInt3.GetKey(x, indice[i], z);
                                if (voxelDict[start].up) continue;
                                string end = VoxelInt3.GetKey(x, indice[i + 1], z);
                                if (voxelDict[end].down) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = vData.unitSize.x * (x + 0.5f);
                                    pos.y = vData.unitSize.y * (j + 0.5f);
                                    pos.z = vData.unitSize.z * (z + 0.5f);
                                    pos += vData.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                case FillCenterMethod.ScanlineZAxis:
                    for (int x = 0; x < vData.totalUnit.x; ++x)
                    {
                        for (int y = 0; y < vData.totalUnit.y; ++y)
                        {
                            List<int> indice = new List<int>();
                            for (int z = 0; z < vData.totalUnit.z; ++z)
                            {
                                string key = VoxelInt3.GetKey(x, y, z);
                                if (voxelDict.ContainsKey(key)) indice.Add(z);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                string start = VoxelInt3.GetKey(x, y, indice[i]);
                                if (voxelDict[start].forward) continue;
                                string end = VoxelInt3.GetKey(x, y, indice[i + 1]);
                                if (voxelDict[end].back) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = vData.unitSize.x * (x + 0.5f);
                                    pos.y = vData.unitSize.y * (y + 0.5f);
                                    pos.z = vData.unitSize.z * (j + 0.5f);
                                    pos += vData.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        static bool GenerateVoxels(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, List<Vector3> centerVoxels)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating voxels... ", 0)) { return false; }
            int counter = 0;
            int total = voxelDict.Count;
            int rem = Mathf.CeilToInt(total * 0.05f);
#endif
            VoxelGroup voxelGroup = new GameObject(sourceGameObject.name + " Voxels").AddComponent<VoxelGroup>();
            voxelGroup.voxelMesh = voxelMesh;
            voxelGroup.ratio = modifyVoxel ? vData.unitVoxelRatio.x / voxelScale : vData.unitVoxelRatio.x;
            voxelGroup.voxelScale = modifyVoxel ? voxelScale : 1.0f;
            voxelGroup.uvType = uvConversion;
            voxelGroup.voxelMaterials = vData.sourceMaterials;
            voxelGroup.voxels = new Voxel[voxelDict.Count];
            voxelGroup.voxelPosition = new Vector3[voxelDict.Count];
            voxelGroup.submesh = new int[voxelDict.Count];
            if (uvConversion == UVConversion.SourceMesh)
            {
                voxelGroup.uvs = new Vector2[voxelDict.Count];
            }
            if (fillCenter != FillCenterMethod.None)
            {
                voxelGroup.centerMaterial = centerMaterial == null ? defaultMaterial : centerMaterial;
                voxelGroup.centerVoxelPosition = centerVoxels.ToArray();
                voxelGroup.centerVoxels = new GameObject[centerVoxels.Count];
            }
            Selection.activeGameObject = voxelGroup.gameObject;

            int temp = 0;
            foreach (VoxelData voxel in voxelDict.Values)
            {
#if !DISABLE_PROGRESSBAR
                counter++;
                if (counter % rem == 0)
                    if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating voxels... ", (float)counter / total)) { return false; }
#endif
                voxelGroup.voxelPosition[temp] = voxel.center;
                voxelGroup.submesh[temp] = voxel.subMesh;
                if (uvConversion == UVConversion.SourceMesh) voxelGroup.uvs[temp] = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv, voxel);
                ++temp;
            }

            voxelGroup.RebuildVoxels();
            voxelGroup.transform.parent = sourceGameObject.transform.parent;
            voxelGroup.transform.SetSiblingIndex(sourceGameObject.transform.GetSiblingIndex() + 1);
            voxelGroup.transform.localPosition = sourceGameObject.transform.localPosition;
            voxelGroup.transform.localScale = sourceGameObject.transform.localScale;
            voxelGroup.transform.localRotation = sourceGameObject.transform.localRotation;

            return true;
        }

        static bool CullFaces(Dictionary<string, VoxelData> voxelDict)
        {
            if (modifyVoxel) return true;
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Culling faces... ", 0)) { return false; }
#endif
            if (backfaceCulling)
            {
                foreach (VoxelData voxel in voxelDict.Values)
                {
                    voxel.forward = voxel.v_forward == null && voxel.forward;
                    voxel.back = voxel.v_back == null && voxel.back;
                    voxel.left = voxel.v_left == null && voxel.left;
                    voxel.right = voxel.v_right == null && voxel.right;
                    voxel.up = voxel.v_up == null && voxel.up;
                    voxel.down = voxel.v_down == null && voxel.down;
                }
            }
            else
            {
                foreach (VoxelData voxel in voxelDict.Values)
                {
                    voxel.forward = voxel.v_forward == null;
                    voxel.back = voxel.v_back == null;
                    voxel.left = voxel.v_left == null;
                    voxel.right = voxel.v_right == null;
                    voxel.up = voxel.v_up == null;
                    voxel.down = voxel.v_down == null;
                }
            }
            return true;
        }

        static bool GenerateMeshVertices(OptimizationData oData, ResultData rData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating mesh... ", 0)) { return false; }
#endif
            AddSliceVertices(rData, oData.sliceBack, Vector3.back);
            AddSliceVertices(rData, oData.sliceForward, Vector3.forward);
            AddSliceVertices(rData, oData.sliceLeft, Vector3.left);
            AddSliceVertices(rData, oData.sliceRight, Vector3.right);
            AddSliceVertices(rData, oData.sliceUp, Vector3.up);
            AddSliceVertices(rData, oData.sliceDown, Vector3.down);
            return true;
        }

        static bool GenerateMeshVertices(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, ResultData rData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating mesh... ", 0)) { return false; }
#endif
            if (modifyVoxel)
            {
                foreach (VoxelData voxel in voxelDict.Values)
                {
                    int index = rData.vertices.Count;
                    foreach (Vector3 v in voxelMesh.vertices) rData.vertices.Add(ConvertVertex(v, voxel.center, vData.unitVoxelRatio));
                    foreach (int i in voxelMesh.triangles) rData.triangles[voxel.subMesh].Add(i + index);
                    rData.normals.AddRange(voxelMesh.normals);
                    voxel.verticeCount = voxelMesh.vertices.Length;
                }
            }
            else
            {
                foreach (VoxelData voxel in voxelDict.Values)
                {
                    int index = rData.vertices.Count;
                    if (voxel.back)     AddOneFaceVertices(rData, voxel, 0, index, vData.unitVoxelRatio);
                    if (voxel.forward)  AddOneFaceVertices(rData, voxel, 1, index, vData.unitVoxelRatio);
                    if (voxel.left)     AddOneFaceVertices(rData, voxel, 2, index, vData.unitVoxelRatio);
                    if (voxel.right)    AddOneFaceVertices(rData, voxel, 3, index, vData.unitVoxelRatio);
                    if (voxel.up)       AddOneFaceVertices(rData, voxel, 4, index, vData.unitVoxelRatio);
                    if (voxel.down)     AddOneFaceVertices(rData, voxel, 5, index, vData.unitVoxelRatio);
                }
            }
            return true;
        }

        static bool DoOptimization(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, OptimizationData oData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Optimizing result... ", 0)) { return false; }
            int counter = 0;
            int total = voxelDict.Count;
            int rem = Mathf.CeilToInt(total * 0.05f);
#endif
            for (int x = 0; x < vData.totalUnit.x; ++x)
            {
                for (int y = 0; y < vData.totalUnit.y; ++y)
                {
                    for (int z = 0; z < vData.totalUnit.z; ++z)
                    {
                        string key = VoxelInt3.GetKey(x, y, z);
                        if (voxelDict.ContainsKey(key))
                        {
#if !DISABLE_PROGRESSBAR
                            counter++;
                            if (counter % rem == 0)
                                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Optimizing result... ", (float)counter / total)) { return false; }
#endif
                            int xInv = vData.totalUnit.x - x - 1;
                            int yInv = vData.totalUnit.y - y - 1;
                            int zInv = vData.totalUnit.z - z - 1;
                            VoxelData voxel = voxelDict[key];
                            Vector2 uv = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv, voxel);
                            if (voxel.back)
                            {
                                if (!oData.sliceBack.ContainsKey(z)) oData.sliceBack.Add(z, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(x, y, 0, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceBack[z].Add(data.position.ToKey(), data);
                            }
                            if (voxel.forward)
                            {
                                if (!oData.sliceForward.ContainsKey(zInv)) oData.sliceForward.Add(zInv, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(xInv, y, 4, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceForward[zInv].Add(data.position.ToKey(), data);
                            }
                            if (voxel.left)
                            {
                                if (!oData.sliceLeft.ContainsKey(x)) oData.sliceLeft.Add(x, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(zInv, y, 8, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceLeft[x].Add(data.position.ToKey(), data);
                            }
                            if (voxel.right)
                            {
                                if (!oData.sliceRight.ContainsKey(xInv)) oData.sliceRight.Add(xInv, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(z, y, 12, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceRight[xInv].Add(data.position.ToKey(), data);
                            }
                            if (voxel.up)
                            {
                                if (!oData.sliceUp.ContainsKey(yInv)) oData.sliceUp.Add(yInv, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(x, z, 16, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceUp[yInv].Add(data.position.ToKey(), data);
                            }
                            if (voxel.down)
                            {
                                if (!oData.sliceDown.ContainsKey(y)) oData.sliceDown.Add(y, new Dictionary<string, SlicePointData>());
                                SlicePointData data = GetSlicePointData(xInv, z, 20, voxel, uv, vData.unitVoxelRatio);
                                oData.sliceDown[y].Add(data.position.ToKey(), data);
                            }
                        }
                    }
                }
            }

#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 0)) { return false; }
#endif
            OptimizeSlice(oData.sliceBack, vData.totalUnit.x, vData.totalUnit.y);
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 1.0f / 6.0f)) { return false; }
#endif
            OptimizeSlice(oData.sliceForward, vData.totalUnit.x, vData.totalUnit.y);
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 2.0f / 6.0f)) { return false; }
#endif
            OptimizeSlice(oData.sliceLeft, vData.totalUnit.z, vData.totalUnit.y);
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 3.0f / 6.0f)) { return false; }
#endif
            OptimizeSlice(oData.sliceRight, vData.totalUnit.z, vData.totalUnit.y);
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 4.0f / 6.0f)) { return false; }
#endif
            OptimizeSlice(oData.sliceUp, vData.totalUnit.x, vData.totalUnit.z);
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Rebuilding voxels... ", 5.0f / 6.0f)) { return false; }
#endif
            OptimizeSlice(oData.sliceDown, vData.totalUnit.x, vData.totalUnit.z);
            return true;
        }

        static bool GenerateMeshUVs(OptimizationData oData, ResultData rData)
        {
            if (uvConversion == UVConversion.SourceMesh)
            {
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 0.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceBack);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 1.0f / 6.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceForward);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 2.0f / 6.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceLeft);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 3.0f / 6.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceRight);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 4.0f / 6.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceUp);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Computing UVs... ", 5.0f / 6.0f)) { return false; }
#endif
                ComputeSlice(oData, oData.sliceDown);

#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 0.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceBack, rData);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 1.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceForward, rData);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 2.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceLeft, rData);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 3.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceRight, rData);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 4.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceUp, rData);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 5.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(oData, oData.sliceDown, rData);
            }
            else if (uvConversion == UVConversion.VoxelMesh)
            {
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 0.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceBack);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 1.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceForward);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 2.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceLeft);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 3.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceRight);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 4.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceUp);
#if !DISABLE_PROGRESSBAR
                if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 5.0f / 6.0f)) { return false; }
#endif
                AddOneSliceUV(rData, oData.sliceDown);
            }
            return true;
        }

        static bool GenerateMeshUVs(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, ResultData rData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating UVs... ", 0)) { return false; }
#endif
            if (uvConversion == UVConversion.SourceMesh)
            {
                if (vData.sourceMesh.uv.Length == vData.sourceMesh.vertices.Length)
                {
                    foreach (VoxelData voxel in voxelDict.Values)
                    {
                        Vector2 uv = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv, voxel);
                        for (int i = 0; i < voxel.verticeCount; ++i) rData.uv.Add(uv);
                    }
                }
                if (vData.sourceMesh.uv2.Length == vData.sourceMesh.vertices.Length)
                {
                    foreach (VoxelData voxel in voxelDict.Values)
                    {
                        Vector2 uv = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv2, voxel);
                        for (int i = 0; i < voxel.verticeCount; ++i) rData.uv2.Add(uv);
                    }
                }
                if (vData.sourceMesh.uv3.Length == vData.sourceMesh.vertices.Length)
                {
                    foreach (VoxelData voxel in voxelDict.Values)
                    {
                        Vector2 uv = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv3, voxel);
                        for (int i = 0; i < voxel.verticeCount; ++i) rData.uv3.Add(uv);
                    }
                }
                if (vData.sourceMesh.uv4.Length == vData.sourceMesh.vertices.Length)
                {
                    foreach (VoxelData voxel in voxelDict.Values)
                    {
                        Vector2 uv = GetUVCoord(vData.sourceMesh, vData.sourceMesh.uv4, voxel);
                        for (int i = 0; i < voxel.verticeCount; ++i) rData.uv4.Add(uv);
                    }
                }
            }
            else if (uvConversion == UVConversion.VoxelMesh)
            {
                if (modifyVoxel)
                {
                    foreach (VoxelData voxel in voxelDict.Values) rData.uv.AddRange(voxelMesh.uv);
                }
                else
                {
                    foreach (VoxelData voxel in voxelDict.Values)
                    {
                        if (voxel.forward) AddOneFaceUV(rData, 0);
                        if (voxel.up) AddOneFaceUV(rData, 4);
                        if (voxel.back) AddOneFaceUV(rData, 8);
                        if (voxel.down) AddOneFaceUV(rData, 12);
                        if (voxel.left) AddOneFaceUV(rData, 16);
                        if (voxel.right) AddOneFaceUV(rData, 20);
                    }
                }
            }
            return true;
        }

        static bool GenerateMeshMaterials(VoxelizationData vData, OptimizationData oData, ResultData rData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating textures...", 0)) { return false; }
#endif
            rData.voxelizedMaterials = new Material[vData.sourceMaterials.Length];
            for (int i = 0; i < vData.sourceMaterials.Length; ++i)
            {
                rData.voxelizedMaterials[i] = GameObject.Instantiate(vData.sourceMaterials[i]);
            }

            foreach (var mat in rData.voxelizedMaterials)
            {
                Shader shader = mat.shader;
                for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                {
                    if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string tName = ShaderUtil.GetPropertyName(shader, i);
                        Texture tex = mat.GetTexture(tName);
                        if (tex != null)
                        {
                            if (uvConversion == UVConversion.SourceMesh)
                                mat.SetTexture(tName, CreateTexture(oData, tex));
                            else
                                mat.SetTexture(tName, tex);
                        }
                    }
                }
            }
            return true;
        }

        static bool GenerateMeshBoneWeights(VoxelizationData vData, Dictionary<string, VoxelData> voxelDict, ResultData rData)
        {
            if (!boneWeightConversion || vData.sourceMesh.boneWeights.Length != vData.sourceMesh.vertices.Length) return true;
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating BoneWeights...", 0)) { return false; }
            int counter = 0;
            int total = voxelDict.Count;
            int rem = Mathf.CeilToInt(total * 0.05f);
#endif

            Dictionary<int, float> temp = new Dictionary<int, float>();
            KeyValuePair<int, float>[] bwArray = new KeyValuePair<int, float>[4];
            foreach (VoxelData voxel in voxelDict.Values)
            {
#if !DISABLE_PROGRESSBAR
                counter++;
                if (counter % rem == 0)
                    if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating BoneWeights...", (float)counter / total)) { return false; }
#endif
                BoneWeight bw0 = vData.sourceMesh.boneWeights[vData.sourceMesh.triangles[voxel.index]];
                BoneWeight bw1 = vData.sourceMesh.boneWeights[vData.sourceMesh.triangles[voxel.index + 1]];
                BoneWeight bw2 = vData.sourceMesh.boneWeights[vData.sourceMesh.triangles[voxel.index + 2]];
                float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;

                float px = voxel.ratio.x / sum;
                if (temp.ContainsKey(bw0.boneIndex0)) temp[bw0.boneIndex0] += bw0.weight0 * px;
                else temp.Add(bw0.boneIndex0, bw0.weight0 * px);
                if (temp.ContainsKey(bw0.boneIndex1)) temp[bw0.boneIndex1] += bw0.weight1 * px;
                else temp.Add(bw0.boneIndex1, bw0.weight1 * px);
                if (temp.ContainsKey(bw0.boneIndex2)) temp[bw0.boneIndex2] += bw0.weight2 * px;
                else temp.Add(bw0.boneIndex2, bw0.weight2 * px);
                if (temp.ContainsKey(bw0.boneIndex3)) temp[bw0.boneIndex3] += bw0.weight3 * px;
                else temp.Add(bw0.boneIndex3, bw0.weight3 * px);

                float py = voxel.ratio.y / sum;
                if (temp.ContainsKey(bw1.boneIndex0)) temp[bw1.boneIndex0] += bw1.weight0 * py;
                else temp.Add(bw1.boneIndex0, bw1.weight0 * py);
                if (temp.ContainsKey(bw1.boneIndex1)) temp[bw1.boneIndex1] += bw1.weight1 * py;
                else temp.Add(bw1.boneIndex1, bw1.weight1 * py);
                if (temp.ContainsKey(bw1.boneIndex2)) temp[bw1.boneIndex2] += bw1.weight2 * py;
                else temp.Add(bw1.boneIndex2, bw1.weight2 * py);
                if (temp.ContainsKey(bw1.boneIndex3)) temp[bw1.boneIndex3] += bw1.weight3 * py;
                else temp.Add(bw1.boneIndex3, bw1.weight3 * py);

                float pz = voxel.ratio.z / sum;
                if (temp.ContainsKey(bw2.boneIndex0)) temp[bw2.boneIndex0] += bw2.weight0 * pz;
                else temp.Add(bw2.boneIndex0, bw2.weight0 * pz);
                if (temp.ContainsKey(bw2.boneIndex1)) temp[bw2.boneIndex1] += bw2.weight1 * pz;
                else temp.Add(bw2.boneIndex1, bw2.weight1 * pz);
                if (temp.ContainsKey(bw2.boneIndex2)) temp[bw2.boneIndex2] += bw2.weight2 * pz;
                else temp.Add(bw2.boneIndex2, bw2.weight2 * pz);
                if (temp.ContainsKey(bw2.boneIndex3)) temp[bw2.boneIndex3] += bw2.weight3 * pz;
                else temp.Add(bw2.boneIndex3, bw2.weight3 * pz);

                var order = temp.OrderByDescending(x => x.Value).ToArray();
                int limit = order.Length < 4 ? order.Length : 4;
                int index;
                for (index = 0; index < limit; ++index)
                {
                    bwArray[index] = order[index];
                }
                for (index = order.Length; index < 4; ++index)
                {
                    bwArray[index] = new KeyValuePair<int, float>();
                }
                sum = bwArray[0].Value + bwArray[1].Value + bwArray[2].Value + bwArray[3].Value;

                BoneWeight bw = new BoneWeight();
                bw.boneIndex0 = bwArray[0].Key;
                bw.boneIndex1 = bwArray[1].Key;
                bw.boneIndex2 = bwArray[2].Key;
                bw.boneIndex3 = bwArray[3].Key;
                bw.weight0 = bwArray[0].Value / sum;
                bw.weight1 = bwArray[1].Value / sum;
                bw.weight2 = bwArray[2].Value / sum;
                bw.weight3 = bwArray[3].Value / sum;
                for (int i = 0; i < voxel.verticeCount; ++i) rData.boneWeights.Add(bw);
                temp.Clear();
            }
            return true;
        }

        static void GenerateResult(VoxelizationData vData, ResultData rData)
        {
#if !DISABLE_PROGRESSBAR
            if (EditorUtility.DisplayCancelableProgressBar("Mesh Voxelizer", "Generating result...", 0)) { return; }
#endif
            rData.voxelizedMesh = new Mesh();
            rData.voxelizedMesh.name = vData.sourceMesh.name + " Voxelized";
#if UNITY_2017_3_OR_NEWER
            if (rData.vertices.Count > 65535) rData.voxelizedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
#endif
            rData.voxelizedMesh.SetVertices(rData.vertices);
            rData.voxelizedMesh.SetNormals(rData.normals);
            rData.voxelizedMesh.subMeshCount = rData.triangles.Count;
            for (int i = 0; i < rData.triangles.Count; ++i)
            {
                rData.voxelizedMesh.SetTriangles(rData.triangles[i], i);
            }
            rData.voxelizedMesh.SetUVs(0, rData.uv);
            if (rData.uv2.Count != 0) rData.voxelizedMesh.SetUVs(1, rData.uv2);
            if (rData.uv3.Count != 0) rData.voxelizedMesh.SetUVs(2, rData.uv3);
            if (rData.uv4.Count != 0) rData.voxelizedMesh.SetUVs(3, rData.uv4);
            if (rData.boneWeights.Count != 0)
            {
                rData.voxelizedMesh.boneWeights = rData.boneWeights.ToArray();
                rData.voxelizedMesh.bindposes = vData.sourceMesh.bindposes;
            }

            //save result
            string folderPath = "Assets/MeshVoxelizer";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string guid = AssetDatabase.CreateFolder("Assets", "MeshVoxelizer");
                folderPath = AssetDatabase.GUIDToAssetPath(guid);
            }
            folderPath += "/Temp";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string guid = AssetDatabase.CreateFolder("Assets/MeshVoxelizer", "Temp");
                folderPath = AssetDatabase.GUIDToAssetPath(guid);
            }
            string assetPath = folderPath + "/" + sourceGameObject.name + " Voxelized.prefab";
            int counter = 1;
            while (File.Exists(assetPath))
            {
                counter++;
                assetPath = folderPath + "/" + sourceGameObject.name + " Voxelized " + counter.ToString() + ".prefab";
            }

            GameObject go = new GameObject(sourceGameObject.name + " Voxelized");
            if (counter > 1) go.name += " " + counter.ToString();
            go.transform.parent = sourceGameObject.transform.parent;
            go.transform.SetSiblingIndex(sourceGameObject.transform.GetSiblingIndex() + 1);
            go.transform.localPosition = sourceGameObject.transform.localPosition;
            go.transform.localScale = sourceGameObject.transform.localScale;
            go.transform.localRotation = sourceGameObject.transform.localRotation;
#if UNITY_2018_3_OR_NEWER
            GameObject result = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetPath, InteractionMode.UserAction);
#else
            GameObject result = PrefabUtility.CreatePrefab(assetPath, go, ReplacePrefabOptions.ConnectToPrefab);
#endif

            AssetDatabase.AddObjectToAsset(rData.voxelizedMesh, assetPath);
            if (rData.voxelizedMaterials != null && rData.voxelizedMaterials.Length != 0)
            {
                foreach (var mat in rData.voxelizedMaterials)
                {
                    Shader shader = mat.shader;
                    for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                    {
                        if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            string tName = ShaderUtil.GetPropertyName(shader, i);
                            Texture tex = mat.GetTexture(tName);
                            if (tex != null && !AssetDatabase.Contains(tex)) AssetDatabase.AddObjectToAsset(tex, assetPath);
                        }
                    }
                    if (!AssetDatabase.Contains(mat)) AssetDatabase.AddObjectToAsset(mat, assetPath);
                }
            }
            AssetDatabase.SaveAssets();

            if (vData.isMeshRenderer)
            {
                MeshFilter mFilter = result.AddComponent<MeshFilter>();
                MeshRenderer mRenderer = result.AddComponent<MeshRenderer>();
                mFilter.sharedMesh = rData.voxelizedMesh;
                mRenderer.sharedMaterials = rData.voxelizedMaterials;
            }
            else
            {
                SkinnedMeshRenderer renderer = result.AddComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = rData.voxelizedMesh;
                renderer.sharedMaterials = rData.voxelizedMaterials;
            }

#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SavePrefabAsset(result);
#endif
            Undo.RegisterCreatedObjectUndo(go, "Undo instantiating " + go.name);
            Selection.activeGameObject = go;
        }

        //=================================================================================
        //helper functions
        //=================================================================================

        static Vector3 ConvertVertex(Vector3 v, Vector3 offset, Vector3 unitVoxelRatio)
        {
            Vector3 vert = v;
            vert.x *= unitVoxelRatio.x;
            vert.y *= unitVoxelRatio.y;
            vert.z *= unitVoxelRatio.z;
            return vert + offset;
        }

        static void AddSliceVertices(ResultData rData, Dictionary<int, Dictionary<string, SlicePointData>> sliceDict, Vector3 normal)
        {
            foreach (var slice in sliceDict.Values)
            {
                foreach (var point in slice.Values)
                {
                    int index = rData.vertices.Count;
                    rData.vertices.Add(point.vertices[0]);
                    rData.vertices.Add(point.vertices[1]);
                    rData.vertices.Add(point.vertices[2]);
                    rData.vertices.Add(point.vertices[3]);
                    rData.normals.Add(normal);
                    rData.normals.Add(normal);
                    rData.normals.Add(normal);
                    rData.normals.Add(normal);
                    rData.triangles[point.subMesh].Add(0 + index);
                    rData.triangles[point.subMesh].Add(1 + index);
                    rData.triangles[point.subMesh].Add(2 + index);
                    rData.triangles[point.subMesh].Add(2 + index);
                    rData.triangles[point.subMesh].Add(3 + index);
                    rData.triangles[point.subMesh].Add(0 + index);
                }
            }
        }

        static void AddOneFaceVertices(ResultData rData, VoxelData voxel, int vIndex, int index, Vector3 unitVoxelRatio)
        {
            int v = 4 * vIndex;
            int t = 6 * vIndex;
            index = index - v + voxel.verticeCount;
            rData.vertices.Add(ConvertVertex(voxelMesh.vertices[v + 0], voxel.center, unitVoxelRatio));
            rData.vertices.Add(ConvertVertex(voxelMesh.vertices[v + 1], voxel.center, unitVoxelRatio));
            rData.vertices.Add(ConvertVertex(voxelMesh.vertices[v + 2], voxel.center, unitVoxelRatio));
            rData.vertices.Add(ConvertVertex(voxelMesh.vertices[v + 3], voxel.center, unitVoxelRatio));
            rData.normals.Add(voxelMesh.normals[v + 0]);
            rData.normals.Add(voxelMesh.normals[v + 1]);
            rData.normals.Add(voxelMesh.normals[v + 2]);
            rData.normals.Add(voxelMesh.normals[v + 3]);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 0] + index);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 1] + index);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 2] + index);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 3] + index);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 4] + index);
            rData.triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 5] + index);
            voxel.verticeCount += 4;
        }

        static void AddOneSliceUV(ResultData rData, Dictionary<int, Dictionary<string, SlicePointData>> sliceDict)
        {
            foreach (var slice in sliceDict.Values)
            {
                foreach (var point in slice.Values)
                {
                    rData.uv.Add(new Vector2(0.0f, 0.0f));
                    rData.uv.Add(new Vector2(0.0f, point.size.y));
                    rData.uv.Add(new Vector2(point.size.x, point.size.y));
                    rData.uv.Add(new Vector2(point.size.x, 0.0f));
                }
            }
        }

        static void AddOneFaceUV(ResultData rData, int v)
        {
            rData.uv.Add(voxelMesh.uv[v + 0]);
            rData.uv.Add(voxelMesh.uv[v + 1]);
            rData.uv.Add(voxelMesh.uv[v + 2]);
            rData.uv.Add(voxelMesh.uv[v + 3]);
        }

        static void AddOneSliceUV(OptimizationData oData, Dictionary<int, Dictionary<string, SlicePointData>> sliceDict, ResultData rData)
        {
            foreach (var slice in sliceDict.Values)
            {
                foreach (var point in slice.Values)
                {
                    float offset = 0.03f;
                    rData.uv.Add(new Vector2((point.position.x + offset) / oData.textureSize, (point.position.y + offset) / oData.textureSize));
                    rData.uv.Add(new Vector2((point.position.x + offset) / oData.textureSize, (point.position.y + point.size.y - offset) / oData.textureSize));
                    rData.uv.Add(new Vector2((point.position.x + point.size.x - offset) / oData.textureSize, (point.position.y + point.size.y - offset) / oData.textureSize));
                    rData.uv.Add(new Vector2((point.position.x + point.size.x - offset) / oData.textureSize, (point.position.y + offset) / oData.textureSize));
                }
            }
        }

        static void ComputeSlice(OptimizationData oData, Dictionary<int, Dictionary<string, SlicePointData>> sliceDict)
        {
            foreach (var slice in sliceDict.Values)
            {
                foreach (var point in slice.Values)
                {
                    bool found = false;
                    for (int i = 0; i < oData.textureSize; ++i)
                    {
                        for (int j = 0; j < oData.textureSize; ++j)
                        {
                            if (i + point.size.x <= oData.textureSize && j + point.size.y <= oData.textureSize)
                            {
                                bool occupied = false;
                                for (int x = 0; x < point.size.x; ++x)
                                {
                                    for (int y = 0; y < point.size.y; ++y)
                                    {
                                        occupied = oData.textureData.ContainsKey(VoxelInt2.GetKey(x + i, y + j));
                                        if (occupied)
                                        {
                                            j += y;
                                            break;
                                        }
                                    }
                                    if (occupied) break;
                                }

                                if (!occupied)
                                {
                                    point.position.x = i;
                                    point.position.y = j;
                                    for (int x = 0; x < point.size.x; ++x)
                                    {
                                        for (int y = 0; y < point.size.y; ++y)
                                        {
                                            oData.textureData.Add(VoxelInt2.GetKey(x + i, y + j), point.uvCoord[point.size.y * x + y]);
                                        }
                                    }
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found) break;
                    }
                    if (!found)
                    {
                        point.position.x = 0;
                        point.position.y = oData.textureSize;
                        for (int x = 0; x < point.size.x; ++x)
                        {
                            for (int y = 0; y < point.size.y; ++y)
                            {
                                oData.textureData.Add(VoxelInt2.GetKey(x, y + oData.textureSize), point.uvCoord[point.size.y * x + y]);
                            }
                        }
                        oData.textureSize += point.size.x > point.size.y ? point.size.x : point.size.y;
                    }
                }
            }
        }

        static SlicePointData GetSlicePointData(int x, int y, int v, VoxelData voxel, Vector2 uvCoord, Vector3 unitVoxelRatio)
        {
            SlicePointData data = new SlicePointData();
            data.position = new VoxelInt2(x, y);
            data.subMesh = voxel.subMesh;
            data.vertices[0] = ConvertVertex(voxelMesh.vertices[v + 0], voxel.center, unitVoxelRatio);
            data.vertices[1] = ConvertVertex(voxelMesh.vertices[v + 1], voxel.center, unitVoxelRatio);
            data.vertices[2] = ConvertVertex(voxelMesh.vertices[v + 2], voxel.center, unitVoxelRatio);
            data.vertices[3] = ConvertVertex(voxelMesh.vertices[v + 3], voxel.center, unitVoxelRatio);
            data.uvCoord.Add(uvCoord);
            return data;
        }

        static void OptimizeSlice(Dictionary<int, Dictionary<string, SlicePointData>> sliceDict, int lenX, int lenY)
        {
            foreach (var slice in sliceDict.Values)
            {
                for (int i = 0; i < lenX; ++i)
                {
                    for (int j = 0; j < lenY - 1; ++j)
                    {
                        string key = VoxelInt2.GetKey(i, j);
                        if (slice.ContainsKey(key))
                        {
                            string nextKey = VoxelInt2.GetKey(i, j + 1);
                            while (slice.ContainsKey(nextKey) && slice[key].subMesh == slice[nextKey].subMesh)
                            {
                                slice[key].CombineV(slice[nextKey]);
                                slice.Remove(nextKey);
                                j++;
                                nextKey = VoxelInt2.GetKey(i, j + 1);
                            }
                        }
                    }
                }

                for (int i = 0; i < lenX - 1; ++i)
                {
                    for (int j = 0; j < lenY; ++j)
                    {
                        string key = VoxelInt2.GetKey(i, j);
                        if (slice.ContainsKey(key))
                        {
                            int temp = i;
                            string nextKey = VoxelInt2.GetKey(i + 1, j);
                            while (slice.ContainsKey(nextKey) && slice[key].subMesh == slice[nextKey].subMesh && slice[key].size.y == slice[nextKey].size.y)
                            {
                                slice[key].CombineH(slice[nextKey]);
                                slice.Remove(nextKey);
                                temp++;
                                nextKey = VoxelInt2.GetKey(temp + 1, j);
                            }
                        }
                    }
                }
            }
        }

        static Texture2D GetTexture2D(Texture tex)
        {
            Texture2D texture2D = new Texture2D(tex.width, tex.height);
            RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height, 32);
            Graphics.Blit(tex, rt);
            RenderTexture curr = RenderTexture.active;
            RenderTexture.active = rt;
            texture2D.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = curr;
            RenderTexture.ReleaseTemporary(rt);
            return texture2D;
        }

        static Texture CreateTexture(OptimizationData oData, Texture texture)
        {
            Texture2D texture2D = GetTexture2D(texture);
            Texture2D t2d = new Texture2D(oData.textureSize, oData.textureSize);
            t2d.name = texture.name + "_Voxelized";
            t2d.filterMode = FilterMode.Point;
            for (int i = 0; i < oData.textureSize; ++i)
            {
                for (int j = 0; j < oData.textureSize; ++j)
                {
                    string key = VoxelInt2.GetKey(i, j);
                    if (oData.textureData.ContainsKey(key))
                    {
                        Color color = texture2D.GetPixel((int)(texture2D.width * oData.textureData[key].x), (int)(texture2D.height * oData.textureData[key].y));
                        t2d.SetPixel(i, j, color);
                    }
                    else
                    {
                        t2d.SetPixel(i, j, Color.white);
                    }
                }
            }
            t2d.Apply();
            GameObject.DestroyImmediate(texture2D);
            return t2d;
        }

        static Vector2 GetUVCoord(Mesh sourceMesh, Vector2[] uvs, VoxelData voxel)
        {
            Vector2 p0 = uvs[sourceMesh.triangles[voxel.index]];
            Vector2 p1 = uvs[sourceMesh.triangles[voxel.index + 1]];
            Vector2 p2 = uvs[sourceMesh.triangles[voxel.index + 2]];
            float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;
            Vector2 uv = p0 * (voxel.ratio.x / sum) + p1 * (voxel.ratio.y / sum) + p2 * (voxel.ratio.z / sum);
            return uv;
        }

        public static float GetMax(float a, float b, float c)
        {
            float max = a > b ? a : b;
            max = max > c ? max : c;
            return max;
        }
    }
}
#endif