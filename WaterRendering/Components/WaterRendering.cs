using OpenTK.Compute.OpenCL;
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
    
    protected WaterRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0.16f, 0.50f, 0.72f), camera)
    {
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        // OpenGlMaterial material = new AmbientMaterial(device, device.Color3(0f, 1, 1f), device.Color3(0.8f, 0.2f, 0));
        OpenGlMaterial material = new UniformMaterial(device, device.Color4(0f, 1, 1f, 1f));

        _surfaceInstance = (OtkRenderObject) Device.Object
        (
            device.World,
            SurfaceInstanceName,
            material,
            GetIndicesOfSurface(NumberOfSubdivisions),
            new VertexAttribute("positionIn", vertices, 3)
        );
        
        Scene.Add(_surfaceInstance);
    }


    private uint[] GetIndicesOfSurface(uint nOfSubdivisions)
    {
        uint nOfSquares = nOfSubdivisions * nOfSubdivisions;
        var indices = new uint[nOfSquares * 6];
        var i = 0;
        var nOfPoints = (nOfSubdivisions + 1);
        
        for (uint row = 0; row < nOfSubdivisions; row++)
        {
            for (uint column = 0; column < nOfSubdivisions; column++)
            {
                //  0 - *
                //  | \ |
                //  * - *
                indices[i++] = nOfPoints * row + column;
            
                //  * - 0
                //  | \ |
                //  * - *
                indices[i++] = nOfPoints * row + (column + 1);
            
                //  * - *
                //  | \ |
                //  * - 0
                indices[i++] = nOfPoints * (row + 1) + (column + 1);
            
                //  0 - *
                //  | \ |
                //  * - *
                indices[i++] = nOfPoints * row + column;
            
                //  * - *
                //  | \ |
                //  0 - *
                indices[i++] = nOfPoints * (row + 1) + column;
            
                //  * - *
                //  | \ |
                //  * - 0
                indices[i++] = nOfPoints * (row + 1) + (column + 1);
            }
        }

        return indices;
    }
    
    private float[] GetVerticesOfSurface(int nOfSubdivisions)
    {
        var vertices = new List<float>();
        int offset = nOfSubdivisions / 2;
        
        for (var x = 0; x <= nOfSubdivisions; x++)
        {
            for (var y = 0; y <= nOfSubdivisions; y++)
            {
                AddVertexAtPosition(vertices, x - offset, y - offset);
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