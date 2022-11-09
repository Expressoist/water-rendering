using OpenTK.Mathematics;
using SharpGfx;
using SharpGfx.Host;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaveRendering : WaterRendering
{
    private readonly OtkRenderObject _floatingInstance;

    private const float WaveAmplitude = 1;
    private const float WaveLength = 50;
    private const float AngularWaveNumber = 2 * MathF.PI / WaveLength;
    private const float AngularFrequency = 0.02f;
    private const float WaveSteepness = AngularWaveNumber * AngularFrequency;

    public WaveRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, camera)
    {
        _floatingInstance = (OtkRenderObject) Plank.Create(device, 1f, 2f, 0.5f);
        Scene.Add(_floatingInstance);
    }

    protected override void AddVertexAtPosition(List<float> vertices, int x, int y)
    {
        float z = CalculateWaveHeight(x, y);
        vertices.Add(x);
        vertices.Add(y);
        vertices.Add(z);
    }

    private float CalculateWaveHeight(int x, int y)
    {
        float omega = AngularWaveNumber * x - AngularFrequency * Time;
        float z = WaveAmplitude * ((1 - 1 / 16f * WaveSteepness) * MathF.Cos(omega) +
                                   1 / 2f * WaveSteepness * MathF.Cos(2 * omega) +
                                   3 / 8f * MathF.Pow(WaveSteepness, 2) * MathF.Cos(3 * omega));

        float pseudoRandomX = 0.05f * MathF.Cos(x - 0.06f * Time) + 0.05f * MathF.Cos(x - 0.02f * Time);
        float pseudoRandomY = 0.05f * MathF.Cos(y - 0.05f * Time);
        return z + pseudoRandomX + pseudoRandomY;
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();

        float z1 = CalculateWaveHeight(0, 0);
        float z2 = CalculateWaveHeight(1, 0);

        float angle = MathF.Atan(z1 - z2);
        
        _floatingInstance.Transform = Device.World.RotationY4(angle) * 
                                      Device.World.Translation4(Device.World.Vector3(0, 0, z1));

    }
}