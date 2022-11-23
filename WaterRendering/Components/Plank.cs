using System.Data.Common;
using System.Drawing;
using System.Net.NetworkInformation;
using OpenTK.Mathematics;
using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace WaterRendering.Components;

public class Plank
{
    private const string PlankInstanceName = "Plank";

    public RenderObject RenderObject { get; }
    const int TextureUnit = 0;

    private float _scale;
    private IVector2 _translation;
    private IVector2 _startPosition;
    private IVector2 _endPosition;

    public Plank(Device device, float scale, IVector2 translation, Color3 ambientColor, Point3 lightPosition)
    {
        _scale = scale;
        _translation = translation;
        _startPosition = device.World.Vector2(translation.X - 0.5f * scale, translation.Y + 0.5f * scale);
        _endPosition = device.World.Vector2(translation.X + 0.5f * scale, translation.Y + 0.5f * scale);
        RenderObject = CreateRenderObject(device, scale, translation, ambientColor, lightPosition);
    }

    private static RenderObject CreateRenderObject(Device device, float scale, IVector2 translation, Color3 ambientColor, Point3 lightPosition)
    {
        var phongMaterial = GetMaterial(device, ambientColor, lightPosition);
        //OpenGlMaterial uniformMaterial = new UniformMaterial(device, device.Color4(0.73f, 0.55f, 0.38f, 1f));

        var instance = device.Object
        (
            //device.World,
            device.Model(),
            PlankInstanceName,
            phongMaterial,
            Cube.Triangles,
            new VertexAttribute("positionIn", Cube.Vertices, 3),
            new VertexAttribute("texCoordIn", Cube.Vertices, 3),
            new VertexAttribute("normalIn", Cube.Vertices, 3)
        );

        instance.Scale(scale);
        instance.Translate(device.World.Vector3(translation.X, 0, translation.X));
        return instance;
    }

    public void RecalculateFloating(Device device, WaveRendering waveRendering)
    {
        float a = waveRendering.CalculateWaveHeight(_startPosition.X, _startPosition.Y);
        float b = waveRendering.CalculateWaveHeight(_endPosition.X, _endPosition.Y);

        float angle = MathF.Atan(b - a);
        float height = (a + b) / 2;

        var translation = device.World.Vector3(_translation.X, height, _translation.Y);

        RenderObject.Transform = device.World.RotationZ4(angle) *
                                 device.World.Translation4(translation) *
                                 device.World.Scale4(_scale);
    }

    private static PhongWithTextureMaterial GetMaterial(Device device, Color3 ambientColor, Point3 lightPosition)
    {
        //var woodMaterial = new FlatTextureMaterial(device, woodTexture, TextureUnit);

        TextureHandle woodTexture;
        using var image = Image.Load<Rgba32>(Path.Combine("Resources", "wooden_chest.jpg"));
        {
            woodTexture = device.Texture(image);
        }
        
        var lightSpectrum = new Light(
            ambientColor,
            device.Color3(0.45f, 0.65f, 0.80f),
            device.Color3(0.80f, 0.80f, 0.60f));

        var reflectance = new Reflectance(
            device.Color3(0.4f, 0.4f, 0.4f),
            device.Color3(0.5f, 0.4f, 0.4f),
            8);

        return new PhongWithTextureMaterial(device, lightPosition, lightSpectrum, reflectance, woodTexture, TextureUnit);
    }
}