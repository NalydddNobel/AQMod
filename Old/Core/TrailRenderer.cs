﻿using Aequu2.Core.Assets;
using System;
using System.Collections.Generic;

namespace Aequu2.Old.Core;

public class TrailRenderer : IPrimRenderer {
    public static Effect Shader => Aequu2Shaders.LegacyVertexShader;

    public static Vector2 CurrentlyDrawnLocation;

    public Vector2 drawOffset;
    public Texture2D Texture;
    public string Pass;
    public Func<float, Vector2> GetWidth;
    public Func<float, Color> GetColor;
    public bool ObeyReversedGravity;
    public bool WorldTrail;

    protected List<VertexPositionColorTexture> vertices;

    public TrailRenderer() { }

    public TrailRenderer(Texture2D texture, string pass, Func<float, Vector2> getWidth, Func<float, Color> getColor, bool obeyReversedGravity = true, bool worldTrail = true, Vector2 drawOffset = default(Vector2)) {
        Texture = texture;
        Pass = pass;
        GetWidth = getWidth;
        GetColor = getColor;
        ObeyReversedGravity = obeyReversedGravity;
        WorldTrail = worldTrail;
        this.drawOffset = drawOffset;
        vertices = null;
    }

    public static TrailRenderer NewRenderer(Texture2D texture, Func<float> width, Func<Color> color) {
        return new TrailRenderer(texture, "Texture", (p) => new Vector2(width() - width() * p), (p) => color() * (1f - p));
    }
    public static TrailRenderer NewRenderer(Texture2D texture, float width, Func<Color> color) {
        return NewRenderer(texture, () => width, color);
    }
    public static TrailRenderer NewRenderer(Texture2D texture, Func<float> width, Color color) {
        return NewRenderer(texture, width, () => color);
    }
    public static TrailRenderer NewRenderer(Texture2D texture, float width, Color color) {
        return NewRenderer(texture, () => width, color);
    }

    public static TrailRenderer NewRenderer(Projectile projectile, Texture2D texture, float width, Color color) {
        var prim = NewRenderer(texture, width, () => color * projectile.Opacity);
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
        var rotationVectors = new Vector2[arr.Length];
        if (rotationArr == null) {
            for (int i = 0; i < arr.Length; i++) {
                rotationVectors[i] = getRotationVector(arr, i);
            }
        }
        else {
            for (int i = 0; i < arr.Length; i++) {
                rotationVectors[i] = rotationArr[i].ToRotationVector2();
            }
        }

        vertices = new List<VertexPositionColorTexture>();
        for (int i = 0; i < arr.Length - 1; i++) {
            float uv = i / (float)arr.Length;
            float uv2 = (i + 1) / (float)arr.Length;
            Vector2 width = GetWidth(uv);
            Vector2 width2 = GetWidth(uv2);
            Vector2 pos1 = arr[i];
            Vector2 pos2 = arr[i + 1];
            Vector2 off1 = rotationVectors[i] * width;
            Vector2 off2 = rotationVectors[i + 1] * width2;
            float coord1 = off1.Length() < off2.Length() ? 1 : 0;
            float coord2 = off1.Length() < off2.Length() ? 0 : 1;
            CurrentlyDrawnLocation = (arr[i] + arr[i + 1]) / 2f;
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
}