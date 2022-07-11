using System.Numerics;
using Bny.Console;

namespace AsciiRender;

public class Program
{
    static void Main(string[] args)
    {
        float pixelRatio = 0.5f;

        Term.IsCursorVisible = false;

        (var pos, var norm) = PointGenerator.GenerateDonut(500, 500, new(), 0.5f, 0.2f);

        AnsiDrawer ansi = new();
        AsciiDrawer ascii = new();
        IDrawer drawer = ascii;
        PointRenderer renderer = new(drawer);

        Vector3 light = new(0, 1, 0);

        Matrix4x4 r = Matrix4x4.Identity;

        renderer.Vertex = (pr, i) => (Vector4.Transform(pos[i], pr.Transform), null);
        renderer.Fragment = (pr, i, _) =>
        {
            var v4 = Vector4.Transform(norm[i], r);
            Vector3 v = new(v4.X, v4.Y, v4.Z);
            float a = MathF.Acos(Vector3.Dot(light, v) / v.Length()) / MathF.PI;
            return new(a, a, a, 1);
        };

        var t = Matrix4x4.CreateTranslation(new(0, 0, -1.5f));
        var s = Matrix4x4.CreateScale(1, 1, 0.001f);
        var rot = Matrix4x4.Identity;
        var la = Matrix4x4.Identity;

        float rx = 0;
        float rpsx = 1;
        float ry = 0;
        float rpsy = 1.2777f;

        DateTime now = DateTime.Now;
        float speed = 0.05f;
        float rspeed = 3f;

        Vector3 camPos = new();
        Vector3 front = new(0, 0, -1);
        Vector3 up = new(0, 1, 0);
        Vector3 right = new(1, 0, 0);

        float pitch = 0;
        float yaw = -90;

        bool rotate = true;
        bool run = true;

        while (run)
        {
            DateTime newNow = DateTime.Now;
            var delta = newNow - now;
            var d = (float)delta.TotalSeconds;
            now = newNow;

            if (rotate)
            {
                rx += rpsx * d;
                ry += rpsy * d;
            }

            if (Console.KeyAvailable)
            {
                var cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.W:
                        camPos += front * speed;
                        break;
                    case ConsoleKey.S:
                        camPos -= front * speed;
                        break;
                    case ConsoleKey.A:
                        camPos -= Vector3.Normalize(Vector3.Cross(front, up)) * speed;
                        break;
                    case ConsoleKey.D:
                        camPos += Vector3.Normalize(Vector3.Cross(front, up)) * speed;
                        break;
                    case ConsoleKey.Q:
                        camPos += up * speed;
                        break;
                    case ConsoleKey.E:
                        camPos -= up * speed;
                        break;
                    case ConsoleKey.I:
                        pitch -= rspeed;
                        break;
                    case ConsoleKey.K:
                        pitch += rspeed;
                        break;
                    case ConsoleKey.J:
                        yaw -= rspeed;
                        break;
                    case ConsoleKey.L:
                        yaw += rspeed;
                        break;
                    case ConsoleKey.Enter:
                        rotate = !rotate;
                        break;
                    case ConsoleKey.Escape:
                        run = false;
                        break;
                    case ConsoleKey.Tab:
                        if (renderer.Drawer is AsciiDrawer)
                        {
                            renderer.Drawer = ansi;
                            drawer = ansi;
                        }
                        else
                        {
                            renderer.Drawer = ascii;
                            drawer = ascii;
                        }
                        Term.ResetColor();
                        break;
                }

                if (pitch > 89.0f)
                    pitch = 89.0f;
                else if (pitch < -89.0f)
                    pitch = -89.0f;

                front.X = MathF.Cos(ToRad(pitch)) * MathF.Cos(ToRad(yaw));
                front.Y = MathF.Sin(ToRad(pitch));
                front.Z = MathF.Cos(ToRad(pitch)) * MathF.Sin(ToRad(yaw));
                front = Vector3.Normalize(front);

                right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
                up = Vector3.Normalize(Vector3.Cross(right, front));

                la = Matrix4x4.CreateLookAt(camPos, camPos + front, up);
            }

            drawer.GetSize();
            var p = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 3f, drawer.Size.X * pixelRatio / drawer.Size.Y, 0.01f, 100f);
            r = Matrix4x4.CreateRotationX(rx);
            r *= Matrix4x4.CreateRotationY(ry);
            renderer.Transform = r * t * la * p;
            renderer.Render(pos.Length);
        }

        Term.IsCursorVisible = true;
        Term.ResetAll();
    }

    public static float ToRad(float deg) => deg * MathF.PI / 180;
}