using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;

namespace WaterRendering.Components;

public class Container
{
    private const string InstanceName = "WaterContainer";

    public static RenderObject Create(Device device, float width, float length, float height, float thickness)
    {
        OpenGlMaterial material = new UniformMaterial(device, device.Color4(0.73f, 0.55f, 0.38f, 1f));
        float[] vertices = GetVertices(width, length, height, thickness);
        return device.Object
        (
            device.World,
            InstanceName,
            material,
            Cube.Triangles,
            new VertexAttribute("positionIn", Cube.Vertices, 3)
        );
    }

    private static float[] GetVertices(float width, float length, float height, float thickness)
    {
        var vertices = new List<float>();
        
        // Side 1
        vertices.Add(0);
        vertices.Add(0);
        vertices.Add(0);
        
        vertices.Add(0);
        vertices.Add(0);
        vertices.Add(height);
        
        vertices.Add(width);
        vertices.Add(0);
        vertices.Add(0);
        
        vertices.Add(width);
        vertices.Add(0);
        vertices.Add(height);
        
        
        return vertices.ToArray();
    }
}