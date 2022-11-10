using System.Numerics;
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
            vec3 ambient = 8 *  (fragPos[2]+0.3) * ambientLightStrength * lightColor;
            vec3 fragColor3 = ambient * objectColor; 
            fragColor = vec4(fragColor3, 0.3);
          }";
     
     private const string PhongFragShader = @"
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

            uniform Light light;
            uniform Material material;
            uniform vec3 cameraPosition;

            out vec4 fragColor;

            void main()
            {
	            vec3 ambient = light.ambient * material.diffuse;

	            vec3 normDir = normalize(normal);
	            vec3 lightDir = normalize(light.position - fragPos); 
	            float cosTheta = max(dot(normDir, lightDir), 0.0);
	            vec3 diffuse = cosTheta * light.diffuse * material.diffuse;

                vec3 cameraDir = normalize(cameraPosition - fragPos);
                vec3 reflectDir = reflect(-lightDir, normDir);
                float intensity = pow(max(dot(cameraDir, reflectDir), 0.0), material.shininess);
                vec3 specular = intensity * light.specular * material.specular;

	            fragColor = vec4(ambient + diffuse + specular, 0.3);
            }";

    private static readonly string VertexShader = Resources.GetSource("basic.vert");
    private static readonly string PhongVertexShader = Resources.GetSource("normal_lighting.vert");

    public AmbientMaterial(
        Device device,
        Color3 lightColor,
        Color3 objectColor,
        Reflectance reflectance)
        : base(device, VertexShader, PhongFragShader)
    {
        Transparent = true;
        // all variables with 'uniform' in the FragShader need to be passed here
        DoInContext(() =>
        {
            // basic
            Set("ambientLightStrength", 0.8f);
            Set("lightColor", lightColor.Vector);
            Set("objectColor", objectColor.Vector);
        });
    }

    public AmbientMaterial(
         Device device, 
         Point3 lightPosition,
         Light light,
         Reflectance reflectance)
         : base(
             device, 
             PhongVertexShader, 
             PhongFragShader)
     {
         Transparent = true;
         var transparency = 0.3;
         
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
         });
    }
}