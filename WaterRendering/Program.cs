﻿// See https://aka.ms/new-console-template for more information

using SharpGfx;
using SharpGfx.OpenTK;
using WaterRendering;

var device = new OtkDevice();
var size = device.World.Vector2(1024, 768);

var camera = new OrbitCamera(device.World, device.World.Origin3(), 30);
var window = new OtkWindow("ComGra - Water Rendering", size, camera);
var rendering = new SceneRendering(device, size, camera);

window.Show(rendering);