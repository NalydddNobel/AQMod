using System;
using System.Collections.Generic;
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

    protected Int32 _vertexAmountCurrentlyMaintained;

    protected Int16[] _indices = new Int16[1];

    protected Int32 _indicesAmountCurrentlyMaintained;

    protected List<Vector2> _temporaryPositionsCache = new List<Vector2>();

    protected List<Single> _temporaryRotationsCache = new List<Single>();

    protected Int32 totalVertexPairs;

    public virtual void PrepareStrip(Vector2[] positions, Single[] rotations, StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 offsetForAllPositions = default(Vector2), Int32? expectedVertexPairsAmount = null, Boolean includeBacksides = false) {
        Int32 positionsLength = positions.Length;
        _vertexAmountCurrentlyMaintained = positionsLength * 2;
        if (_vertices.Length < _vertexAmountCurrentlyMaintained) {
            Array.Resize(ref _vertices, _vertexAmountCurrentlyMaintained);
        }

        totalVertexPairs = expectedVertexPairsAmount ?? positionsLength;

        for (Int32 i = 0; i < positionsLength; i++) {
            if (positions[i] == Vector2.Zero) {
                positionsLength = i - 1;
                _vertexAmountCurrentlyMaintained = positionsLength * 2;
                break;
            }

            Vector2 pos = positions[i] + offsetForAllPositions;
            Single rot = MathHelper.WrapAngle(rotations[i]);
            Int32 indexOnVertexArray = i * 2;
            Single progressOnStrip = i / (Single)(totalVertexPairs - 1);
            AddVertex(colorFunction, widthFunction, pos, rot, indexOnVertexArray, progressOnStrip);
        }

        PrepareIndices(positionsLength, includeBacksides);
    }

    public virtual void PrepareStripWithProceduralPadding(Vector2[] positions, Single[] rotations, StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 offsetForAllPositions = default(Vector2), Boolean includeBacksides = false, Boolean tryStoppingOddBug = true) {
        Int32 num = positions.Length;
        _temporaryPositionsCache.Clear();
        _temporaryRotationsCache.Clear();
        for (Int32 i = 0; i < num && !(positions[i] == Vector2.Zero); i++) {
            Vector2 vector = positions[i];
            Single num2 = MathHelper.WrapAngle(rotations[i]);
            _temporaryPositionsCache.Add(vector);
            _temporaryRotationsCache.Add(num2);
            if (i + 1 >= num || !(positions[i + 1] != Vector2.Zero)) {
                continue;
            }

            Vector2 vector2 = positions[i + 1];
            Single num3 = MathHelper.WrapAngle(rotations[i + 1]);
            Int32 num4 = (Int32)(Math.Abs(MathHelper.WrapAngle(num3 - num2)) / (MathF.PI / 12f));
            if (num4 != 0) {
                Single num5 = vector.Distance(vector2);
                Vector2 value = vector + num2.ToRotationVector2() * num5;
                Vector2 value2 = vector2 + num3.ToRotationVector2() * (0f - num5);
                Int32 num6 = num4 + 2;
                Single num7 = 1f / num6;
                Vector2 target = vector;
                for (Single num8 = num7; num8 < 1f; num8 += num7) {
                    Vector2 vector3 = Vector2.CatmullRom(value, vector, vector2, value2, num8);
                    Single item = MathHelper.WrapAngle(vector3.DirectionTo(target).ToRotation());
                    _temporaryPositionsCache.Add(vector3);
                    _temporaryRotationsCache.Add(item);
                    target = vector3;
                }
            }
        }

        Int32 count = _temporaryPositionsCache.Count;
        totalVertexPairs = count;
        Vector2 zero = Vector2.Zero;
        for (Int32 j = 0; j < count && (!tryStoppingOddBug || !(_temporaryPositionsCache[j] == zero)); j++) {
            Vector2 pos = _temporaryPositionsCache[j] + offsetForAllPositions;
            Single rot = _temporaryRotationsCache[j];
            Int32 indexOnVertexArray = j * 2;
            Single progressOnStrip = j / (Single)(count - 1);
            AddVertex(colorFunction, widthFunction, pos, rot, indexOnVertexArray, progressOnStrip);
        }

        _vertexAmountCurrentlyMaintained = count * 2;
        PrepareIndices(count, includeBacksides);
    }

    protected virtual void PrepareIndices(Int32 vertexPaidsAdded, Boolean includeBacksides) {
        Int32 pairIndicesCount = vertexPaidsAdded - 1;
        Int32 num2 = 6 + includeBacksides.ToInt() * 6;
        _indicesAmountCurrentlyMaintained = pairIndicesCount * num2;
        if (_indices.Length < _indicesAmountCurrentlyMaintained) {
            Array.Resize(ref _indices, _indicesAmountCurrentlyMaintained);
        }

        for (Int16 i = 0; i < pairIndicesCount; i = (Int16)(i + 1)) {
            Int16 num5 = (Int16)(i * num2);
            Int32 indexOnStrip = i * 2;
            _indices[num5] = (Int16)indexOnStrip;
            _indices[num5 + 1] = (Int16)(indexOnStrip + 1);
            _indices[num5 + 2] = (Int16)(indexOnStrip + 2);
            _indices[num5 + 3] = (Int16)(indexOnStrip + 2);
            _indices[num5 + 4] = (Int16)(indexOnStrip + 1);
            _indices[num5 + 5] = (Int16)(indexOnStrip + 3);
            if (includeBacksides) {
                _indices[num5 + 6] = (Int16)(indexOnStrip + 2);
                _indices[num5 + 7] = (Int16)(indexOnStrip + 1);
                _indices[num5 + 8] = (Int16)indexOnStrip;
                _indices[num5 + 9] = (Int16)(indexOnStrip + 2);
                _indices[num5 + 10] = (Int16)(indexOnStrip + 3);
                _indices[num5 + 11] = (Int16)(indexOnStrip + 1);
            }
        }
    }

    protected virtual void AddVertex(StripColorFunction colorFunction, StripHalfWidthFunction widthFunction, Vector2 pos, Single rot, Int32 indexOnVertexArray, Single progressOnStrip) {
        Color color = colorFunction(progressOnStrip);
        Single width = widthFunction(progressOnStrip);
        AddVertex(pos, rot, indexOnVertexArray, color, width, progressOnStrip);
    }
    protected virtual void AddVertex(Vector2 pos, Single rot, Int32 indexOnVertexArray, Color color, Single width, Single textureUV) {
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