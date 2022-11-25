using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class RippleRendering : CameraRendering
{
    
    public double Time { get; set; }

    private RenderObject _surfaceInstance;
    
    private const string SurfaceInstanceName = "Rain Ribble";
    private const int NumberOfSubdivisions = 100;
    
    private const float Gravity = 9.8f;
    private const float Density = 997;
    private const float SurfaceTension = 73E-3f;
    private const float Strength = 0.01f;

    private const float DampingCoefficient = 0;
    private const float SmallScale = 0.8f;

    
    public RippleRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0.1f, 0.8f, 0.4f), camera)
    {
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        //var material = new AmbientMaterial(device, device.Color3(0f, 1, 1f), device.Color3(0.8f, 0.2f, 0));
        // var material = new UniformMaterial(device, device.Color4(1f, 1f, 0.8f, 1f));
        
        // Reflectance Material
        float shininess = 0.8f;

        #region Reflection Test

        //var reflectance = new Reflectance(diffuse, specular, shininess);
        var world = device.World;
        var lightPosition = world.Point3(2, 3, -10);
        var lightSpectrum = new Light(
            AmbientColor,
            device.Color3(0.45f, 0.65f, 0.80f),
            device.Color3(0.90f, 0.90f, 0.70f));
        
        var reflectance = new Reflectance(
            device.Color3(0.0f, 0.0f, 0.4f),
            device.Color3(0f, 0.1f, 0.2f),
            32);
        
        Material reflectiveMaterial = new PhongMaterial(device, lightPosition, lightSpectrum, reflectance);
        
        // Light Point
        const int rings = 10;
        var lightVertices = Sphere.GetIsoVertices(rings);
        var lightMaterial = new UniformMaterial(device, device.Color4(1f, 1f, 1f, 1f));
        
        var light = device.Object(
                device.Model(),
                "light",
                lightMaterial,
                Sphere.GetIsoTriangles(rings),
                new VertexAttribute("positionIn", lightVertices, 3))
            .Scale(SmallScale)
            .Translate(lightPosition.Vector);
        Scene.Add(light);
        #endregion

        _surfaceInstance = device.Object
        (
            device.World,
            SurfaceInstanceName,
            reflectiveMaterial,
            new VertexAttribute("positionIn", vertices, 3),
            new VertexAttribute("normalIn", vertices, 3)
            )
            .Scale(0.2f);
            
            /* Old version
            _surfaceInstance = device.Object
            (
                device.World,
                SurfaceInstanceName,
                material,
                new VertexAttribute("positionIn", vertices, 3)
            )
            .Scale(0.2f);
            */

        Scene.Add(_surfaceInstance);
    }

    private float[] GetVerticesOfSurface(int nOfSubdivisions)
    {
        //var vertices = new float[nOfSubdivisions * nOfSubdivisions * 3];
        var vertices = new List<float>();
        float offset = nOfSubdivisions / 2;

        for (var x = 0; x < nOfSubdivisions - 1; x++)
        {
            for (var y = 0; y < nOfSubdivisions - 1; y++)
            {
                AddSquareAtPosition(vertices, x, y, offset);
            }
        }

        return vertices.ToArray();
    }

    private void AddSquareAtPosition(List<float> vertices, int x, int y, float offset)
    {
        AddVertexAtPosition(vertices, x, y, offset);
        AddVertexAtPosition(vertices, x + 1, y, offset);
        AddVertexAtPosition(vertices, x, y + 1, offset);
        
        AddVertexAtPosition(vertices, x + 1, y + 1, offset);
        AddVertexAtPosition(vertices, x + 1, y, offset);
        AddVertexAtPosition(vertices, x, y + 1, offset);
    }

    private void AddVertexAtPosition(List<float> vertices, int x, int y, float offset)
    {
        float distanceToOrigin = MathF.Sqrt((x - offset) * (x - offset) + (y - offset) * (y - offset));
        
        vertices.Add(x - offset);
        vertices.Add(y - offset);
        vertices.Add(0.5f * MathF.Cos(distanceToOrigin));
        /*vertices[x + NumberOfSubdivisions * y] = x - offset;
        vertices[x + NumberOfSubdivisions * y + 1] = y - offset;
        vertices[x + NumberOfSubdivisions * y + 2] = 0;*/
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();
    }
}