using System.Numerics;

namespace AsciiRender;

public interface IRenderer<TSelf>
{
    public IDrawer Drawer { get; }
    public Matrix4x4 Transform { get; set; }
    public Func<TSelf, int, (Vector4, object?)> Vertex { get; set; }
    public Func<TSelf, int, object?, Vector4> Fragment { get; set; }
    public void Render(int count);
}