using Bny.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsciiRender;

public class AnsiDrawer : IDrawer
{
    public (int X, int Y) Size { get; private set; }

    private StringBuilder Sb { get; } = new();

    public AnsiDrawer() { }

    public StringBuilder GetShade(Vector4 color) => Term.PrepareSB(Term.fg, (byte)(color.X * 255), (byte)(color.Y * 255), (byte)(color.Z * 255), "█");

    public (int X, int Y) GetSize() => Size = Term.GetWindowSize();

    public void Draw(ReadOnlySpan<Vector4> pixels)
    {
        Sb.Clear();

        for (int i = 0; i < pixels.Length; i++)
            Sb.Append(GetShade(pixels[i]));

        Term.Move(0, 0);
        Console.Write(Sb.ToString());
    }
}
