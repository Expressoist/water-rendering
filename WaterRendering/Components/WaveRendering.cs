using System.Collections;
using System.Collections.Concurrent;
using OpenTK.Mathematics;
using SharpGfx;
using SharpGfx.Host;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using SharpGfx.Primitives;

namespace WaterRendering.Components;

public class WaveRendering : WaterRendering
{
    private readonly List<Box> _floatingPlanks;

    private const float WaveAmplitude = 1;
    private const float WaveLength = 50;
    private const float AngularWaveNumber = 2 * MathF.PI / WaveLength;
    private const float AngularFrequency = 0.02f;
    private const float WaveSteepness = AngularWaveNumber * AngularFrequency;

    public WaveRendering(Device device, IVector2 size, Camera camera)
        : base(device, size, camera)
    {
        _floatingPlanks = new List<Box>
        {
            new(device, 2f, device.World.Vector2(2, 2), AmbientColor, LightPosition),
            new(device, 1f, device.World.Vector2(-1, -1), AmbientColor, LightPosition)
        };
        
        _floatingPlanks.ForEach(plank => Scene.Insert(1, plank.RenderObject)); // Really ugly fix
    }

    protected override void AddVertexAtPosition(float[] vertices, int x, int y, int index)
    {
        float z = CalculateWaveHeight(x, y);
        z += CalculatePseudoRandomFactor(x, y);
        
        vertices[index] = x;
        vertices[index + 1] = z;
        vertices[index + 2] = y;
    }

    public float CalculateWaveHeight(float x, float y)
    {
        float omega = AngularWaveNumber * x - AngularFrequency * Time;
        return WaveAmplitude * ((1 - 1 / 16f * WaveSteepness) * MathF.Cos(omega) +
                                   1 / 2f * WaveSteepness * MathF.Cos(2 * omega) +
                                   3 / 8f * MathF.Pow(WaveSteepness, 2) * MathF.Cos(3 * omega));
    }

    private float CalculatePseudoRandomFactor(float x, float y)
    {
        float pseudoRandomX = 0.08f * MathF.Cos(0.5f * x - 0.06f * Time)
                              + 0.06f * MathF.Cos(2f * x - 0.04f * Time);

        float pseudoRandomY = 0.07f * MathF.Cos(0.4f * y - 0.07f * Time)
                        + 0.05f * MathF.Cos(1.8f * y - 0.04f * Time);
        
        return pseudoRandomX + pseudoRandomY;
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();
        _floatingPlanks.ForEach(plank => plank.UpdateFloatingTransformation(Device, this));
    }
}