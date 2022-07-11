using Bny.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsciiRender;

public class AsciiDrawer : IDrawer
{
    public string Shades { get; set; } = " .:-=+*#%@";//" .'`^\",:;Il!i><~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";

    public (int X, int Y) Size { get; private set; }

    protected char[] PixelBuffer { get; set; } = Array.Empty<char>();

    public AsciiDrawer() { }

    public char GetShade(Vector4 color) => Shades[(int)((color.X + color.Y + color.Z) * (Shades.Length - 1) / 3 + 0.5)];

    public (int X, int Y) GetSize() => Size = Term.GetWindowSize();

    protected void FitBuffer()
    {
        if (Size.X * Size.Y > PixelBuffer.Length)
            PixelBuffer = new char[Size.X * Size.Y];
    }

    public void Draw(ReadOnlySpan<Vector4> pixels)
    {
        FitBuffer();

        for (int i = 0; i < pixels.Length; i++)
            PixelBuffer[i] = GetShade(pixels[i]);

        Term.Move(0, 0);
        Console.Write(PixelBuffer.AsSpan(..pixels.Length).ToString());
    }
}
