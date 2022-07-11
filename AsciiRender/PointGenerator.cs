using System.Numerics;

namespace AsciiRender;

public static class PointGenerator
{
    public static (Vector4[] Points, Vector4[] Normals) GenerateDonut(int cc, int pc, Vector3 origin, float dr, float tr)
    {
        Vector4[] points = new Vector4[cc * pc];
        Vector4[] normals = new Vector4[cc * pc];

        for (int i = 0; i < cc; i++)
        {
            for (int j = 0; j < pc; j++)
            {
                var rotX = Matrix4x4.CreateRotationX((float)(Math.PI * 2 * j / pc));
                var rotZ = Matrix4x4.CreateRotationZ((float)(Math.PI * 2 * i / cc));

                normals[i * pc + j] = Vector4.Transform(new Vector4(0, 1, 0, 1), rotX * rotZ);

                var vec = Vector4.Transform(new Vector4(0, dr, 0, 1), rotZ);
                points[i * pc + j] = Vector4.Transform(Vector4.Transform(new Vector4(0, tr, 0, 1), rotX * rotZ), Matrix4x4.CreateTranslation(vec.X, vec.Y, vec.Z) * Matrix4x4.CreateTranslation(origin));
            }
        }

        return (points, normals);
    }
}
