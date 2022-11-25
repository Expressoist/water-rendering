using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace WaterRendering.Components;

public static class Sky
{
    const int Resolution = 32;
    const int TextureUnit = 0;
    
    public static void AddToScene(Device device, List<RenderObject> scene)
    {
        TextureHandle texture;
        using (var image = Image.Load<Rgba32>(Path.Combine("Resources", "sky1.jpg")))
        {
            texture = device.Texture(image);
        }

        var skyMaterial = new FlatTextureMaterial(device, texture, TextureUnit);
        var skySphere = device.Object(
            device.World,
            "sky",
            skyMaterial,
            Sphere.GetTriangles(Resolution, Resolution),
            new VertexAttribute("positionIn", Sphere.GetVertices(Resolution, Resolution), 3),
            new VertexAttribute("texCoordIn", GetTextureCoordinates(Resolution, Resolution), 2)
        ).Scale(55).RotateX(MathF.PI / 2);
        
        scene.Add(skySphere);
    }
    
    private static float[] GetTextureCoordinates(int longitudeCount, int latitudeCount)
    {
        var vertices = new float[(longitudeCount + 1) * (latitudeCount + 1) * 2];

        float lngStep = 1f / longitudeCount;
        float latStep = 1f / latitudeCount;

        int i = 0;
        float latitude = 0;
        for (int lat = 0; lat <= latitudeCount; lat++)
        {
            // add (sectorCount+1) vertices per stack
            // the first and last vertices have same position and normal, but different tex coords
            float longitude = 0;
            for (int lng = 0; lng <= longitudeCount; lng++)
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