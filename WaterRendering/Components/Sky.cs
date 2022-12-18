using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace WaterRendering.Components;

public static class Sky
{
    private const string InstanceName = "Sky";
    private const int Resolution = 32;
    private const int TextureUnit = 0;

    public static RenderObject Create(Device device)
    {
        TextureHandle texture;
        using (var image = Image.Load<Rgba32>(Path.Combine("Resources", "sky_texture.jpg")))
        {
            texture = device.Texture(image);
        }

        var skyMaterial = new FlatTextureMaterial(device, texture, TextureUnit);
        var skySphere = device.Object(
                device.World,
                InstanceName,
                skyMaterial,
                Sphere.GetTriangles(Resolution, Resolution),
                new VertexAttribute("positionIn", Sphere.GetVertices(Resolution, Resolution), 3),
                new VertexAttribute("texCoordIn", GetTextureCoordinates(Resolution, Resolution), 2)
            )
            .Scale(55)
            .RotateX(MathF.PI / 2)
            .Translate(device.World.Vector3(0, -2, 0));

        return skySphere;
    }

    private static float[] GetTextureCoordinates(int longitudeCount, int latitudeCount)
    {
        var vertices = new float[(longitudeCount + 1) * (latitudeCount + 1) * 2];
        float lngStep = 1f / longitudeCount;
        float latStep = 1f / latitudeCount;
        var i = 0;
        float latitude = 0;
        for (var lat = 0; lat <= latitudeCount; lat++)
        {
            float longitude = 0;
            for (var lng = 0; lng <= longitudeCount; lng++)
            {
                vertices[i++] = longitude;
                vertices[i++] = latitude;
                longitude -= lngStep;
            }

            latitude += latStep;
        }

        return vertices;
    }
}