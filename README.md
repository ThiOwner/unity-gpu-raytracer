# Custom GPU Ray Tracer in Unity

![Ray Tracing Render Preview](Media/render_preview.jpg)

A GPU-accelerated Ray Tracer built from scratch in **Unity** using **HLSL Compute Shaders** and C#. This project explores core graphics programming concepts, geometry-ray intersections, and progressive sampling techniques without relying on Unity's built-in pipeline lighting systems.

---

## Key Features Implemented

* **Compute Shader Pipeline:** Custom HLSL Compute Shader generating camera rays directly on the GPU.
* **Camera Ray Generation:** Inverse Projection and Camera-to-World matrix transformations for pixel-to-world ray generation.
* **Geometrical Intersections:**
  * Exact Ray-Plane intersection for infinite ground surfaces.
  * Ray-Sphere quadratic equation solving for dynamic primitives.
* **Shading & Lighting Model:**
  * **Lambertian Diffuse Lighting** with directional sun mapping.
  * **Specular Reflections** and recursive ray bouncing.
  * **Hard Shadow Rays** cast towards directional light sources with self-shadowing offset protection (*shadow acne prevention*).
  * **HDRI Skybox Sampling** using spherical coordinates ($\theta, \phi$) transformation.
* **GPU Buffers:** Scene data driven via C# `StructuredBuffer<Sphere>` passed to HLSL.
* **Interactive Free Camera:** Built with Unity's **Input Action System** for seamless navigation and mouse locking.

---

## Future Roadmap & Next Steps

This repository serves as a foundational step toward building a full **Path Tracer** and advanced graphics portfolio.

- [ ] **Triangle Geometry (Ray-Triangle):**
  - Implement **Möller–Trumbore** intersection algorithm.
- [ ] **Acceleration Structure (BVH):**
  - Build a CPU/GPU **Bounding Volume Hierarchy** (AABB) to handle complex scenes with thousands of triangles at interactive frame rates.

---

## Tech Stack & Requirements

* **Engine:** Unity 2022 / 6 (Built-in Render Pipeline)
* **Languages:** C# (Host Controller) & HLSL (Compute Shader)
* **API:** DirectX 11 / Metal / Vulkan (Compute Shader support required)
* **License:** MIT License

---

## Media & Setup

1. Clone the repository:
   ```bash
   git clone [https://github.com/ThiOwner/unity-gpu-raytracer.git](https://github.com/ThiOwner/unity-gpu-raytracer.git)