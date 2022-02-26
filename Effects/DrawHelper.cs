using AQMod.Common.Graphics;
using AQMod.Content.Entities;
using AQMod.Effects.GoreNest;
using AQMod.Effects.Wind;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    public sealed class DrawHelper : ModWorld
    {
        public static int Main_bgTop;

        public static class Hooks
        {
            internal static int LastScreenWidth;
            internal static int LastScreenHeight;

            internal static void Main_UpdateDisplaySettings(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
            {
                orig(self);
                if (!Main.gameMenu && Main.graphics.GraphicsDevice != null && !Main.graphics.GraphicsDevice.IsDisposed && Main.spriteBatch != null)
                {
                    if (LastScreenWidth != Main.screenWidth || LastScreenHeight != Main.screenHeight)
                    {
                        WindLayer.ResetTargets(Main.graphics.GraphicsDevice);
                    }
                    WindLayer.DrawTargets();
                    AQGraphics.SetCullPadding();
                    LastScreenWidth = Main.screenWidth;
                    LastScreenHeight = Main.screenHeight;
                }
            }

            internal static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
            {
                try
                {
                    NPCsBehindAllNPCs.drawingNow = true;
                    for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                    {
                        Main.instance.DrawNPC(NPCsBehindAllNPCs.Indices[i], behindTiles);
                    }
                    NPCsBehindAllNPCs.Clear();
                }
                catch
                {
                    NPCsBehindAllNPCs?.Clear();
                    NPCsBehindAllNPCs = new DrawCache();
                }
                if (!behindTiles)
                {
                    GoreNestRenderer.Render();
                    UltimateSwordRenderer.Render();
                    WindLayer.DrawFinal();
                }
                orig(self, behindTiles);
                if (behindTiles)
                {
                    try
                    {
                        ProjsBehindTiles.drawingNow = true;
                        if (ProjsBehindTiles != null)
                        {
                            for (int i = 0; i < ProjsBehindTiles.Count; i++)
                            {
                                Main.instance.DrawProj(ProjsBehindTiles.Indices[i]);
                            }
                        }
                        ProjsBehindTiles.Clear();
                    }
                    catch
                    {
                        ProjsBehindTiles?.Clear();
                        ProjsBehindTiles = new DrawCache();
                    }
                }
            }

            internal static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
            {
                if (!solidOnly)
                {
                    GoreNestRenderer.RefreshCoordinates();
                }
                orig(self, solidOnly, waterStyleOverride);
            }

            internal static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
            {
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                Particle.PreDrawProjectiles.Render();
                Trail.PreDrawProjectiles.Render();
                Main.spriteBatch.End();
                orig(self);
            }

            internal static void Main_DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
            {
                orig(self);
                AQGraphics.SetCullPadding();
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                for (int i = 0; i < CrabPot.maxCrabPots; i++)
                {
                    CrabPot.crabPots[i].Render();
                }
                Particle.PostDrawPlayers.Render();
                Main.spriteBatch.End();
            }
        }
        public static class BGStars
        {
            public static Vector2 GetRenderPosition(Star star)
            {
                return GetRenderPosition(new Vector2(star.position.X + Main.starTexture[star.type].Width * 0.5f, star.position.Y + Main.starTexture[star.type].Height * 0.5f));
            }
            public static Vector2 GetRenderPosition(Vector2 position)
            {
                return new Vector2(position.X * (Main.screenWidth / 800f),
                    position.Y * (Main.screenHeight / 600f) + Main_bgTop);
            }
        }

        public class DrawCache
        {
            public readonly List<int> Indices;
            public int Count => Indices.Count;

            public bool drawingNow;

            public DrawCache()
            {
                Indices = new List<int>();
            }

            public void Clear()
            {
                Indices.Clear();
                drawingNow = false;
            }

            public void Add(int item)
            {
                Indices.Add(item);
            }
        }

        public static DrawCache NPCsBehindAllNPCs { get; private set; }
        public static DrawCache ProjsBehindTiles { get; private set; }

        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                UltimateSwordRenderer.Initalize();
            }
        }

        internal static void Load()
        {
            Hooks.LastScreenWidth = 0;
            Hooks.LastScreenHeight = 0;
            NPCsBehindAllNPCs = new DrawCache();
            ProjsBehindTiles = new DrawCache();
        }

        internal static void Unload()
        {
            NPCsBehindAllNPCs?.Clear();
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles?.Clear();
            ProjsBehindTiles = null;
        }

        internal static void DoUpdate()
        {
            UltimateSwordRenderer.Update();
        }
    }
}