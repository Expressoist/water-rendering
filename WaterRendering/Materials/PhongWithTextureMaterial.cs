using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class PhongWithTextureMaterial : OpenGlMaterial
{
    private const string PhongFragWithTextureFragShader = @"
          #version 410

            struct Light {
                vec3 position;
              
                vec3 ambient;
                vec3 diffuse;
                vec3 specular;
            };

            struct Material {
                vec3 diffuse;
                vec3 specular;

                float shininess;
            };

            in vec3 fragPos;
            in vec3 normal;
            in vec2 texCoord;

            uniform Light light;
            uniform Material material;
            uniform vec3 cameraPosition;
            uniform sampler2D texUnit;

            out vec4 fragColor;

            void main()
            {
	            vec3 ambient = vec3(0.5, 0.5, 0.5);

	            vec3 normDir = normalize(normal);
	            vec3 lightDir = normalize(light.position - fragPos); 
                float cosTheta = max(dot(normDir, lightDir), 0.0);
	            vec3 diffuse = cosTheta * light.diffuse * material.diffuse;

                vec3 cameraDir = normalize(cameraPosition - fragPos);
                vec3 reflectDir = reflect(-lightDir, normDir);
                float intensity = pow(max(dot(cameraDir, reflectDir), 0.0), material.shininess);
                vec3 specular = intensity * light.specular * material.specular;

                fragColor = vec4(ambient + diffuse + specular, 1.0) * texture(texUnit, texCoord);
            }";

    private static readonly string PhongWithTextureVertexShader = Resources.GetSource("texture_normal_lighting.vert");
    private readonly TextureHandle _handle;
    private readonly int _unit;
    
    public PhongWithTextureMaterial(
        Device device, 
        Point3 lightPosition,
        Light light,
        Reflectance reflectance,
        TextureHandle handle,
        int unit = 0)
        : base(
            device, 
            PhongWithTextureVertexShader, 
            PhongFragWithTextureFragShader)
    {
        _unit = unit;
        _handle = handle;
        
        Transparent = true;
         
        // all variables with 'uniform' in the FragShader need to be passed here
        DoInContext(() =>
        {
            Set("light.position", lightPosition.Vector);
            Set("light.ambient", light.Ambient.Vector);
            Set("light.diffuse", light.Diffuse.Vector);
            Set("material.diffuse", reflectance.Diffuse.Vector);
            Set("light.specular", light.Specular.Vector);
            Set("material.specular", reflectance.Specular.Vector);
            Set("material.shininess", reflectance.Shininess);
            Set("texUnit", _unit);
        });
    }
    
    public override void Apply()
    {
        _handle.ActivateTexture(_unit);
        base.Apply();
    }

    public override void UnApply()
    {
        base.UnApply();
        Device.ClearTexture(_unit);
    }

    protected override void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        base.Dispose(disposing);
    }

    private void ReleaseUnmanagedResources()
    {
        _handle.DeleteTexture();
    }
}