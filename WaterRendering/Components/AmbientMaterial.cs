using System.Numerics;
using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class AmbientMaterial : OpenGlMaterial
{
    /*
        https://www.khronos.org/opengl/wiki/Fragment_Shader
        https://learnopengl.com/Getting-started/Shaders
        Fragrance Shader
        Input variables:
        in vec4 gl_FragCoord;
        in bool gl_FrontFacing;
        in vec2 gl_PointCoord;
        
        
    
    */
     private const string FragShader = @"
          #version 410

          in vec3 fragPos;
          uniform float ambientLightStrength;
          uniform vec3 lightColor;
          uniform vec3 objectColor;
          out vec4 fragColor;

          void main() {
            vec3 ambient = 8 *  (fragPos[2]+0.3) * ambientLightStrength * lightColor;
            vec3 fragColor3 = ambient * objectColor; 
            fragColor = vec4(fragColor3, 1.0); 
          }";

    private static readonly string VertexShader = Resources.GetSource("basic.vert");
        
     public AmbientMaterial(Device device, Color3 lightColor, Color3 objectColor)
    : base(device, VertexShader, FragShader)
    {
        // all variables with 'uniform' in the FragShader need to be passed here
        DoInContext(() =>
        {
            // basic
            Set("ambientLightStrength", 0.8f);
            Set("lightColor", lightColor.Vector);
            Set("objectColor", objectColor.Vector);
        });
    }
}