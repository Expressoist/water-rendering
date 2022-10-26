using System.Collections;
using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class RippleRendering : CameraRendering
{

    public int Time { get; set; } = 1;

    private OtkRenderObject _surfaceInstance;
    private Device _device;
    
    private const string SurfaceInstanceName = "";
    private const int NumberOfSubdivisions = 100;
    
    private const float Gravity = 9.8f;
    private const float Density = 997;
    private const float SurfaceTension = 73E-3f;
    private const float Strength = 0.01f;

    private const float Frequency = 0.1f;
    private const float DampingCoefficient = 0.001f;
    private const float Amplitude = 3;

    private const float WaveSpeed = 0.1f;
    private const float SeparationFactor = 0.1f;
    
    public RippleRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0, 0, 0), camera)
    {
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        var material = new AmbientMaterial(device, device.Color3(0f, 1, 1f), device.Color3(0.8f, 0.2f, 0));
        
        _surfaceInstance = device.Object

        _device = device;
        _surfaceInstance = (OtkRenderObject)Device.Object
        (
            device.World,
            SurfaceInstanceName,
            new UniformMaterial(device, device.Color4(1f, 1f, 0.8f, 1f)),
            new VertexAttribute("positionIn", vertices, 3)
            );
        
        Scene.Add(_surfaceInstance);
    }

    private float[] GetVerticesOfSurface(int nOfSubdivisions)
    {
        //var vertices = new float[nOfSubdivisions * nOfSubdivisions * 3];
        var vertices = new List<float>();
        int offset = nOfSubdivisions / 2;

        for (var x = 0; x < nOfSubdivisions - 1; x++)
        {
            for (var y = 0; y < nOfSubdivisions - 1; y++)
            {
                AddSquareAtPosition(vertices, x - offset, y - offset);
            }
        }

        return vertices.ToArray();
    }

    private void AddSquareAtPosition(List<float> vertices, int x, int y)
    {
        AddVertexAtPosition(vertices, x, y);
        AddVertexAtPosition(vertices, x + 1, y);
        AddVertexAtPosition(vertices, x, y + 1);
        
        AddVertexAtPosition(vertices, x + 1, y + 1);
        AddVertexAtPosition(vertices, x + 1, y);
        AddVertexAtPosition(vertices, x, y + 1);
    }

    private void AddVertexAtPosition(List<float> vertices, int x, int y)
    {
        float distanceToOrigin = MathF.Max(MathF.Sqrt(x * x + y * y), 1f);

        float zPosition = MathF.Pow(MathF.E, -DampingCoefficient * Time) * MathF.Cos(distanceToOrigin - Frequency * Time) / distanceToOrigin;

        float z = Amplitude * MathF.Exp(-DampingCoefficient * Time) * MathF.Cos(distanceToOrigin - Frequency * Time) / distanceToOrigin;
        
        // float zPosition = MathF.Cos(distanceToOrigin - Time);
        
        vertices.Add(x);
        vertices.Add(y);
        vertices.Add(z);
        
        /*vertices[x + NumberOfSubdivisions * y] = x - offset;
        vertices[x + NumberOfSubdivisions * y + 1] = y - offset;
        vertices[x + NumberOfSubdivisions * y + 2] = 0;*/
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();

        Time++;
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        _surfaceInstance.UpdateVertices(new VertexAttribute("positionIn", vertices, 3));

        /*
        _surfaceInstance = _device.Object
        (
            _device.World,
            SurfaceInstanceName,
            new UniformMaterial(_device, _device.Color4(1f, 1f, 0.8f, 1f)),
            new VertexAttribute("positionIn", vertices, 3)
        );
        Scene.Add(_surfaceInstance);*/

        //_surfaceInstance.Transform;

        //Time++;
        //_surfaceInstance.Transform = Device.World.Scale4(1000 / Time);
    }
}