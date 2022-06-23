using Aequus.Common.Utilities;
using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class AequusEffects : ModSystem
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
                    _shake = (Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(Intensity * 0.8f, Intensity)).Floor();
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

        public static GhostOutlineTarget[] necromancyRenderers;

        public static StaticMiscShaderInfo VerticalGradient { get; private set; }

        public static MiniRandom EffectRand { get; private set; }

        public static ParticleRenderer BehindProjs { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_BehindPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer BehindPlayers { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_OverPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer AbovePlayers { get; private set; }

        public static DrawList ProjsBehindTiles { get; private set; }
        public static DrawList NPCsBehindAllNPCs { get; private set; }

        public static ScreenShake Shake { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            VerticalGradient = new StaticMiscShaderInfo("MiscEffects", "Aequus:VerticalGradient", "VerticalGradientPass", true);
            NPCsBehindAllNPCs = new DrawList();
            ProjsBehindTiles = new DrawList();
            Shake = new ScreenShake();
            EffectRand = new MiniRandom("Split".GetHashCode(), capacity: 256 * 4);
            BehindProjs = new ParticleRenderer();
            BehindPlayers = new ParticleRenderer();
            AbovePlayers = new ParticleRenderer();
            necromancyRenderers = new GhostOutlineTarget[]
            {
                new GhostOutlineTarget(0, GhostOutlineTarget.IDs.LocalPlayer, () => Color.White),
                new GhostOutlineTarget(-1, GhostOutlineTarget.IDs.Zombie, () => new Color(100, 149, 237, 255)),
                new GhostOutlineTarget(-1, GhostOutlineTarget.IDs.Revenant, () => new Color(40, 100, 237, 255)),
                new GhostOutlineTarget(-1, GhostOutlineTarget.IDs.Osiris, () => new Color(255, 128, 20, 255)),
                new GhostOutlineTarget(-1, GhostOutlineTarget.IDs.Insurgent, () => new Color(80, 255, 200, 255)),
                new GhostOutlineTarget(-1, GhostOutlineTarget.IDs.BloodRed, () => new Color(255, 10, 10, 255)),
            };
            LoadHooks();
        }
        private void LoadHooks()
        {
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
            On.Terraria.Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
            On.Terraria.Main.DrawDust += Hook_OnDrawDust;
            On.Terraria.Main.DrawProjectiles += Hook_OnDrawProjs;
            On.Terraria.Main.DrawNPCs += Hook_OnDrawNPCs;
        }

        public override void Unload()
        {
            VerticalGradient = null;
            necromancyRenderers = null;
            BehindProjs = null;
            Shake = null;
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }

        public override void OnWorldLoad()
        {
            if (Main.dedServ)
            {
                return;
            }
            BehindProjs = new ParticleRenderer();
            BehindPlayers = new ParticleRenderer();
            AbovePlayers = new ParticleRenderer();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SnowgraveCorpse.ResetCounts();

                Shake.Update();
                BehindProjs.Update();
                BehindPlayers.Update();
                AbovePlayers.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfig.Instance.ScreenshakeIntensity;
        }

        private static void LegacyPlayerRenderer_DrawPlayers(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
        {
            Begin.GeneralEntities.Begin(Main.spriteBatch);
            BehindPlayers.Draw(Main.spriteBatch);
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
            AbovePlayers.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        private static void Main_DoDraw_UpdateCameraPosition(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();
            for (int i = 0; i < necromancyRenderers.Length; i++)
            {
                if (necromancyRenderers[i] != null)
                {
                    necromancyRenderers[i].Request();
                    necromancyRenderers[i].PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
                }
            }
        }

        private static void Hook_OnDrawDust(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            try
            {
                for (int i = 0; i < necromancyRenderers.Length; i++)
                {
                    if (necromancyRenderers[i] != null && necromancyRenderers[i].IsReady)
                    {
                        necromancyRenderers[i].DrawOntoScreen(Main.spriteBatch);
                    }
                }
            }
            catch
            {

            }
        }

        private void Hook_OnDrawProjs(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            BehindProjs.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self);
        }

        internal static void Hook_OnDrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            try
            {
                NPCsBehindAllNPCs.renderingNow = true;
                for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                {
                    Main.instance.DrawNPC(NPCsBehindAllNPCs.Index(i), behindTiles);
                }
                NPCsBehindAllNPCs.Clear();

                if (!behindTiles)
                {
                    DustDevil.DrawBack.renderingNow = true;
                    for (int i = 0; i < DustDevil.DrawBack.Count; i++)
                    {
                        Main.instance.DrawProj(DustDevil.DrawBack.Index(i));
                    }
                    DustDevil.DrawBack.Clear();
                }
            }
            catch
            {
                NPCsBehindAllNPCs?.Clear();
                NPCsBehindAllNPCs = new DrawList();
                DustDevil.DrawBack?.Clear();
                DustDevil.DrawBack = new DrawList();
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
                    DustDevil.DrawFront.renderingNow = true;
                    for (int i = 0; i < DustDevil.DrawFront.Count; i++)
                    {
                        Main.instance.DrawProj(DustDevil.DrawFront.Index(i));
                    }
                    DustDevil.DrawFront.Clear();
                }
            }
            catch
            {
                ProjsBehindTiles?.Clear();
                ProjsBehindTiles = new DrawList();
                DustDevil.DrawFront?.Clear();
                DustDevil.DrawFront = new DrawList();
            }
        }

        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, Vector2? scale = null)
        {
            var sampler = Images.Pixel.Value;
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