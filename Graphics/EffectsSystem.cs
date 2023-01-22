using Aequus.Biomes.Glimmer;
using Aequus.Common.Utilities;
using Aequus.Content.DronePylons;
using Aequus.Content.Necromancy.Renderer;
using Aequus.Graphics.DustDevilEffects;
using Aequus.Graphics.RenderTargets;
using Aequus.NPCs.Boss;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class EffectsSystem : ModSystem
    {
        public class ScreenShake
        {
            public float Intensity;
            public float MultiplyPerTick;

            private Vector2 _shake;

            public void Update()
            {
                if (Intensity > 0f)
                {
                    _shake = (new Vector2(Main.rand.NextFloat(-Intensity, Intensity), Main.rand.NextFloat(-Intensity, Intensity)) * 0.5f).Floor();
                    Intensity *= MultiplyPerTick;
                }
                else
                {
                    Clear();
                }
            }

            public Vector2 GetScreenOffset()
            {
                return _shake;
            }

            public void Set(float intensity, float multiplier = 0.9f)
            {
                Intensity = intensity;
                MultiplyPerTick = multiplier;
            }
            public void Clear()
            {
                Intensity = 0f;
                MultiplyPerTick = 0.9f;
                _shake = new Vector2();
            }
        }

        public static StaticMiscShaderInfo VerticalGradient { get; private set; }

        public static CachedRandom EffectRand { get; private set; }

        public static ParticleRenderer ParticlesBehindAllNPCs { get; private set; }
        public static ParticleRenderer ParticlesBehindProjs { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_BehindPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer ParticlesBehindPlayers { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_OverPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer ParticlesAbovePlayers { get; private set; }
        public static ParticleRenderer ParticlesAboveDust { get; private set; }

        public static LegacyDrawList ProjsBehindProjs { get; private set; }
        public static LegacyDrawList ProjsBehindTiles { get; private set; }

        public static DrawList<NPC> NPCsBehindAllNPCs { get; private set; }

        public static ScreenShake Shake { get; private set; }

        public static List<RequestableRenderTarget> Renderers { get; internal set; }
        public static bool ForceRenderDrawlists;

        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            VerticalGradient = new StaticMiscShaderInfo("MiscEffects", "Aequus:VerticalGradient", "VerticalGradientPass", true);
            NPCsBehindAllNPCs = new DrawList<NPC>();
            ProjsBehindProjs = new LegacyDrawList();
            ProjsBehindTiles = new LegacyDrawList();
            Shake = new ScreenShake();
            EffectRand = new CachedRandom("Split".GetHashCode(), capacity: 256 * 4);
            ParticlesBehindAllNPCs = new ParticleRenderer();
            ParticlesBehindProjs = new ParticleRenderer();
            ParticlesBehindPlayers = new ParticleRenderer();
            ParticlesAbovePlayers = new ParticleRenderer();
            ParticlesAboveDust = new ParticleRenderer();
            if (Renderers == null)
                Renderers = new List<RequestableRenderTarget>();
            LoadHooks();
        }
        private static void LoadHooks()
        {
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
            On.Terraria.Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
            On.Terraria.Main.DrawDust += Main_DrawDust;
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
        }

        public override void Unload()
        {
            VerticalGradient = null;
            ParticlesBehindProjs = null;
            Shake = null;
            ParticlesBehindAllNPCs = null;
            ParticlesBehindProjs = null;
            ParticlesBehindPlayers = null;
            ParticlesAbovePlayers = null;
            NPCsBehindAllNPCs?.Clear();
            NPCsBehindAllNPCs = null;
            ProjsBehindProjs = null;
            ProjsBehindTiles = null;
            Renderers?.Clear();
            Renderers = null;
        }

        public override void OnWorldLoad()
        {
            if (Main.dedServ)
            {
                return;
            }
        }

        public void InitWorldData()
        {
            NPCsBehindAllNPCs.Clear();
            ParticlesBehindAllNPCs = new ParticleRenderer();
            ParticlesBehindProjs = new ParticleRenderer();
            ParticlesBehindPlayers = new ParticleRenderer();
            ParticlesAbovePlayers = new ParticleRenderer();
        }

        public override void PreUpdatePlayers()
        {
            if (CosmicMonolithScene.Active > 0)
                CosmicMonolithScene.Active--;
            if (Main.netMode != NetmodeID.Server)
            {
                SnowgraveCorpse.ResetCounts();

                Shake.Update();
                ParticlesAboveDust.Update();
                ParticlesBehindAllNPCs.Update();
                ParticlesBehindProjs.Update();
                ParticlesBehindPlayers.Update();
                ParticlesAbovePlayers.Update();
                GamestarRenderer.Particles.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfig.Instance.ScreenshakeIntensity;
        }

        private static void Main_DoDraw_UpdateCameraPosition(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();
            if (Main.gameMenu)
                return;

            GhostRenderer.Instance.CheckSelfRequest();
            GhostRenderer.Instance.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
            foreach (var r in Renderers)
            {
                r.CheckSelfRequest();
                r.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
            }
        }

        private static void LegacyPlayerRenderer_DrawPlayers(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
        {
            Begin.GeneralEntities.Begin(Main.spriteBatch);
            ParticlesBehindPlayers.Draw(Main.spriteBatch);
            Main.spriteBatch.End();

            var aequusPlayers = new List<AequusPlayer>();
            foreach (var p in players)
            {
                aequusPlayers.Add(p.GetModPlayer<AequusPlayer>());
            }
            foreach (var aequus in aequusPlayers)
            {
                aequus.PreDrawAllPlayers(self, camera, players);
            }
            orig(self, camera, players);
            //foreach (var p in active)
            //{
            //    p.PostDrawAllPlayers(self);
            //}
            Begin.GeneralEntities.Begin(Main.spriteBatch);
            ParticlesAbovePlayers.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        private static void Main_DrawDust(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            try
            {
                GhostRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
                if (!Lighting.NotRetro)
                {
                    if (GamestarRenderer.Instance.IsReady)
                        GamestarRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
                }
                else if (GamestarRenderer.Instance.IsReady)
                {
                    Filters.Scene.Activate(GamestarRenderer.ScreenShaderKey, Main.LocalPlayer.Center);
                    Filters.Scene[GamestarRenderer.ScreenShaderKey].GetShader().UseOpacity(1f);
                }
                else
                {
                    Filters.Scene.Deactivate(GamestarRenderer.ScreenShaderKey, Main.LocalPlayer.Center);
                    Filters.Scene[GamestarRenderer.ScreenShaderKey].GetShader().UseOpacity(0f);
                }
                Begin.GeneralEntities.Begin(Main.spriteBatch);
                ParticlesAboveDust.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
            }
            catch
            {

            }
        }

        private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            SurgeRodProj.DrawResultTexture();
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            ParticlesBehindProjs.Draw(Main.spriteBatch);

            ProjsBehindProjs.renderingNow = true;
            for (int i = 0; i < ProjsBehindProjs.Count; i++)
            {
                Main.instance.DrawProj(ProjsBehindProjs.Index(i));
            }
            ProjsBehindProjs.Clear();

            Main.spriteBatch.End();
            orig(self);
        }

        internal static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            var particleSettings = new ParticleRendererSettings();
            try
            {
                if (!behindTiles)
                {
                    GlimmerSceneEffect.DrawUltimateSword();

                    foreach (var p in DustDevilParticleSystem.CachedBackParticles)
                    {
                        p.Draw(ref particleSettings, Main.spriteBatch);
                    }

                    DustDevil.LegacyDrawBack.renderingNow = true;
                    for (int i = 0; i < DustDevil.LegacyDrawBack.Count; i++)
                    {
                        Main.instance.DrawProj(DustDevil.LegacyDrawBack.Index(i));
                    }
                    DustDevil.LegacyDrawBack.Clear();

                    try
                    {
                        if (HealerDroneRenderer.Instance.IsReady)
                        {
                            HealerDroneRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    ParticlesBehindAllNPCs.Draw(Main.spriteBatch);
                    GhostRenderer.DrawChainedNPCs();
                    NPCsBehindAllNPCs.renderNow = true;
                    for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                    {
                        Main.instance.DrawNPC(NPCsBehindAllNPCs[i].whoAmI, behindTiles);
                    }
                    NPCsBehindAllNPCs.renderNow = false;
                    NPCsBehindAllNPCs.Clear();
                }
            }
            catch
            {
                NPCsBehindAllNPCs?.Clear();
                DustDevil.LegacyDrawBack?.Clear();
                DustDevil.LegacyDrawBack = new LegacyDrawList();
            }

            orig(self, behindTiles);

            try
            {
                if (behindTiles)
                {
                    ProjsBehindTiles.renderingNow = true;
                    for (int i = 0; i < ProjsBehindTiles.Count; i++)
                    {
                        Main.instance.DrawProj(ProjsBehindTiles.Index(i));
                    }
                    ProjsBehindTiles.Clear();
                }
                else
                {
                    foreach (var p in DustDevilParticleSystem.CachedFrontParticles)
                    {
                        p.Draw(ref particleSettings, Main.spriteBatch);
                    }

                    DustDevil.LegacyDrawFront.renderingNow = true;
                    for (int i = 0; i < DustDevil.LegacyDrawFront.Count; i++)
                    {
                        Main.instance.DrawProj(DustDevil.LegacyDrawFront.Index(i));
                    }
                    DustDevil.LegacyDrawFront.Clear();
                }
            }
            catch
            {
                ProjsBehindTiles?.Clear();
                ProjsBehindTiles = new LegacyDrawList();
                DustDevil.LegacyDrawFront?.Clear();
                DustDevil.LegacyDrawFront = new LegacyDrawList();
            }
        }

        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, Vector2? scale = null)
        {
            var sampler = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Pixel", AssetRequestMode.ImmediateLoad).Value;
            var drawData = new DrawData(sampler, drawPosition, null, color, rotation, new Vector2(0.5f, 0.5f), scale ?? Vector2.One, SpriteEffects.None, 0);
            effect.UseColor(color);
            effect.Apply(drawData);
            drawData.Draw(spriteBatch);
        }
        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, float scale = 1f)
        {
            DrawShader(effect, spriteBatch, drawPosition, color, rotation, new Vector2(scale, scale));
        }
    }
}