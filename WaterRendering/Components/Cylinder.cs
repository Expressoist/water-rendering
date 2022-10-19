using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using SharpGfx;
using SharpGfx.Primitives;

namespace S2A2
{
    struct Vertex
    {
        public float X;
        public float Y;
        public float Z;

        public float[] ToArray()
        {
            return new[] { X, Y, Z };
        }
    }

    struct Circle
    {
        public static Vertex Center()
        {
            return new Vertex { X = 0, Y = 0, Z = 0 };
        }
        
        public static Vertex TriangleBottom(int offset, float deltaAngle)
        {
            return new Vertex
            {
                X = MathF.Cos(offset * deltaAngle),
                Y = 0,
                Z = MathF.Sin(offset * deltaAngle)
            };
        }
        
        public static Vertex TriangleTop(int offset, float deltaAngle)
        {
            return new Vertex
            {
                X = MathF.Cos((offset + 1) * deltaAngle),
                Y = 0,
                Z = MathF.Sin((offset + 1) * deltaAngle)
            };
        }
    }

    public static class Cylinder
    {
        public static float[] GetVertices(Space space, float length, float radius, int faceCount)
        {
            var verticies = new List<Vertex>();

            var circle1 = GenerateCircle(faceCount);
            var circle2 = GenerateCircle(faceCount).Select(vertex => vertex with { Y = vertex.Y + length }).ToList();

            for (var i = 0; i < faceCount*3; i += 3)
            {
                verticies.Add(new Vertex
                {
                    X = circle1[i+1].X,
                    Y = circle1[i+1].Y,
                    Z = circle1[i+1].Z,
                });
                verticies.Add(new Vertex
                {
                    X = circle1[i+2].X,
                    Y = circle1[i+2].Y,
                    Z = circle1[i+2].Z,
                });
                verticies.Add(new Vertex
                {
                    X = circle2[i+1].X,
                    Y = circle2[i+1].Y,
                    Z = circle2[i+1].Z,
                });
                // Second Face
                verticies.Add(new Vertex
                {
                    X = circle2[i+1].X,
                    Y = circle2[i+1].Y,
                    Z = circle2[i+1].Z,
                });
                verticies.Add(new Vertex
                {
                    X = circle2[i+2].X,
                    Y = circle2[i+2].Y,
                    Z = circle2[i+2].Z,
                });
                verticies.Add(new Vertex
                {
                    X = circle1[i+2].X,
                    Y = circle1[i+2].Y,
                    Z = circle1[i+2].Z,
                });
            }
            
            verticies.AddRange(circle1);
            verticies.AddRange(circle2);

            return verticies.SelectMany(vertex => vertex.ToArray()).ToArray();
        }

        private static List<Vertex> GenerateCircle(int faceCount)
        {
            var vertices = new List<Vertex>();
            float deltaAngle = 2 * MathF.PI / faceCount;
            for (var faceIndex = 0; faceIndex < faceCount; faceIndex++)
            {
                vertices.Add(Circle.Center());
                vertices.Add(Circle.TriangleBottom(faceIndex, deltaAngle));
                vertices.Add(Circle.TriangleTop(faceIndex, deltaAngle));
            }
            return vertices;
        }

        public static ushort[] GetIndices(int faceCount)
        {
            return null;
        }
    }
}