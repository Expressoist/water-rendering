using SharpGfx;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaveRendering : WaterRendering
{

    private const float WaveAmplitude = 1;
    private const float WaveLength = 50;
    private const float AngularWaveNumber = 2 * MathF.PI / WaveLength;
    private const float AngularFrequency = 0.05f;
    private const float WaveSteepness = AngularWaveNumber * AngularFrequency;

    public WaveRendering(Device device, IVector2 size, Color3 ambientColor, Camera camera)
        : base(device, size, ambientColor, camera)
    {
    }

    protected override void AddVertexAtPosition(List<float> vertices, int x, int y)
    {
        float omega = AngularWaveNumber * x - AngularFrequency * Time;
        float z = WaveAmplitude * ((1 - 1 / 16f * WaveSteepness) * MathF.Cos(omega) +
                                   1 / 2f * WaveSteepness * MathF.Cos(2 * omega) +
                                   3 / 8f * MathF.Pow(WaveSteepness, 2) * MathF.Cos(3 * omega));
        
        vertices.Add(x);
        vertices.Add(y);
        vertices.Add(z);
    }
}