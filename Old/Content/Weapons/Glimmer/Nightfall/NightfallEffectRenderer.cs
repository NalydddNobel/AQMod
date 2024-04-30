using Aequus.Core;
using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Aequus.Old.Content.Weapons.Glimmer.Nightfall;

public class NightfallEffectRenderer : RequestHandler<NightfallEffectRenderer.NightfallEffectDrawData> {
    public record struct NightfallEffectDrawData(NPC NPC, float Opacity);

    private RenderTarget2D _target;
    private RenderTarget2D _helperTarget;

    protected override void OnActivate() {
        DrawHelper.AddPreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.WorldBehindTiles += DrawOntoScreen;
    }

    protected override void OnDeactivate() {
        DrawHelper.RemovePreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.WorldBehindTiles -= DrawOntoScreen;
    }

    private void HandleRequestsOnPreDraw(GameTime gameTime) {
        HandleRequests();
    }

    protected override bool HandleRequests(IEnumerable<NightfallEffectDrawData> todo) {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice device = Main.instance.GraphicsDevice;

        if (!DrawHelper.CheckTargetCycle(ref _target, Main.screenWidth, Main.screenHeight, device, RenderTargetUsage.PreserveContents)
            || !DrawHelper.CheckTargetCycle(ref _helperTarget, Main.screenWidth, Main.screenHeight, device, RenderTargetUsage.DiscardContents)) {
            return false;
        }

        Texture2D bloomTexture = AequusTextures.Bloom;
        Vector2 bloomSize = bloomTexture.Size();
        Vector2 bloomOrigin = bloomSize / 2f;
        float flareScale = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 40f, 0.3f, 0.5f);
        var flareColor = Color.BlueViolet with { A = 0 };
        Texture2D flare = AequusTextures.FlareSoft;
        Vector2 flareOrigin = flare.Size() / 2f;
        Color white = Color.White with { A = 0 };

        RenderTargetBinding[] oldTargets = device.GetRenderTargets();

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Begin();
        IEnumerable<NightfallEffectDrawData> where = todo.Where(n => n.NPC.active || n.NPC.shimmerTransparency == 0f);
        foreach (NightfallEffectDrawData n in where) {
            NPC npc = n.NPC;

            float opacity = n.Opacity;
            Vector2 drawCoordinates = npc.Center - Main.screenPosition + npc.netOffset;

            int smallestFrameSize = Math.Min(npc.frame.Width, npc.frame.Height);
            spriteBatch.Draw(
                bloomTexture,
                drawCoordinates,
                null,
                Color.Blue * 0.03f * opacity,
                0f,
                bloomOrigin,
                Math.Max(smallestFrameSize / bloomSize.X, 1f),
                SpriteEffects.None, 0f);

            int trailCount = 5;

            Vector2 difference = npc.netOffset;
            npc.position += difference;
            int alpha = npc.alpha;

            bool gameMenu = Main.gameMenu;
            Main.gameMenu = true;
            try {
                for (int i = 0; i < trailCount; i++) {
                    npc.Opacity = opacity * (1f - i / (float)trailCount) * 0.4f;

                    Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
                    difference += -npc.velocity;
                    npc.position += -npc.velocity;
                }
            }
            catch {
            }
            Main.gameMenu = gameMenu;

            npc.alpha = alpha;
            npc.position -= difference;
        }
        spriteBatch.End();

        if (!Aequus.HighQualityEffects) {
            device.SetRenderTargets(oldTargets);
            return true;
        }

        device.SetRenderTarget(_helperTarget);
        device.Clear(Color.Transparent);

        spriteBatch.Begin();

        Color outlineColor = Color.White with { A = 0 };

        spriteBatch.Draw(_target, new Rectangle(2, 0, _target.Width, _target.Height), outlineColor);
        spriteBatch.Draw(_target, new Rectangle(-2, 0, _target.Width, _target.Height), outlineColor);
        spriteBatch.Draw(_target, new Rectangle(0, 2, _target.Width, _target.Height), outlineColor);
        spriteBatch.Draw(_target, new Rectangle(0, -2, _target.Width, _target.Height), outlineColor);

        spriteBatch.End();
        device.SetRenderTarget(_target);

        spriteBatch.Begin();
        spriteBatch.Draw(_helperTarget, new Rectangle(0, 0, _target.Width, _target.Height), Color.BlueViolet);

        foreach (NightfallEffectDrawData n in where) {
            NPC npc = n.NPC;
            float opacity = n.Opacity;
            Vector2 drawCoordinates = npc.Center - Main.screenPosition + npc.netOffset;

            spriteBatch.Draw(
                flare,
                drawCoordinates,
                null,
                white*opacity,
                MathHelper.PiOver2,
                flareOrigin,
                flareScale * 0.66f,
                SpriteEffects.None, 0f);
            spriteBatch.Draw(
                flare,
                drawCoordinates,
                null,
                white * opacity,
                0f,
                flareOrigin,
                new Vector2(3f, 1f) * 0.66f * flareScale,
                SpriteEffects.None, 0f);


            spriteBatch.Draw(
                flare,
                drawCoordinates,
                null,
                flareColor * opacity,
                MathHelper.PiOver2,
                flareOrigin,
                flareScale,
                SpriteEffects.None, 0f);
            spriteBatch.Draw(
                flare,
                drawCoordinates,
                null,
                flareColor * opacity,
                0f,
                flareOrigin,
                new Vector2(3f, 1f) * flareScale,
                SpriteEffects.None, 0f);
        }

        spriteBatch.End();

        device.SetRenderTargets(oldTargets);

        return true;
    }

    private void DrawOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Draw(_target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
    }
}
