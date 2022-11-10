using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaterMaterial {

    public static PhongMaterial Create(Device device, Color3 ambientColor, Point3 lightPosition)
    {
        var lightSpectrum = new Light(
            ambientColor,
            device.Color3(1f, 1f, 1f),
            device.Color3(0.90f, 0.90f, 0.70f));
        
        var reflectance = new Reflectance(
            device.Color3(0.0f, 0.0f, 0.4f),
            device.Color3(0f, 0.1f, 0.2f),
            32);
        
        return new PhongMaterial(device, lightPosition, lightSpectrum, reflectance);
    }
}