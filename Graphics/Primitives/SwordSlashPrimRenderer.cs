using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Graphics.Primitives
{
    public sealed class SwordSlashPrimRenderer : TrailRenderer
    {
        public float coord1;
        public float coord2;

        public SwordSlashPrimRenderer(Texture2D texture, string pass, Func<float, Vector2> getWidth, Func<float, Color> getColor, bool obeyReversedGravity = true, bool worldTrail = true, Vector2 drawOffset = default(Vector2)) : base(texture, pass, getWidth, getColor, obeyReversedGravity, worldTrail, drawOffset)
        {
        }

        protected override bool InternalPrepare(Vector2[] arr, float[] rotationArr, float uvAdd = 0f, float uvMultiplier = 1f)
        {
            if (WorldTrail)
            {
                arr = RemoveZerosAndDoOffset(arr, -Main.screenPosition + drawOffset);
                if (arr.Length <= 1)
                {
                    return false;
                }
            }
            else if (drawOffset != Vector2.Zero)
            {
                var arr2 = new Vector2[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr2[i] = arr[i] + drawOffset;
                }
                arr = arr2;
            }
            if (ObeyReversedGravity && Main.player[Main.myPlayer].gravDir == -1)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = new Vector2(arr[i].X, -arr[i].Y + Main.screenHeight);
                }
            }
            var rotationVectors = new Vector2[arr.Length];
            if (rotationArr == null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    rotationVectors[i] = getRotationVector(arr, i);
                }
            }
            else
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    rotationVectors[i] = rotationArr[i].ToRotationVector2();
                }
            }
            vertices = new List<VertexPositionColorTexture>();
            for (int i = 0; i < arr.Length - 1; i++)
            {
                float uv = i / (float)arr.Length;
                float uv2 = (i + 1) / (float)arr.Length;
                Vector2 width = GetWidth(uv);
                Vector2 width2 = GetWidth(uv2);
                Vector2 pos1 = arr[i];
                Vector2 pos2 = arr[i + 1];

                Vector2 off1 = rotationVectors[i] * width;
                Vector2 off2 = rotationVectors[i + 1] * width2;

                Color col1 = GetColor(uv);
                Color col2 = GetColor(uv2);
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
            }
            return true;
        }
    }
}