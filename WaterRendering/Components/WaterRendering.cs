using OpenTK.Compute.OpenCL;
using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public abstract class WaterRendering : CameraRendering
{
    protected int Time { get; set; }
    private OtkRenderObject _surfaceInstance;
    
    private const int NumberOfSubdivisions = 50;
    private const string SurfaceInstanceName = "WaterRender";
    
    private const float SmallScale = 0.8f;

    private Device _device;
    
    protected WaterRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0.16f, 0.50f, 0.72f), camera)
    {
        float[] vertices = GetVerticesOfSurface(NumberOfSubdivisions);
        
        var lightPosition = device.World.Point3(30, 20, 0);

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

        _device = device;
        var triangles = GetIndicesOfSurface(NumberOfSubdivisions);
        var normals = GetNormals(device.World, triangles, vertices);

        var test = Tetrahedron.CreateFromPointVectors(vertices, normals, 1);
        
        /*var test1 = (OtkRenderObject) Device.Object
        (
            device.World,
            "Hello",
            new UniformMaterial(device, device.Color4(1,1,1,1)),
            test.Item2,
            new VertexAttribute("positionIn", test.Item1, 3)
        );
        
        Scene.Add(test1);*/

        // WaterMaterial.Create(device, AmbientColor, lightPosition)

        _surfaceInstance = (OtkRenderObject) Device.Object
        (
            device.World,
            SurfaceInstanceName,
            WaterMaterial.Create(device, AmbientColor, lightPosition),
            GetIndicesOfSurface(NumberOfSubdivisions),
            new VertexAttribute("positionIn", vertices, 3),
            new VertexAttribute("normalIn", normals, 3)
        );
        
        Scene.Add(_surfaceInstance);
    }


    private float[] GetNormals(Space space, uint[] indices, float[] vertices)
    {
        var normals = new float[vertices.Length];
        for (var i = 0; i < indices.Length - 3; i += 3)
        {
            var a = space.Point3(vertices[indices[i] * 3 + 0], vertices[indices[i] * 3 + 1], vertices[indices[i] * 3 + 2]);
            var b = space.Point3(vertices[indices[i + 1] * 3 + 0], vertices[indices[i + 1] * 3 + 1], vertices[indices[i + 1] * 3 + 2]);
            var c = space.Point3(vertices[indices[i + 2] * 3 + 0], vertices[indices[i + 2] * 3 + 1], vertices[indices[i + 2] * 3 + 2]);

            var vectorNormal = IVector3.Cross(a - b, c - b).Normalized();

            normals[indices[i] * 3 + 0] = MathF.Abs(vectorNormal.X);
            normals[indices[i] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            normals[indices[i] * 3 + 2] = MathF.Abs(vectorNormal.Z);
            
            normals[indices[i + 1] * 3 + 0] = MathF.Abs(vectorNormal.X);
            normals[indices[i + 1] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            normals[indices[i + 1] * 3 + 2] = MathF.Abs(vectorNormal.Z);
            
            normals[indices[i + 2] * 3 + 0] = MathF.Abs(vectorNormal.X);
            normals[indices[i + 2] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            normals[indices[i + 2] * 3 + 2] = MathF.Abs(vectorNormal.Z);
        }

        return normals;
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
        var triangles = GetIndicesOfSurface(NumberOfSubdivisions);
        var normals = GetNormals(_device.World, triangles, vertices);
        _surfaceInstance.UpdateVertices(
            new VertexAttribute("positionIn", vertices, 3),
            new VertexAttribute("normalIn", normals, 3));
    }
}