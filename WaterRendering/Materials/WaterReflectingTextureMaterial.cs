using System.Numerics;
using SharpGfx;
using SharpGfx.OpenGL.Shading;

namespace WaterRendering.Components;

public class WaterReflectingTextureMaterial : OpenGlMaterial
{
    private const string FragranceShader = @"
        #version 330 core

        in vec3 normalIn;
        in vec3 positionIn;
        out vec4 fragColor;

        uniform vec3 cameraPosition;

        void main()
        {             
            vec3 I = normalize(positionIn - cameraPosition);
            vec3 R = reflect(I, normalize(normalIn));

            fragColor = vec4(texture(textUnit, R).rgb, 1.0);
        }
    ";

    private const string VertexShader = @"   
        #version 330 core

        layout (location = 0) in vec3 aPos;
        layout (location = 1) in vec3 aNormal;

        out vec3 normalIn;
        out vec3 positionIn;

        uniform mat4 model;
        uniform mat4 cameraView;
        uniform mat4 projection;

        void main()
        {
            normalIn = mat3(transpose(inverse(model))) * aNormal;
            positionIn = vec3(model * vec4(aPos, 1.0));
            gl_Position = projection * cameraView * vec4(positionIn, 1.0);
        }  
    ";

    // https://learnopengl.com/Advanced-OpenGL/Cubemaps
    public WaterReflectingTextureMaterial(Device device)
        : base(device, VertexShader, FragranceShader)
    {
        DoInContext(() =>
        {
            
        });
    }
}