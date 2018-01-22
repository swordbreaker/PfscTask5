using System;
using System.Windows.Media.Imaging;
using OpenTK;

namespace PfscTask5
{
    public static class Figures
    {
        public static Mesh Cube(int hProgramm) => new Mesh(
            verts: new float[]
            {
                -1, -1, -1,
                +1, -1, -1,
                +1, +1, -1,
                -1, +1, -1,

                -1, -1, +1,
                +1, -1, +1,
                +1, +1, +1,
                -1, +1, +1,

                -1, -1, -1,
                -1, +1, -1,
                -1, +1, +1,
                -1, -1, +1,

                +1, -1, +1,
                +1, +1, +1,
                +1, +1, -1,
                +1, -1, -1,

                -1, +1, -1,
                +1, +1, -1,
                +1, +1, +1,
                -1, +1, +1,

                -1, -1, +1,
                +1, -1, +1,
                +1, -1, -1,
                -1, -1, -1,
            },
            idx: new int[]
            {
                0,   1,  2,
                0,   2,  3,
                7,   6,  5,
                7,   5,  4,

                8,   9, 10,
                8,  10, 11,
                12, 13, 14,
                14, 15, 12,

                16, 18, 19,
                16, 17, 18,
                20, 21, 22,
                22, 23, 20,
            },
            colors: new float[]
            {
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,

                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,

                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,

                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,

                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,

                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
                1,1,1,1,
            },
            normals: new float[]
            {
                0,0,-1,
                0,0,-1,
                0,0,-1,
                0,0,-1,
                //Top
                0,0,1,
                0,0,1,
                0,0,1,
                0,0,1,
                //Left
                -1,0,0,
                -1,0,0,
                -1,0,0,
                -1,0,0,
                //Right
                1,0,0,
                1,0,0,
                1,0,0,
                1,0,0,
                //Front
                0,1,0,
                0,1,0,
                0,1,0,
                0,1,0,
                //Back
                0,-1,0,
                0,-1,0,
                0,-1,0,
                0,-1,0,
            }, 
            uvs: new float[]
            {
                0,1,
                1,1,
                1,0,
                0,0,

                0,1,
                1,1,
                1,0,
                0,0,

                0,1,
                1,1,
                1,0,
                0,0,

                0,1,
                1,1,
                1,0,
                0,0,

                0,1,
                1,1,
                1,0,
                0,0,

                0,1,
                1,1,
                1,0,
                0,0,
            },
            hProgram: hProgramm
            );

        //public Mesh Circle(int hProgram, Vector2 center, float r)
        //{
        //    int nPkte = 20;
        //    double phi = 2 * Math.PI / (nPkte - 1);
        //    Vector2 p;

        //    var verts = new float[nPkte * 3 + 3];
        //    var normals = new float[nPkte * 3 + 3];
        //    var colors = new float[nPkte * 3 + 3];
        //    var uvs = new float[nPkte * 2 + 2];
        //    var idx = new int[nPkte/2 * 3];
        //    //verts[0] = center.X;
        //    //verts[1] = center.Y;
        //    //verts[2] = 0;
        //    verts[0] = center.X;
        //    verts[1] = center.Y;
        //    verts[2] = 0;

        //    normals[0] = 0;
        //    normals[1] = 0;
        //    normals[2] = 1;

        //    colors[0] = 1;
        //    colors[1] = 1;
        //    colors[2] = 1;

        //    var k = 3;
        //    for (int i = 0; i < nPkte; i++, k += 3)
        //    {
        //        p.X = center.X + (float)(r * Math.Cos(i * phi));
        //        p.Y = center.Y + (float) (r * Math.Sign(i * phi));
        //        verts[k + 0] = p.X;
        //        verts[k + 1] = p.Y;
        //        verts[k + 2] = 0;

        //        normals[k + 0] = 0;
        //        normals[k + 1] = 0;
        //        normals[k + 2] = 1;

        //        colors[k + 0] = 1;
        //        colors[k + 1] = 1;
        //        colors[k + 2] = 1;

        //        if (i % 2 == 0)
        //        {
        //            idx[k + 0] = 0;
        //            idx[k + 1] = i - 1;
        //            idx[k + 2] = i;
        //        }
        //    }

        //    return new Mesh(verts, idx, colors, normals, uvs, hProgram);
        //}
    }
}