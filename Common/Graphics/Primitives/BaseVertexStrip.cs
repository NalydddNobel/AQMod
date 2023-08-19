using Aequus.Common.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Common.Graphics.Primitives;
public abstract class BaseVertexStrip {
    public const string DefaultPass = "Texture";

    public Effect Shader => AequusShaders.Trailshader.Value;

    public static Vector2 CurrentlyDrawnLocation;

    public Vector2 drawOffset;
    public Texture2D Texture;
    public string Pass;
    public Func<float, Vector2> GetWidth;
    public Func<float, Color> GetColor;
    public bool ObeyReversedGravity;
    public bool WorldTrail;
    public float uvAdd;
    public float uvMultiplier;

    private RasterizerState rasterizerState = new() { CullMode = CullMode.None, };

    protected List<Vector2> rotationVectors = new();

    protected List<VertexPositionColorTexture> vertices = new();

    public void Prepare(Texture2D texture, Func<float, Vector2> getWidth, Func<float, Color> getColor, string pass = DefaultPass, bool flipInReversedGravity = true, bool subtractScreenPosition = true, Vector2 drawingOffset = default, float uvAdditive = 0f, float uvMult = 1f) {
        Texture = texture;
        GetWidth = getWidth;
        GetColor = getColor;
        Pass = pass;
        ObeyReversedGravity = flipInReversedGravity;
        WorldTrail = subtractScreenPosition;
        drawOffset = drawingOffset;
        uvAdd = uvAdditive;
        uvMultiplier = uvMult;
    }

    protected virtual void GetHorizontalCoordinates(ref float UVFront, ref float UVBack) {
        UVFront = (UVFront + uvAdd) * uvMultiplier;
        UVBack = (UVBack + uvAdd) * uvMultiplier;
    }

    protected virtual void GetVerticalCoordinates(Vector2 frontWidth, Vector2 backWidth, ref float UVFront, ref float UVBack) {
        bool value = frontWidth.Length() < backWidth.Length();
        UVFront = value ? 1f : 0f;
        UVBack = value ? 0f : 1f;
    }

    protected virtual bool InternalPrepare(Vector2[] arr, float[] rotationArr) {
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

        PrepareRotationVectorCache(arr, rotationArr);
        vertices.Clear();
        for (int i = 0; i < arr.Length - 1; i++) {
            float uv = i / (float)arr.Length;
            float uv2 = (i + 1) / (float)arr.Length;
            Vector2 width = GetWidth(uv);
            Vector2 width2 = GetWidth(uv2);
            Vector2 pos1 = arr[i];
            Vector2 pos2 = arr[i + 1];
            Vector2 off1 = rotationVectors[i] * width;
            Vector2 off2 = rotationVectors[i + 1] * width2;
            CurrentlyDrawnLocation = (arr[i] + arr[i + 1]) / 2f;
            Color col1 = GetColor(uv);
            Color col2 = GetColor(uv2);
            float coord1 = 0f;
            float coord2 = 1f;
            GetVerticalCoordinates(off1, off2, ref coord1, ref coord2);
            GetHorizontalCoordinates(ref uv2, ref uv);
            vertices.Add(new(new Vector3(pos1 + off1, 0f), col1, new Vector2(uv, coord1)));
            vertices.Add(new(new Vector3(pos1 - off1, 0f), col1, new Vector2(uv, coord2)));
            vertices.Add(new(new Vector3(pos2 + off2, 0f), col2, new Vector2(uv2, coord1)));
            vertices.Add(new(new Vector3(pos2 + off2, 0f), col2, new Vector2(uv2, coord1)));
            vertices.Add(new(new Vector3(pos2 - off2, 0f), col2, new Vector2(uv2, coord2)));
            vertices.Add(new(new Vector3(pos1 - off1, 0f), col1, new Vector2(uv, coord2)));
        }
        return true;
    }

    public void Draw(Vector2[] arr, float[] rotation) {
        if (!InternalPrepare(arr, rotation)) {
            return;
        }
        var effect = Shader;
        effect.Parameters["WVP"].SetValue(AequusDrawing.WorldViewPointMatrix);
        effect.Parameters["imageTexture"].SetValue(Texture);
        effect.Parameters["strength"].SetValue(1f);
        effect.CurrentTechnique.Passes[Pass].Apply();
        Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
    }

    public void Draw(Vector2[] arr) {
        Draw(arr, null);
    }

    public void Clear() {
        vertices.Clear();
    }

    protected void PrepareRotationVectorCache(Vector2[] oldPos, float[] oldRotation) {
        rotationVectors.Clear();
        if (oldRotation == null) {
            for (int i = 0; i < oldPos.Length; i++) {
                rotationVectors.Add(GetRotationVector(oldPos, i));
            }
        }
        else {
            for (int i = 0; i < oldPos.Length; i++) {
                rotationVectors.Add(oldRotation[i].ToRotationVector2());
            }
        }
    }

    protected Vector2[] RemoveZerosAndDoOffset(Vector2[] arr, Vector2 offset) {
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

    protected Vector2 GetRotationVector(Vector2[] oldPos, int index) {
        return oldPos.Length == 1
            ? oldPos[0]
            : index == 0
            ? Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2)
            : (index == oldPos.Length - 1
            ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
            : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
    }
}