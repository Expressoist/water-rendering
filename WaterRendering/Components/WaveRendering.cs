using SharpGfx;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaveRendering : WaterRendering
{
    private readonly List<Box> _floatingPlanks;

    private const float WaveAmplitude = 1.2f;
    private const float WaveLength = 80;
    private const float Frequency = 0.02f;
    private const float WaveNumber = 2 * MathF.PI / WaveLength;

    public WaveRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, camera)
    {
        _floatingPlanks = new List<Box>
        {
            new(device, 2f, device.World.Vector2(2, 2), AmbientColor, LightPosition),
            new(device, 1f, device.World.Vector2(-2f, -2f), AmbientColor, LightPosition)
        };
        
        _floatingPlanks.ForEach(plank => Scene.Insert(1, plank.RenderObject)); // Really ugly fix
    }

    protected override void AddVertexAtPosition(float[] vertices, float x, float y, int index)
    {
        float z = CalculateWaveHeight(x, y);
        z += CalculatePseudoRandomFactor(x, y);

        vertices[index] = x;
        vertices[index + 1] = z;
        vertices[index + 2] = y;
    }

    public float CalculateWaveHeight(float x, float y)
    {
        return WaveAmplitude * MathF.Cos(WaveNumber * x - Frequency * Time);
    }

    private float CalculatePseudoRandomFactor(float x, float y)
    {
        float randomX = 0.05f * MathF.Cos(0.4f * x - Frequency * 1.5f * Time)
                        + 0.05f * MathF.Cos(0.6f * x - Frequency * 2f * Time);
        
        float randomY = 0.05f * MathF.Cos(0.3f * y + Frequency * 1.5f * Time)
                        + 0.05f * MathF.Cos(0.5f * y + Frequency * 2f * Time);

        return randomX + randomY;
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();
        _floatingPlanks.ForEach(plank => plank.UpdateFloatingTransformation(Device, this));
    }
}