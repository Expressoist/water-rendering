using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace WaterRendering.Components;

public class Box
{
    private const string PlankInstanceName = "Plank";

    public readonly RenderObject RenderObject;
    
    private readonly float _scale;
    private readonly IVector2 _translation;
    private readonly IVector2 _startPosition;
    private readonly IVector2 _endPosition;
    
    private static readonly string Texture = Path.Combine("Resources", "wooden_chest.jpg");

    public Box(Device device, float scale, IVector2 translation, Color3 ambientColor, Point3 lightPosition)
    {
        _scale = scale;
        _translation = translation;
        _startPosition = device.World.Vector2(translation.X - 0.5f * scale, translation.Y + 0.5f * scale);
        _endPosition = device.World.Vector2(translation.X + 0.5f * scale, translation.Y + 0.5f * scale);
        RenderObject = CreateRenderObject(device, scale, translation, ambientColor, lightPosition);
    }

    private static RenderObject CreateRenderObject(Device device, float scale, IVector2 translation, Color3 ambientColor, Point3 lightPosition)
    {
        var renderObject = device.Object
        (
            device.Model(),
            PlankInstanceName,
            GetMaterial(device, ambientColor, lightPosition),
            Cube.SeparateTriangles,
            new VertexAttribute("positionIn", Cube.Vertices, 3),
            new VertexAttribute("texCoordIn", Cube.TextureCoordinates, 2),
            new VertexAttribute("normalIn", Cube.Normals, 3)
        );
        renderObject.Scale(scale);
        renderObject.Translate(device.World.Vector3(translation.X, 0, translation.X));
        return renderObject;
    }

    public void UpdateFloatingTransformation(Device device, WaveRendering waveRendering)
    {
        float a = waveRendering.CalculateWaveHeight(_startPosition.X, _startPosition.Y);
        float b = waveRendering.CalculateWaveHeight(_endPosition.X, _endPosition.Y);

        float angle = MathF.Atan(b - a);
        float height = (a + b) / (2 * _scale);

        var translation = device.World.Vector3(_translation.X, height, _translation.Y);

        RenderObject.Transform = device.World.RotationZ4(angle) *
                                 device.World.Translation4(translation) *
                                 device.World.Scale4(_scale);
    }

    public static PhongWithTextureMaterial GetMaterial(Device device, Color3 ambientColor, Point3 lightPosition)
    {
        using var image = Image.Load<Rgba32>(Texture);
        TextureHandle woodTexture = device.Texture(image);
        
        var lightSpectrum = new Light(
            ambientColor,
            device.Color3(0.45f, 0.65f, 0.80f),
            device.Color3(0.80f, 0.80f, 0.60f));

        var reflectance = new Reflectance(
            device.Color3(0.4f, 0.4f, 0.4f),
            device.Color3(0.5f, 0.4f, 0.4f),
            8);

        return new PhongWithTextureMaterial(device, lightPosition, lightSpectrum, reflectance, woodTexture);
    }
}