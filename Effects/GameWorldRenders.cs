using AQMod.Common.Graphics;
using AQMod.Effects.GoreNest;
using AQMod.Effects.Wind;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    public class GameWorldRenders : ModWorld
    {
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

            public static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
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

            public static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
            {
                if (!solidOnly)
                {
                    GoreNestRenderer.RefreshCoordinates();
                }
                orig(self, solidOnly, waterStyleOverride);
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