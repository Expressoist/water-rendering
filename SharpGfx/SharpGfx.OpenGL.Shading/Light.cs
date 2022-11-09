using SharpGfx.Primitives;

namespace SharpGfx.OpenGL.Shading
{
    public readonly struct Light
    {
        public readonly Color3 Ambient;
        public readonly Color3 Diffuse;
        public readonly Color3 Specular;

        public Light(Color3 ambient, Color3 diffuse, Color3 specular)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
        }
    }
}
