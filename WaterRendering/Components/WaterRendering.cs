using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public abstract class WaterRendering : CameraRendering
{
    protected int Time { get; set; }
    private OtkRenderObject _surfaceInstance;
    
    private const int NumberOfSubdivisions = 100;
    private const string SurfaceInstanceName = "WaterRender";
    
    protected WaterRendering(Device device, IVector2 size, Color3 ambientColor, Camera camera)
        : base(device, size, ambientColor, camera)
    {
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
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
        var vertices = new List<float>();
        int offset = nOfSubdivisions / 2;
        
        for (var x = 0; x < nOfSubdivisions - 1; x++)
        {
            for (var y = 0; y < nOfSubdivisions - 1; y++)
            {
                AddSquareAtPosition(ref vertices, x - offset, y - offset);
            }
        }
        
        return vertices.ToArray();
    }
    
    private void AddSquareAtPosition(ref List<float> vertices, int x, int y)
    {
        AddVertexAtPosition(vertices, x, y);
        AddVertexAtPosition(vertices, x + 1, y);
        AddVertexAtPosition(vertices, x, y + 1);
        
        AddVertexAtPosition(vertices, x + 1, y + 1);
        AddVertexAtPosition(vertices, x + 1, y);
        AddVertexAtPosition(vertices, x, y + 1);
    }

    protected abstract void AddVertexAtPosition(List<float> vertices, int x, int y);
    
    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();

        Time++;
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        _surfaceInstance.UpdateVertices(new VertexAttribute("positionIn", vertices, 3));
    }
}