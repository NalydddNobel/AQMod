using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Effects.Prims
{
    public class PrimRenderer : IPrimRenderer, IAutoloadType
    {
        public const string DefaultPass = "Texture";

        public static bool renderProjTrails;

        protected readonly Texture2D Texture;
        protected readonly string Pass;
        protected readonly Func<float, Vector2> GetWidth;
        protected readonly Func<float, Color> GetColor;
        protected readonly bool ObeyReversedGravity;
        protected readonly bool WorldTrail;

        protected List<VertexPositionColorTexture> vertices;

        public PrimRenderer()
        {

        }

        public PrimRenderer(Texture2D texture, string pass, Func<float, Vector2> getWidth, Func<float, Color> getColor, bool obeyReversedGravity = true, bool worldTrail = true)
        {
            Texture = texture;
            Pass = pass;
            GetWidth = getWidth;
            GetColor = getColor;
            ObeyReversedGravity = obeyReversedGravity;
            WorldTrail = worldTrail;
            vertices = null;
        }

        public static Vector2[] RemoveZerosAndDoOffset(Vector2[] arr, Vector2 offset)
        {
            var valid = new List<Vector2>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == new Vector2(0f, 0f))
                    break;
                if (i != 0 && arr[i - 1] == arr[i])
                    continue;
                valid.Add(arr[i] + offset);
            }
            return valid.ToArray();
        }
        protected virtual bool InternalPrepare(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f)
        {
            if (WorldTrail)
            {
                arr = RemoveZerosAndDoOffset(arr, -Main.screenPosition);
                if (arr.Length <= 1)
                {
                    return false;
                }
            }
            if (ObeyReversedGravity && Main.player[Main.myPlayer].gravDir == -1)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = new Vector2(arr[i].X, -arr[i].Y + Main.screenHeight);
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
                Vector2 off1 = getRotationVector(arr, i) * width;
                Vector2 off2 = getRotationVector(arr, i + 1) * width;
                float coord1 = off1.Length() < off2.Length() ? 1 : 0;
                float coord2 = off1.Length() < off2.Length() ? 0 : 1;
                Color col1 = GetColor(uv);
                Color col2 = GetColor(uv2);
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier % 1, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier % 1, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier % 1, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier % 1, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier % 1, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier % 1, coord2)));
            }
            return true;
        }

        private Vector2 getRotationVector(Vector2[] oldPos, int index)
        {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }

        public void Draw(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f)
        {
            if (!InternalPrepare(arr, uvAdd, uvMultiplier))
            {
                return;
            }
            var effect = LegacyEffectCache.Trailshader;
            effect.Parameters["WVP"].SetValue(AQGraphics.WorldViewPoint);
            effect.Parameters["imageTexture"].SetValue(Texture);
            effect.Parameters["strength"].SetValue(1f);
            effect.CurrentTechnique.Passes[Pass].Apply();
            Main.graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
        }

        public void Clear()
        {
            vertices.Clear();
        }

        void IAutoloadType.Load()
        {
            renderProjTrails = ModLoader.GetMod("ShaderLib") == null;
        }

        void IAutoloadType.Unload()
        {
        }
    }
}