using SharpGfx;
using SharpGfx.Geometry;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaterRendering : CameraRendering
{
    private readonly List<Box> _floatingBoxes;
    private readonly OtkRenderObject _water;
    private readonly RenderObject _skyBox;
    private readonly Device _device;
    private int _time;
    
    private const float WaveAmplitude = 1.2f;
    private const float WaveLength = 80;
    private const float Frequency = 0.02f;
    private const float WaveNumber = 2 * MathF.PI / WaveLength;
    private const float SmallScale = 0.8f;
    
    public WaterRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, device.Color3(0.16f, 0.50f, 0.72f), camera)
    {
        var lightPosition = device.World.Point3(-60, 20, 60);
        
        _device = device;
        _skyBox = Sky.Create(device);
        _water = Water.Create(device, AmbientColor, lightPosition, _time);
        _floatingBoxes = new List<Box>
        {
            new(device, 2f, device.World.Vector2(2, 2), AmbientColor, lightPosition),
            new(device, 1f, device.World.Vector2(-2f, -2f), AmbientColor, lightPosition)
        };

        // Create light point
        const int rings = 10;
        var lightVertices = Sphere.GetIsoVertices(rings);
        var lightMaterial = new UniformMaterial(device, device.Color4(1f, 1f, 1f, 1f));
        
        var light = device.Object(
                device.Model(),
                "light",
                lightMaterial,
                Sphere.GetIsoTriangles(rings),
                new VertexAttribute("positionIn", lightVertices, 10))
            .Scale(SmallScale)
            .Translate(lightPosition.Vector);

        // Add components to scene
        Scene.Add(_skyBox);
        Scene.Add(light);
        Scene.Add(_water);
        _floatingBoxes.ForEach(box => Scene.Insert(1, box.RenderObject)); // Really ugly fix
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();

        _time++;
        
        Water.Update(_device, _water, _time);

        _skyBox.RotateY(-0.0003f);
        _floatingBoxes.ForEach(box => box.UpdateFloatingTransformation(Device, this));
    }
    
    public float CalculateWaveHeight(float x, float y)
    {
        return WaveAmplitude * MathF.Cos(WaveNumber * x - Frequency * _time);
    }
}