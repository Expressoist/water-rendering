// See https://aka.ms/new-console-template for more information

using OpenTK.Graphics.OpenGL;
using SharpGfx;
using SharpGfx.OpenGL.Shading;
using SharpGfx.OpenTK;
using WaterRendering.Components;

var device = new OtkDevice();
var size = device.World.Vector2(1024, 768);

var camera = new OrbitCamera(device.World, device.World.Origin3(), 30);
var window = new OtkWindow("S2A2", size, camera);
var rendering = new RippleRendering(device, size, camera);

window.Show(rendering);