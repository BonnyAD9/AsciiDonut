using System.Numerics;

namespace AsciiRender;

public interface IDrawer
{
    public (int X, int Y) Size { get; }

    public void Draw(ReadOnlySpan<Vector4> pixels);
    public (int X, int Y) GetSize();
}
