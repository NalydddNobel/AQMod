﻿using Aequus.Common.Particles;
using Aequus.Common.Rendering;
using Aequus.Common.Utilities;
using Aequus.Content.Items.Materials.MonoGem;
using Aequus.Items.Equipment.Accessories.Combat.Passive.CelesteTorus;
using Aequus.Particles;
using Aequus.Tiles.Monoliths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Effects {
    public class LegacyEffects : ModSystem {
        public static LegacyMiscShaderWrap VerticalGradient { get; private set; }

        [Obsolete("Use Terraria.Utilities.FastRandom instead.")]
        public static CachedRandom EffectRand { get; private set; }

        internal static LegacyDrawList ProjsBehindProjs { get; set; }
        internal static LegacyDrawList ProjsBehindTiles { get; set; }

        public static DrawList<NPC> NPCsBehindAllNPCs { get; internal set; }

        public static List<RequestableRenderTarget> Renderers { get; internal set; }
        public static bool LegacyForceRenderDrawlists { get; set; }

        public override void Load() {
            if (Main.dedServ) {
                return;
            }
            VerticalGradient = new LegacyMiscShaderWrap("Aequus/Assets/Effects/MiscEffects", "Aequus:VerticalGradient", "VerticalGradientPass", true);
            NPCsBehindAllNPCs = new DrawList<NPC>();
            ProjsBehindProjs = new LegacyDrawList();
            ProjsBehindTiles = new LegacyDrawList();
            EffectRand = new CachedRandom("Split".GetHashCode(), capacity: 256 * 4);
            if (Renderers == null)
                Renderers = new List<RequestableRenderTarget>();
            LoadHooks();
        }
        private static void LoadHooks() {
            Terraria.Graphics.Renderers.On_LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
            Terraria.On_Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
            Terraria.On_Main.DrawDust += Main_DrawDust;
        }

        public override void Unload() {
            VerticalGradient = null;
            NPCsBehindAllNPCs?.Clear();
            NPCsBehindAllNPCs = null;
            ProjsBehindProjs = null;
            ProjsBehindTiles = null;
            Renderers?.Clear();
            Renderers = null;
        }

        public void InitWorldData() {
            if (Main.dedServ)
                return;
            foreach (var r in Renderers) {
                r.CleanUp();
            }
            ProjsBehindProjs.Clear();
            ProjsBehindTiles.Clear();
            NPCsBehindAllNPCs.Clear();
        }

        public override void OnWorldLoad() {
            InitWorldData();
        }

        public override void OnWorldUnload() {
            InitWorldData();
        }

        public override void PreUpdatePlayers() {
            if (Main.netMode != NetmodeID.Server) {
                SnowgraveCorpse.ResetCounts();
                GamestarRenderer.Particles.Update();
            }
        }

        private static void Main_DoDraw_UpdateCameraPosition(Terraria.On_Main.orig_DoDraw_UpdateCameraPosition orig) {
            orig();
            if (Main.gameMenu)
                return;

            foreach (var r in Renderers) {
                r.CheckSelfRequest();
                r.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
            }
        }

        private static void LegacyPlayerRenderer_DrawPlayers(On_LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players) {
            Main.spriteBatch.Begin_World(shader: false); ;
            ParticleSystem.GetLayer(ParticleLayer.BehindPlayers).Draw(Main.spriteBatch);
            Main.spriteBatch.End();

            CelesteTorus.DrawGlows(players);
            CelesteTorus.DrawOrbs(CelesteTorus.BackOrbsCullingRule, Reversed: true, players);

            orig(self, camera, players);
        }

        private static void Main_DrawDust(On_Main.orig_DrawDust orig, Main self) {
            CelesteTorus.DrawOrbs(CelesteTorus.FrontOrbsCullingRule, Reversed: false, Main.player);
            CelesteTorus.ClearDrawData();
            Main.spriteBatch.Begin_World(shader: false);
            ParticleSystem.GetLayer(ParticleLayer.AbovePlayers).Draw(Main.spriteBatch);
            Main.spriteBatch.End();

            orig(self);
            try {
                MonoGemRenderer.HandleScreenRender();
                if (!Lighting.NotRetro) {
                    if (GamestarRenderer.Instance.IsReady)
                        GamestarRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
                }
                else if (GamestarRenderer.Instance.IsReady) {
                    Filters.Scene.Activate(GamestarRenderer.ScreenShaderKey, Main.LocalPlayer.Center);
                    Filters.Scene[GamestarRenderer.ScreenShaderKey].GetShader().UseOpacity(1f);
                }
                else {
                    Filters.Scene.Deactivate(GamestarRenderer.ScreenShaderKey, Main.LocalPlayer.Center);
                    Filters.Scene[GamestarRenderer.ScreenShaderKey].GetShader().UseOpacity(0f);
                }
                Main.spriteBatch.Begin_World(shader: false); ;
                ParticleSystem.GetLayer(ParticleLayer.AboveDust).Draw(Main.spriteBatch);
                Main.spriteBatch.End();
            }
            catch {

            }
        }

        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, Vector2? scale = null) {
            var sampler = AequusTextures.Pixel.Value;
            var drawData = new DrawData(sampler, drawPosition, null, color, rotation, new Vector2(0.5f, 0.5f), scale ?? Vector2.One, SpriteEffects.None, 0);
            effect.UseColor(color);
            effect.Apply(drawData);
            drawData.Draw(spriteBatch);
        }
        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, float scale = 1f) {
            DrawShader(effect, spriteBatch, drawPosition, color, rotation, new Vector2(scale, scale));
        }
    }
}