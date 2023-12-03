using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Core.Graphics.Models;

public class CircleCrystalGenMesh : Icosphere {
    private int _seed;

    public CircleCrystalGenMesh(int seed, int subDivideCount) : base(subDivideCount) {
        _seed = seed;
    }

    protected override void GenerateInner() {
        base.GenerateInner();
        var newVerts = new List<VertexPositionColor>();
        for (short i = 0; i < Indices.Count; i += 3) {
            var faceColor = new Color(0, (i * 50 % 253) + 1, 0, 255);
            newVerts.Add(new VertexPositionColor(Vertices[Indices[i]].Position, faceColor));
            newVerts.Add(new VertexPositionColor(Vertices[Indices[i + 1]].Position, faceColor));
            newVerts.Add(new VertexPositionColor(Vertices[Indices[i + 2]].Position, faceColor));

            Indices[i] = i;
            Indices[i + 1] = (short)(i + 1);
            Indices[i + 2] = (short)(i + 2);
        }
        Vertices.Clear();
        Vertices.AddRange(newVerts);
    }

    protected override void DrawInner(VertexPositionColor[] vertices, short[] indices) {
        //var fastRandom = new FastRandom(2);
        //float x = (Main.mouseX - Main.screenWidth / 2f);
        //float y = (Main.mouseY - Main.screenHeight / 2f);
        for (int i = 0; i < vertices.Length; ++i) {
            vertices[i].Position = Vector3.Transform(vertices[i].Position, Matrix.CreateFromYawPitchRoll(Main.GlobalTimeWrappedHourly, Main.GlobalTimeWrappedHourly * 0.6f, 0f));
            //vertices[i].Position += new Vector3(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * 0.1f;
            //var normal = Vector3.Normalize(vertices[i].Position);
            //if (normal.Y > 0f) {

            //}
            //else {
            //    vertices[i].Color.R = (byte)Math.Clamp((1f + normal.Y) * byte.MaxValue, 0f, byte.MaxValue);
            //}
            //vertices[i].Color.R = (byte)Math.Min(vertices[i].Color.R + 1, 254);
        }
        for (int i = 0; i < indices.Length; i += 3) {
            var middle = Vector3.Normalize((vertices[indices[i]].Position + vertices[indices[i + 1]].Position + vertices[indices[i + 2]].Position) / 3f);
            byte r = (byte)Math.Clamp((middle.Y * 0.5f + 0.5f) * byte.MaxValue, 1f, byte.MaxValue - 1f);
            vertices[i].Color.R = r;
            vertices[i+1].Color.R = r;
            vertices[i+2].Color.R = r;
        }
        base.DrawInner(vertices, indices);
    }
}