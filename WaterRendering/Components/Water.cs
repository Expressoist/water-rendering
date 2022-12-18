using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WaterRendering.Materials;

namespace WaterRendering.Components;

public class Water
{
    private const string SurfaceInstanceName = "WaterRender";
    
    // Level of Detail
    private const int NumberOfSubdivisions = 80;
    private const int NumberOfPoints = NumberOfSubdivisions + 1;
    private const int NumberOfSquares = NumberOfSubdivisions * NumberOfSubdivisions;
    
    // Wave Constants
    private const float Frequency = 0.02f;
    private const float WaveAmplitude = 1.2f;
    private const float WaveLength = 80;
    private const float WaveNumber = 2 * MathF.PI / WaveLength;
    private const float SurfaceScaleFactor = 1.4f;
    
    // Array Constants
    private const int VerticesSize = NumberOfPoints * NumberOfPoints * 3;
    private const int NormalsSize = NumberOfPoints * NumberOfPoints * 3;
    private const int TextureCoordinateSize = NumberOfPoints * NumberOfPoints * 2;
    private const int FacesSize = NumberOfSquares * 6;

    public OtkRenderObject RenderObject { get; }

    private readonly float[] _vertices = new float[VerticesSize];
    private readonly float[] _normals = new float[NormalsSize];
    private readonly uint[] _faces = new uint[FacesSize];
    private readonly float[] _textureCoordinates = new float[TextureCoordinateSize];

    private static readonly string Texture = Path.Combine("Resources", "water_texture.png");

    private Water(Device device, Color3 ambientColor, Point3 lightPosition, int time)
    {
        CalculateTextureCoordinates(_textureCoordinates);
        CalculateVertices(_vertices, time);
        CalculateFaces(_faces);
        CalculateNormals(device.World, _faces, _vertices, _normals);

        RenderObject = (OtkRenderObject)device.Object
        (
            device.World,
            SurfaceInstanceName,
            GetMaterial(device, ambientColor, lightPosition),
            _faces,
            new VertexAttribute("positionIn", _vertices, 3),
            new VertexAttribute("texCoordIn", _textureCoordinates, 2),
            new VertexAttribute("normalIn", _normals, 3)
        );
    }

    public static Water Create(Device device, Color3 ambientColor, Point3 lightPosition, int time)
    {
        return new Water(device, ambientColor, lightPosition, time);
    }

    public void Update(Device device, int time)
    {
        CalculateVertices(_vertices, time);
        CalculateNormals(device.World, _faces, _vertices, _normals);

        RenderObject.UpdateVertices(
            new VertexAttribute("positionIn", _vertices, 3),
            new VertexAttribute("texCoordIn", _textureCoordinates, 2),
            new VertexAttribute("normalIn", _normals, 3));
    }

    private static void CalculateTextureCoordinates(float[] outArray)
    {
        for (int x = 0; x <= NumberOfSubdivisions; x++)
        {
            for (int y = 0; y <= NumberOfSubdivisions; y++)
            {
                int index = 2 * (y + NumberOfPoints * x);
                outArray[index] = x / (float)NumberOfSubdivisions;
                outArray[index + 1] = y / (float)NumberOfSubdivisions;
            }
        }
    }

    private static void CalculateNormals(Space space, uint[] faces, float[] vertices, float[] outArray)
    {
        for (var i = 0; i < faces.Length - 3; i += 3)
        {
            var a = space.Point3(vertices[faces[i] * 3 + 0], vertices[faces[i] * 3 + 1], vertices[faces[i] * 3 + 2]);
            var b = space.Point3(vertices[faces[i + 1] * 3 + 0], vertices[faces[i + 1] * 3 + 1],
                vertices[faces[i + 1] * 3 + 2]);
            var c = space.Point3(vertices[faces[i + 2] * 3 + 0], vertices[faces[i + 2] * 3 + 1],
                vertices[faces[i + 2] * 3 + 2]);

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

    private static void CalculateVertices(float[] outArray, int time)
    {
        const int offset = NumberOfSubdivisions / 2;
        for (var x = 0; x <= NumberOfSubdivisions; x++)
        {
            for (var y = 0; y <= NumberOfSubdivisions; y++)
            {
                int index = 3 * (y + NumberOfPoints * x);
                AddVertexAtPosition(outArray, (x - offset) * SurfaceScaleFactor, (y - offset) * SurfaceScaleFactor,
                    index, time);
            }
        }
    }

    private static void AddVertexAtPosition(float[] vertices, float x, float y, int index, int time)
    {
        float z = CalculateWaveHeight(x, time);
        z += CalculatePseudoRandomFactor(x, y, time);

        vertices[index] = x;
        vertices[index + 1] = z;
        vertices[index + 2] = y;
    }

    private static float CalculateWaveHeight(float x, int time)
    {
        return WaveAmplitude * MathF.Cos(WaveNumber * x - Frequency * time);
    }

    private static float CalculatePseudoRandomFactor(float x, float y, int time)
    {
        float randomX = 0.05f * MathF.Cos(0.4f * x - Frequency * 1.5f * time)
                        + 0.05f * MathF.Cos(0.6f * x - Frequency * 2f * time);

        float randomY = 0.05f * MathF.Cos(0.3f * y + Frequency * 1.5f * time)
                        + 0.05f * MathF.Cos(0.5f * y + Frequency * 2f * time);

        return randomX + randomY;
    }

    private static void CalculateFaces(uint[] outArray)
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

    private static Material GetMaterial(Device device, Color3 ambientColor, Point3 lightPosition)
    {
        using var image = Image.Load<Rgba32>(Texture);
        TextureHandle texture = device.Texture(image);

        var lightSpectrum = new Light(
            ambientColor,
            device.Color3(1f, 1f, 1f),
            device.Color3(0.50f, 0.90f, 0.70f));

        var reflectance = new Reflectance(
            device.Color3(0.1f, 0.3f, 0.8f),
            device.Color3(0.8f, 0.8f, 0.8f),
            24);

        return new PhongTextureMaterial(device, lightPosition, lightSpectrum, reflectance, texture);
    }
}