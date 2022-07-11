using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsciiRender;

public class PointRenderer : IRenderer<PointRenderer>
{
    public IDrawer Drawer { get; set; }

    public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

    public Func<PointRenderer, int, (Vector4, object?)> Vertex { get; set; } = (_, _) => new();
    public Func<PointRenderer, int, object?, Vector4> Fragment { get; set; } = (_, _, _) => new();

    protected Vector4[] Buffer { get; set; } = Array.Empty<Vector4>();

    protected int BufferLen => Size.X * Size.Y;

    protected Vector4 this[float x, float y]
    {
        get => Buffer[GetBufferCoordinate(x, y)];
        set => Buffer[GetBufferCoordinate(x, y)] = value;
    }

    protected (int X, int Y) Size => Drawer.Size;

    public PointRenderer(IDrawer drawer)
    {
        Drawer = drawer;
    }

    //protected int GetBufferCoordinate(float x, float y) => (int)(((y + 1) * Size.Y * Size.X + (x + 1) * Size.X) / 2);
    protected int GetBufferCoordinate(float x, float y) => (int)((y + 1) / 2 * (Size.Y - 0.001)) * Size.X + (int)((x + 1) / 2 * (Size.X - 0.001));

    protected void FitBuffer()
    {
        if (Buffer.Length < BufferLen)
            Buffer = new Vector4[BufferLen];
    }

    public void Render(int count)
    {
        FitBuffer();
        Array.Fill(Buffer, new(0, 0, 0, 1), 0, BufferLen);

        for (int i = 0; i < count; i++)
        {
            (var v, var o) = Vertex(this, i);
            v /= v.W;
            if (v.Z > 1 || v.Z < 0 || v.X < -1 || v.X > 1 || v.Y < -1 || v.Y > 1 || this[v.X, v.Y].W < v.Z)
                continue;
            var f = Fragment(this, i, o);
            this[v.X, v.Y] = new(f.X, f.Y, f.Z, v.Z);
        }

        Drawer.Draw(Buffer.AsSpan(..BufferLen));
    }
}
