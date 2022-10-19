// See https://aka.ms/new-console-template for more information

using S2A2;
using SharpGfx;
using SharpGfx.OpenTK;

var device = new OtkDevice();
var size = device.World.Vector2(1024, 768);

var camera = new OrbitCamera(device.World, device.World.Origin3(), 5);
var window = new OtkWindow("S2A2", size, camera);
var rendering = new CylinderRendering(device, size, camera);;

window.Show(rendering);