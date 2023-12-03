using Aequus.Core.Graphics;
using Aequus.Core.Graphics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Content.Enemies.Glimmer;

public class UltraStariteRenderTarget : RequestRenderer {
    public static UltraStariteRenderTarget Instance { get; private set; }

    public List<DrawInfo> RenderTodo = new();

    public record DrawInfo(GenMesh Mesh, float Size) {
        internal RenderTarget2D _target;
        public RenderTarget2D Target => _target;
    }

    public override void Load(Mod mod) {
        base.Load(mod);
        Instance = this;
    }

    public override void Unload() {
        Instance = null;
    }

    protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch) {
    }

    protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        foreach (var mesh in RenderTodo) {
            PrepareARenderTarget_AndListenToEvents(ref mesh._target, device, (int)mesh.Size, (int)mesh.Size, RenderTargetUsage.PreserveContents);

            device.SetRenderTarget(mesh._target);
            device.Clear(Color.Transparent);

            mesh.Mesh.DrawMesh(new Vector3(0f, 0f, 0f), Matrix.CreateLookAt(new Vector3(0f, 0f, 2f), new Vector3(0f, 0f, 1f), new Vector3(0, 1, 0)), Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi * 0.5f, device.Viewport.AspectRatio, 1.0f, 400.0f), 1f);
        }
        RenderTodo.Clear();
    }

    protected override bool PrepareTarget() {
        return RenderTodo.Count > 0;
    }
}