using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneMeshExtractor : MonoBehaviour
{
    [System.Serializable]
    public struct ExtractedMeshData{
        public string objectName;
        public Vector3[] worldVertices;
        public Vector3[] worldNormals;
        public int[] triangles;
    }

    public static SceneMeshExtractor Instance { get; private set; } 

    private void Awake(){
        if (Instance != null && Instance != this){
            Debug.LogWarning("[Extractror] Can only have one instance of SceneMeshExtractor class.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public List<ExtractedMeshData> ExtractAllMeshesInActiveScene(){
        List<ExtractedMeshData> extractedMeshes = new List<ExtractedMeshData>();

        MeshFilter[] meshFilters = Object.FindObjectsByType<MeshFilter>(
            FindObjectsInactive.Exclude, 
            FindObjectsSortMode.None
        );

        foreach (MeshFilter filter in meshFilters){
            if (filter == null) continue;

            Mesh mesh = filter.sharedMesh;
            if (mesh == null) continue;

            if (!mesh.isReadable){
                Debug.LogWarning($"[Extractor] Mesh '{filter.gameObject.name}' has not 'Read/Write Enabled' enabled.");
                continue;
            }

            Vector3[] localVertices = mesh.vertices;
            Vector3[] localNormals = mesh.normals;
            bool hasNormals = localNormals != null && localNormals.Length == localVertices.Length;

            Vector3[] worldVertices = new Vector3[localVertices.Length];
            Vector3[] worldNormals = new Vector3[localVertices.Length];
            Transform objTransform = filter.transform;

            for (int i = 0; i < localVertices.Length; i++){
                worldVertices[i] = objTransform.TransformPoint(localVertices[i]);
                if (hasNormals)
                    worldNormals[i] = objTransform.TransformDirection(localNormals[i]);
            }

            int[] triangles = mesh.triangles;

            extractedMeshes.Add(new ExtractedMeshData
            {
                objectName = filter.gameObject.name,
                worldVertices = worldVertices,
                worldNormals = worldNormals,
                triangles = triangles
            });
        }

        return extractedMeshes;
    }
}
