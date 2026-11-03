using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class RayTracingSystem : MonoBehaviour {
    
    struct Vertex {
        public Vector3 position;
        public Vector3 normal;
    }

    struct Triangle {
        public int i0, i1, i2;
    }

    [SerializeField] private SceneMeshExtractor extractorInstance;

    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;

    private uint _currentSample = 0;
    private Material _addMaterial;
    private RenderTexture _target;
    private Camera _camera;

    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _triangleBuffer;

    private void OnEnable(){
        if (SceneMeshExtractor.Instance != null){
            extractorInstance = SceneMeshExtractor.Instance;
        }
        SetUpScene();
    }

    private void OnDisable(){
        if (_vertexBuffer != null) _vertexBuffer.Release();
        if (_triangleBuffer != null) _triangleBuffer.Release();
    }

    private void Awake(){
        _camera = GetComponent<Camera>();
    }

    private void Update(){
        if(transform.hasChanged){
            _currentSample = 0;
            transform.hasChanged = false;
        }
    }

    // Serialize scene and setting up vertrices and triangles ComputerBuffer
    private void SetUpScene(){
        if (_vertexBuffer != null) _vertexBuffer.Release();
        if (_triangleBuffer != null) _triangleBuffer.Release();

        List<Vertex> verticesList = new List<Vertex>();
        List<Triangle> trianglesList = new List<Triangle>();
        List<SceneMeshExtractor.ExtractedMeshData> extractedData = extractorInstance.ExtractAllMeshesInActiveScene(); 

        int vertexOffset = 0;

        foreach (SceneMeshExtractor.ExtractedMeshData data in extractedData){
            for (int i = 0; i < data.worldVertices.Length; i++){
                verticesList.Add(new Vertex{
                    position = data.worldVertices[i],
                    normal = data.worldNormals[i]
                });
            }
            for (int j = 0; j < data.triangles.Length; j += 3){
                trianglesList.Add(new Triangle{
                    i0 = data.triangles[j] + vertexOffset,
                    i1 = data.triangles[j + 1] + vertexOffset,
                    i2 = data.triangles[j + 2] + vertexOffset
                });
            }
            vertexOffset += data.worldVertices.Length;
        }

        if (verticesList.Count == 0 || trianglesList.Count == 0){
            Debug.LogWarning("[SetUpScene] The scene is empty");
            return;
        }

        int vertexStride = Marshal.SizeOf<Vertex>();
        int triangleStride = Marshal.SizeOf<Triangle>();

        _vertexBuffer = new ComputeBuffer(verticesList.Count, vertexStride);
        _vertexBuffer.SetData(verticesList);

        _triangleBuffer = new ComputeBuffer(trianglesList.Count, triangleStride);
        _triangleBuffer.SetData(trianglesList);
    }

    private void SetShaderParameters(){
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracingShader.SetInt("_TriangleCount", _triangleBuffer.count);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetBuffer(0, "_Vertices", _vertexBuffer);
        RayTracingShader.SetBuffer(0, "_Triangles", _triangleBuffer);
    }

    // Automatically called by Unity whenever the camera has finished rendering.
    private void OnRenderImage(RenderTexture source, RenderTexture destination){
        SetShaderParameters();
        Render(destination);
    }

    private void Render(RenderTexture destination){
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        if (_addMaterial == null) _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        _addMaterial.SetFloat("_Sample", _currentSample);
        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample++;
    }

    private void InitRenderTexture(){
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height){
            // Release render texture if we already have one
            if (_target != null)
            _target.Release();

            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
