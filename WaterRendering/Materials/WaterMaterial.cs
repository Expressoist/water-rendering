using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WaterRendering.Components;

public class WaterMaterial {

    public static Material Create(Device device, Color3 ambientColor, Point3 lightPosition)
    {
        
        string texture = Path.Combine("Resources", "water-texture-2.png");
        using var image = Image.Load<Rgba32>(texture);
        TextureHandle woodTexture = device.Texture(image);
    
        var lightSpectrum = new Light(
            ambientColor,
            device.Color3(1f, 1f, 1f),
            device.Color3(0.50f, 0.90f, 0.70f));
        
        var reflectance = new Reflectance(
            device.Color3(0.1f, 0.3f, 0.8f),
            device.Color3(0.8f, 0.8f, 0.8f),
            24);
        
        return new PhongWithTextureMaterial(device, lightPosition, lightSpectrum, reflectance, woodTexture);
    }
}