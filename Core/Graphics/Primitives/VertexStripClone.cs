using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using static Terraria.Graphics.VertexStrip;

namespace Aequus.Core.Graphics.Primitives;

public abstract class VertexStripClone {
    protected struct CustomVertexInfo : IVertexType {
        public Vector2 Position;

        public Color Color;

        public Vector2 TexCoord;

        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        public CustomVertexInfo(Vector2 position, Color color, Vector2 texCoord) {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }
    }

    protected CustomVertexInfo[] _vertices = new CustomVertexInfo[1];

    protected int _vertexAmountCurrentlyMaintained;

    protected short[] _indices = new short[1];

    protected int _indicesAmountCurrentlyMaintained;

    protected List<Vector2> _temporaryPositionsCache = new List<Vector2>();

    protected List<float> _temporaryRotationsCache = new List<float>();

    protected int totalVertexPairs;

    public virtual void PrepareStrip(Vector2[] positions, float[] rotations, StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 offsetForAllPositions = default(Vector2), int? expectedVertexPairsAmount = null, bool includeBacksides = false) {
        int positionsLength = positions.Length;
        _vertexAmountCurrentlyMaintained = positionsLength * 2;
        if (_vertices.Length < _vertexAmountCurrentlyMaintained) {
            Array.Resize(ref _vertices, _vertexAmountCurrentlyMaintained);
        }

        totalVertexPairs = expectedVertexPairsAmount ?? positionsLength;

        for (int i = 0; i < positionsLength; i++) {
            if (positions[i] == Vector2.Zero) {
                positionsLength = i - 1;
                _vertexAmountCurrentlyMaintained = positionsLength * 2;
                break;
            }

            Vector2 pos = positions[i] + offsetForAllPositions;
            float rot = MathHelper.WrapAngle(rotations[i]);
            int indexOnVertexArray = i * 2;
            float progressOnStrip = i / (float)(totalVertexPairs - 1);
            AddVertex(colorFunction, widthFunction, pos, rot, indexOnVertexArray, progressOnStrip);
        }

        PrepareIndices(positionsLength, includeBacksides);
    }

    public virtual void PrepareStripWithProceduralPadding(Vector2[] positions, float[] rotations, StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 offsetForAllPositions = default(Vector2), bool includeBacksides = false, bool tryStoppingOddBug = true) {
        int num = positions.Length;
        _temporaryPositionsCache.Clear();
        _temporaryRotationsCache.Clear();
        for (int i = 0; i < num && !(positions[i] == Vector2.Zero); i++) {
            Vector2 vector = positions[i];
            float num2 = MathHelper.WrapAngle(rotations[i]);
            _temporaryPositionsCache.Add(vector);
            _temporaryRotationsCache.Add(num2);
            if (i + 1 >= num || !(positions[i + 1] != Vector2.Zero)) {
                continue;
            }

            Vector2 vector2 = positions[i + 1];
            float num3 = MathHelper.WrapAngle(rotations[i + 1]);
            int num4 = (int)(Math.Abs(MathHelper.WrapAngle(num3 - num2)) / (MathF.PI / 12f));
            if (num4 != 0) {
                float num5 = vector.Distance(vector2);
                Vector2 value = vector + num2.ToRotationVector2() * num5;
                Vector2 value2 = vector2 + num3.ToRotationVector2() * (0f - num5);
                int num6 = num4 + 2;
                float num7 = 1f / num6;
                Vector2 target = vector;
                for (float num8 = num7; num8 < 1f; num8 += num7) {
                    Vector2 vector3 = Vector2.CatmullRom(value, vector, vector2, value2, num8);
                    float item = MathHelper.WrapAngle(vector3.DirectionTo(target).ToRotation());
                    _temporaryPositionsCache.Add(vector3);
                    _temporaryRotationsCache.Add(item);
                    target = vector3;
                }
            }
        }

        int count = _temporaryPositionsCache.Count;
        totalVertexPairs = count;
        Vector2 zero = Vector2.Zero;
        for (int j = 0; j < count && (!tryStoppingOddBug || !(_temporaryPositionsCache[j] == zero)); j++) {
            Vector2 pos = _temporaryPositionsCache[j] + offsetForAllPositions;
            float rot = _temporaryRotationsCache[j];
            int indexOnVertexArray = j * 2;
            float progressOnStrip = j / (float)(count - 1);
            AddVertex(colorFunction, widthFunction, pos, rot, indexOnVertexArray, progressOnStrip);
        }

        _vertexAmountCurrentlyMaintained = count * 2;
        PrepareIndices(count, includeBacksides);
    }

    protected virtual void PrepareIndices(int vertexPaidsAdded, bool includeBacksides) {
        int pairIndicesCount = vertexPaidsAdded - 1;
        int num2 = 6 + includeBacksides.ToInt() * 6;
        _indicesAmountCurrentlyMaintained = pairIndicesCount * num2;
        if (_indices.Length < _indicesAmountCurrentlyMaintained) {
            Array.Resize(ref _indices, _indicesAmountCurrentlyMaintained);
        }

        for (short i = 0; i < pairIndicesCount; i = (short)(i + 1)) {
            short num5 = (short)(i * num2);
            int indexOnStrip = i * 2;
            _indices[num5] = (short)indexOnStrip;
            _indices[num5 + 1] = (short)(indexOnStrip + 1);
            _indices[num5 + 2] = (short)(indexOnStrip + 2);
            _indices[num5 + 3] = (short)(indexOnStrip + 2);
            _indices[num5 + 4] = (short)(indexOnStrip + 1);
            _indices[num5 + 5] = (short)(indexOnStrip + 3);
            if (includeBacksides) {
                _indices[num5 + 6] = (short)(indexOnStrip + 2);
                _indices[num5 + 7] = (short)(indexOnStrip + 1);
                _indices[num5 + 8] = (short)indexOnStrip;
                _indices[num5 + 9] = (short)(indexOnStrip + 2);
                _indices[num5 + 10] = (short)(indexOnStrip + 3);
                _indices[num5 + 11] = (short)(indexOnStrip + 1);
            }
        }
    }

    protected virtual void AddVertex(StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 pos, float rot, int indexOnVertexArray, float progressOnStrip) {
        Color color = colorFunction(progressOnStrip);
        float width = widthFunction(progressOnStrip);
        AddVertex(pos, rot, indexOnVertexArray, color, width, progressOnStrip);
    }
    protected virtual void AddVertex(Vector2 pos, float rot, int indexOnVertexArray, Color color, float width, float textureUV) {
        while (indexOnVertexArray + 1 >= _vertices.Length) {
            Array.Resize(ref _vertices, _vertices.Length * 2);
        }

        Vector2 vector = MathHelper.WrapAngle(rot - MathF.PI / 2f).ToRotationVector2() * width;
        _vertices[indexOnVertexArray].Position = pos + vector;
        _vertices[indexOnVertexArray + 1].Position = pos - vector;
        _vertices[indexOnVertexArray].TexCoord = new Vector2(textureUV, 1f);
        _vertices[indexOnVertexArray + 1].TexCoord = new Vector2(textureUV, 0f);
        _vertices[indexOnVertexArray].Color = color;
        _vertices[indexOnVertexArray + 1].Color = color;
    }

    public virtual void DrawTrail() {
        if (_vertexAmountCurrentlyMaintained >= 3) {
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertexAmountCurrentlyMaintained, _indices, 0, _indicesAmountCurrentlyMaintained / 3);
        }
    }
}