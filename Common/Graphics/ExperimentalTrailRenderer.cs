using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics {
    public class ExperimentalTrailRenderer : IPrimRenderer, ILoadable {
        public const string DefaultPass = "Texture";

        protected static Asset<Effect> shader;
        public static Effect Shader => shader.Value;

        public static Vector2 CurrentlyDrawnLocation;

        public Vector2 drawOffset;
        public Texture2D Texture;
        public string Pass;
        public Func<float, Vector2> GetWidth;
        public Func<float, Color> GetColor;
        public bool ObeyReversedGravity;
        public bool WorldTrail;

        protected List<VertexPositionColorTexture> vertices;

        public ExperimentalTrailRenderer() {
        }

        public ExperimentalTrailRenderer(Texture2D texture, string pass, Func<float, Vector2> getWidth, Func<float, Color> getColor, bool obeyReversedGravity = true, bool worldTrail = true, Vector2 drawOffset = default(Vector2)) {
            Texture = texture;
            Pass = pass;
            GetWidth = getWidth;
            GetColor = getColor;
            ObeyReversedGravity = obeyReversedGravity;
            WorldTrail = worldTrail;
            this.drawOffset = drawOffset;
            vertices = null;
        }

        public static TrailRenderer NewRenderer(int type, Func<float> width, Func<Color> color) {
            return new TrailRenderer(TrailTextures.Trail[type].Value, DefaultPass, (p) => new Vector2(width() - width() * p), (p) => color() * (1f - p));
        }
        public static TrailRenderer NewRenderer(int type, float width, Func<Color> color) {
            return NewRenderer(type, () => width, color);
        }
        public static TrailRenderer NewRenderer(int type, Func<float> width, Color color) {
            return NewRenderer(type, width, () => color);
        }
        public static TrailRenderer NewRenderer(int type, float width, Color color) {
            return NewRenderer(type, () => width, color);
        }

        public static TrailRenderer NewRenderer(Projectile projectile, int type, float width, Color color) {
            var prim = NewRenderer(type, width, () => color * projectile.Opacity);
            prim.drawOffset = projectile.Size / 2f;
            return prim;
        }

        public static Vector2[] RemoveZerosAndDoOffset(Vector2[] arr, Vector2 offset) {
            var valid = new List<Vector2>();
            for (int i = 0; i < arr.Length; i++) {
                if (arr[i] == Vector2.Zero || arr[i].HasNaNs())
                    break;
                if (i != 0) {
                    if (arr[i - 1] == arr[i])
                        continue;

                    var d = arr[i - 1] - arr[i];
                    if (d.X < -1000f || d.X > 1000f || d.Y < -1000f || d.Y > 1000f) {
                        //Main.NewText(d + " = " + arr[i - 1] + " - " + arr[i]);
                        continue;
                    }
                }
                valid.Add(arr[i] + offset);
            }
            return valid.ToArray();
        }

        protected virtual bool InternalPrepare(Vector2[] arr, float[] rotationArr, float uvAdd = 0f, float uvMultiplier = 1f) {
            if (WorldTrail) {
                arr = RemoveZerosAndDoOffset(arr, -Main.screenPosition + drawOffset);
                if (arr.Length <= 1) {
                    return false;
                }
            }
            else if (drawOffset != Vector2.Zero) {
                var arr2 = new Vector2[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    arr2[i] = arr[i] + drawOffset;
                }
                arr = arr2;
            }
            if (ObeyReversedGravity && Main.player[Main.myPlayer].gravDir == -1) {
                for (int i = 0; i < arr.Length; i++) {
                    arr[i] = new Vector2(arr[i].X, -arr[i].Y + Main.screenHeight);
                }
            }

            //Vector2[] arrSmooth = new Vector2[arr.Length];
            //arrSmooth[0] = arr[0];
            //arrSmooth[^1] = arr[^1];
            //for (int i = 1; i < arr.Length - 1; i++) {
            //    arrSmooth[i] = (arr[i - 1] + arr[i]) / 2f;
            //}
            //arr = arrSmooth;

            vertices = new();
            float uv = 0f;
            Vector2 position = arr[0];
            Vector2 normal = (rotationArr?[0])?.ToRotationVector2() ?? Vector2.Normalize(arr[1] - position);
            Vector2 width = GetWidth(uv);
            Color color = GetColor(uv);
            for (int i = 1; i < arr.Length; i++) {
                float nextUV = i / (float)arr.Length;
                Vector2 nextWidth = GetWidth(nextUV);
                Vector2 nextPosition = arr[i];
                var nextNormal = (rotationArr?[i])?.ToRotationVector2() ?? (i >= arr.Length - 2 ? normal : Vector2.Normalize(arr[i + 1] - nextPosition));
                //var nextNextNormal = i >= arr.Length - 3 ? nextNormal : Vector2.Normalize(arr[i + 2] - arr[i + 1]);
                //nextNormal = Vector2.Normalize((nextNormal + nextNextNormal) / 2f);
                //CurrentlyDrawnLocation = (arr[i] + arr[i + 1]) / 2f;
                Color nextColor = GetColor(nextUV);
                vertices.Add(new(
                    new Vector3(position + normal.RotatedBy(MathHelper.PiOver2) * width.X, 0f), color, new Vector2((uv + uvAdd) * uvMultiplier, 1f)));
                vertices.Add(new(
                    new Vector3(position + normal.RotatedBy(-MathHelper.PiOver2) * width.Y, 0f), color, new Vector2((uv + uvAdd) * uvMultiplier, 0f)));
                vertices.Add(new(
                    new Vector3(nextPosition + nextNormal.RotatedBy(MathHelper.PiOver2) * nextWidth.X, 0f), nextColor, new Vector2((nextUV + uvAdd) * uvMultiplier, 1f)));

                vertices.Add(new(
                    new Vector3(nextPosition + nextNormal.RotatedBy(MathHelper.PiOver2) * nextWidth.X, 0f), nextColor, new Vector2((nextUV + uvAdd) * uvMultiplier, 1f)));
                vertices.Add(new(
                    new Vector3(nextPosition + nextNormal.RotatedBy(-MathHelper.PiOver2) * nextWidth.Y, 0f), nextColor, new Vector2((nextUV + uvAdd) * uvMultiplier, 0f)));
                vertices.Add(new(
                    new Vector3(position + normal.RotatedBy(-MathHelper.PiOver2) * width.X, 0f), color, new Vector2((uv + uvAdd) * uvMultiplier, 0f)));

                normal = nextNormal;
                uv = nextUV;
                position = nextPosition;
                width = nextWidth;
                color = nextColor;
            }
            return true;
        }

        protected Vector2 getRotationVector(Vector2[] oldPos, int index) {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }

        public void Draw(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f) {
            Draw(arr, null, uvAdd, uvMultiplier);
        }
        public void Draw(Vector2[] arr, float[] rotation, float uvAdd = 0f, float uvMultiplier = 1f) {
            if (!InternalPrepare(arr, rotation, uvAdd, uvMultiplier)) {
                return;
            }
            //#if DEBUG
            //            for (int i = 0; i < vertices.Count; i++) {
            //                Main.spriteBatch.Draw(
            //                    AequusTextures.Pixel,
            //                    new Vector2(vertices[i].Position.X, vertices[i].Position.Y),
            //                    Color.Red.HueSet(i / (float)vertices.Count));
            //            }
            //            for (int i = 0; i < 6; i++) {
            //                Main.spriteBatch.Draw(
            //                    AequusTextures.Pixel,
            //                    new Vector2(vertices[i].Position.X, vertices[i].Position.Y),
            //                    new Color(255, 100, 100, 255).HueSet(i / 6f));
            //            }
            //            for (int i = 0; i < arr.Length; i++) {
            //                Main.spriteBatch.Draw(
            //                    AequusTextures.Pixel,
            //                    arr[i] - Main.screenPosition,
            //                    Color.Red.HueSet(i / (float)arr.Length));
            //            }
            //#endif
            var effect = Shader;
            effect.Parameters["WVP"].SetValue(IPrimRenderer.WorldViewPoint);
            effect.Parameters["imageTexture"].SetValue(Texture);
            effect.Parameters["strength"].SetValue(1f);
            effect.CurrentTechnique.Passes[Pass].Apply();
            Main.graphics.GraphicsDevice.RasterizerState = new RasterizerState {
                CullMode = CullMode.None
            };
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
        }

        public void Clear() {
            vertices.Clear();
        }

        void ILoadable.Load(Mod mod) {
            if (!Main.dedServ) {
                shader = ModContent.Request<Effect>("Aequus/Assets/Effects/Prims/Trailshader");
            }
        }

        void ILoadable.Unload() {
            shader = null;
        }
    }
}