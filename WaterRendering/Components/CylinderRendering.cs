using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.Primitives;

namespace S2A2
{
    public class CylinderRendering : CameraRendering
    {
        public CylinderRendering(Device device, IVector2 size, Camera camera)
            : base(device, size, device.Color3(0, 0, 0), camera)
        {
            const int faceCount = 64;
            
            var material = new UniformMaterial(device, device.Color4(1, 0, 0, 1));
            var vertices = Cylinder.GetVertices(device.Model(), 4, 1, faceCount);
            var vertexAttribute = new VertexAttribute("positionIn", vertices, 3);
            var cylinder = device.Object(device.World, "cylinder", material, vertexAttribute);
            
            Scene.Add(cylinder);

            //Scene.Add(cylinder.Copy("1")); // TODO
            //Scene.Add(cylinder.Copy("2")); // TODO
            //Scene.Add(cylinder.Copy("3")); // TODO
            //Scene.Add(cylinder.Copy("4")); // TODO
        }
    }
}
