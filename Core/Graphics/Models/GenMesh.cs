using Aequus.Core.Assets;
using Aequus.Core.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics;

namespace Aequus.Core.Graphics.Models;

public abstract class GenMesh {
    protected VertexBuffer _vertexBuffer { get; private set; }
    protected IndexBuffer _indexBuffer { get; private set; }
    protected GraphicsDevice GraphicsDevice { get; private set; }

    private VertexPositionColor[] _vertices;
    private short[] _indices;
    private VertexPositionColor[] _drawnVertices;

    protected List<VertexPositionColor> Vertices { get; private set; } = new();
    protected List<short> Indices { get; private set; } = new();

    protected void AddVertex(Vector3 vector3) {
        Vertices.Add(new(vector3, Color.White));
    }

    public void Generate() {
        Vertices.Clear();
        Indices.Clear();
        _vertexBuffer = null;
        _indexBuffer = null;

        GenerateInner();

        _vertices = Vertices.ToArray();
        _indices = Indices.ToArray();
    }

    protected abstract void GenerateInner();

    public void DrawMesh(Vector3 where, Matrix view, Matrix projection, float scale = 1f) {
        if (Indices.Count < 3) {
            return;
        }

        // Ensure Graphics Device
        if (GraphicsDevice == null || GraphicsDevice != Main.instance.GraphicsDevice) {
            GraphicsDevice = Main.instance.GraphicsDevice;
        }
        // Ensure Vertex Buffer
        if (_vertexBuffer == null || _vertexBuffer.GraphicsDevice != GraphicsDevice || _vertexBuffer.VertexCount != _vertices.Length) {
            _vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        }
        // Ensure Index Buffer
        if (_indexBuffer == null || _indexBuffer.GraphicsDevice != GraphicsDevice) {
            _indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, sizeof(short) * _indices.Length, BufferUsage.None);
        }

        int count = _vertices.Length;
        if (_drawnVertices == null || _drawnVertices.Length != count) {
            _drawnVertices = new VertexPositionColor[Vertices.Count];
        }

        for (int i = 0; i < count; i++) {
            _drawnVertices[i] = new VertexPositionColor(_vertices[i].Position * scale + where, _vertices[i].Color);
        }

        var effect = AequusShaders.VertexShader.Value;
        effect.CurrentTechnique = effect.Techniques["Untextured"];
        effect.Parameters["XViewProjection"].SetValue(view * projection);
        foreach (var pass in effect.CurrentTechnique.Passes) {
            pass.Apply();
            DrawInner(_drawnVertices, _indices);
        }
    }

    protected virtual void DrawInner(VertexPositionColor[] vertices, short[] indices) {
        _vertexBuffer.SetData(vertices);
        _indexBuffer.SetData(indices);

        GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        GraphicsDevice.Indices = _indexBuffer;

        int numVertices = _vertexBuffer.VertexCount;
        int numTris = _indexBuffer.IndexCount / 3;
        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTris);
    }

    protected void AddVertices(Vertex[] vertices) {
        foreach (var v in vertices) {
            AddVertex(v);
        }
    }

    protected void AddTriangles(TriangleIndex[] tris) {
        foreach (var i in tris) {
            Indices.Add(i[0]);
            Indices.Add(i[1]);
            Indices.Add(i[2]);
        }
    }
}