using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class AmbientMaterial : OpenGlMaterial
{
    private const string FragShader = @"
        #version 410

        in vec3 fragPos;
        uniform float ambientLightStrength;
        uniform vec3 lightColor;
        uniform vec3 objectColor;
        out vec4 fragColor;

        void main() {
          vec3 ambient = 200 * fragPos[2] * ambientLightStrength * lightColor;
          vec3 fragColor3 = ambient * objectColor; 
          fragColor = vec4(fragColor3, 0.5); 
        }";

    public AmbientMaterial(Device device, Color3 lightColor, Color3 objectColor)
    : base(device, Resources.GetSource("basic.vert"), FragShader)
    {
        DoInContext(() =>
        {
            Set("ambientLightStrength", 0.2f);
            Set("lightColor", lightColor.Vector);
            Set("objectColor", objectColor.Vector);
        });
    }
}