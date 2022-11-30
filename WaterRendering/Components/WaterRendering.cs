using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public abstract class WaterRendering : CameraRendering
{
    protected int Time { get; set; }
    protected Point3 LightPosition { get; set; }

    private float[] Vertices { get; } = new float[NumberOfPoints * NumberOfPoints * 3];
    private uint[] Faces { get; } = new uint[NumberOfSquares * 6];
    private float[] Normals { get; } = new float[NumberOfPoints * NumberOfPoints * 3];
    private float[] TextureCoordinates { get; } = new float[NumberOfPoints * NumberOfPoints * 2];
    
    private readonly OtkRenderObject _surfaceInstance;
    private readonly Device _device;
    
    private const int NumberOfSubdivisions = 80;
    private const int NumberOfPoints = NumberOfSubdivisions + 1;
    private const int NumberOfSquares = NumberOfSubdivisions * NumberOfSubdivisions;

    private const float SurfaceScaleFactor = 1.4f;
    private const string SurfaceInstanceName = "WaterRender";
    private const float SmallScale = 0.8f;
    
    protected WaterRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0.16f, 0.50f, 0.72f), camera)
    {
        Sky.AddToScene(device, Scene);

        LightPosition = device.World.Point3(-60, 20, 60);

        // Light Point
        const int rings = 10;
        var lightVertices = Sphere.GetIsoVertices(rings);
        var lightMaterial = new UniformMaterial(device, device.Color4(1f, 1f, 1f, 1f));
        
        var light = device.Object(
                device.Model(),
                "light",
                lightMaterial,
                Sphere.GetIsoTriangles(rings),
                new VertexAttribute("positionIn", lightVertices, 10))
            .Scale(SmallScale)
            .Translate(LightPosition.Vector);
        Scene.Add(light);

        // Create Water
        _device = device;
        CalculateVertices(Vertices);
        CalculateFaces(Faces);
        CalculateNormals(device.World, Faces, Vertices, Normals);
        CalculateTextureCoordinates(TextureCoordinates);

        _surfaceInstance = (OtkRenderObject) Device.Object
        (
            device.World,
            SurfaceInstanceName,
            WaterMaterial.Create(device, AmbientColor, LightPosition),
            Faces,
            new VertexAttribute("positionIn", Vertices, 3),
            new VertexAttribute("texCoordIn", TextureCoordinates, 2),
            new VertexAttribute("normalIn", Normals, 3)
        );
        
        Scene.Add(_surfaceInstance);
    }

    private void CalculateTextureCoordinates(float[] outArray)
    {
        for (int x = 0; x <= NumberOfSubdivisions; x++)
        {
            for (int y = 0; y <= NumberOfSubdivisions; y++)
            {
                int index = 2 * (y + NumberOfPoints * x);
                outArray[index] = (float) x / (float) NumberOfSubdivisions;
                outArray[index + 1] = (float) y / (float) NumberOfSubdivisions;
            }
        }
    }

    private void CalculateNormals(Space space, uint[] faces, float[] vertices, float[] outArray)
    {
        for (var i = 0; i < faces.Length - 3; i += 3)
        {
            var a = space.Point3(vertices[faces[i] * 3 + 0], vertices[faces[i] * 3 + 1], vertices[faces[i] * 3 + 2]);
            var b = space.Point3(vertices[faces[i + 1] * 3 + 0], vertices[faces[i + 1] * 3 + 1], vertices[faces[i + 1] * 3 + 2]);
            var c = space.Point3(vertices[faces[i + 2] * 3 + 0], vertices[faces[i + 2] * 3 + 1], vertices[faces[i + 2] * 3 + 2]);

            var vectorNormal = IVector3.Cross(a - b, c - b).Normalized();

            outArray[faces[i] * 3 + 0] = MathF.Abs(vectorNormal.X);
            outArray[faces[i] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            outArray[faces[i] * 3 + 2] = MathF.Abs(vectorNormal.Z);
            
            outArray[faces[i + 1] * 3 + 0] = MathF.Abs(vectorNormal.X);
            outArray[faces[i + 1] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            outArray[faces[i + 1] * 3 + 2] = MathF.Abs(vectorNormal.Z);
            
            outArray[faces[i + 2] * 3 + 0] = MathF.Abs(vectorNormal.X);
            outArray[faces[i + 2] * 3 + 1] = MathF.Abs(vectorNormal.Y);
            outArray[faces[i + 2] * 3 + 2] = MathF.Abs(vectorNormal.Z);
        }
    }


    private void CalculateFaces(uint[] outArray)
    {
        var i = 0;
        for (uint row = 0; row < NumberOfSubdivisions; row++)
        {
            for (uint column = 0; column < NumberOfSubdivisions; column++)
            {
                //  0 - *
                //  | \ |
                //  * - *
                outArray[i++] = NumberOfPoints * row + column;
            
                //  * - 0
                //  | \ |
                //  * - *
                outArray[i++] = NumberOfPoints * row + (column + 1);
            
                //  * - *
                //  | \ |
                //  * - 0
                outArray[i++] = NumberOfPoints * (row + 1) + (column + 1);
            
                //  0 - *
                //  | \ |
                //  * - *
                outArray[i++] = NumberOfPoints * row + column;
            
                //  * - *
                //  | \ |
                //  0 - *
                outArray[i++] = NumberOfPoints * (row + 1) + column;
            
                //  * - *
                //  | \ |
                //  * - 0
                outArray[i++] = NumberOfPoints * (row + 1) + (column + 1);
            }
        }
    }
    
    private void CalculateVertices(float[] outArray)
    {
        const int offset = NumberOfSubdivisions / 2;
        for (var x = 0; x <= NumberOfSubdivisions; x++)
        {
            for (var y = 0; y <= NumberOfSubdivisions; y++)
            {
                int index = 3 * (y + NumberOfPoints * x);
                AddVertexAtPosition(outArray, (x - offset) * SurfaceScaleFactor, (y - offset) * SurfaceScaleFactor, index);
            }
        }
    }

    protected abstract void AddVertexAtPosition(float[] vertices, float x, float y, int index);

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();

        Time++;
        CalculateVertices(Vertices);
        CalculateNormals(_device.World, Faces, Vertices, Normals);
        _surfaceInstance.UpdateVertices(
            new VertexAttribute("positionIn", Vertices, 3),
            new VertexAttribute("texCoordIn", TextureCoordinates, 2),
            new VertexAttribute("normalIn", Normals, 3));
    }
}