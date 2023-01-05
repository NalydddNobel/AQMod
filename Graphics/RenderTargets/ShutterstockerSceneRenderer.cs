using Aequus.Items.Tools.Camera;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics.RenderTargets
{
    public class ShutterstockerSceneRenderer : RequestableRenderTarget
    {
        public const int TileSize = 16;
        public const int TilePadding = 6;

        public static int TilePaddingForChecking => TilePadding - 2;

        public static List<ShutterstockerClip> RenderRequests { get; private set; }

        public override void Load(Mod mod)
        {
            base.Load(mod);
            RenderRequests = new List<ShutterstockerClip>();
        }

        public override void Unload()
        {
            RenderRequests?.Clear();
            RenderRequests = null;
            base.Unload();
        }

        protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (RenderRequests.Count == 0 || RenderRequests[0].tileMap == null || RenderRequests[0].TooltipTexture == null)
                return;

            int size = TileSize;
            int sub = TilePadding * TileSize;
            PrepareARenderTarget_WithoutListeningToEvents(ref _target, device, RenderRequests[0].tileMap.Width * size - sub, RenderRequests[0].tileMap.Height * size - sub, RenderTargetUsage.PreserveContents);
            PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, RenderRequests[0].tileMap.Width * size - sub, RenderRequests[0].tileMap.Height * size - sub, RenderTargetUsage.DiscardContents);
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (RenderRequests.Count == 0 || !IsTargetUsable(_target) || !IsTargetUsable(helperTarget))
                return;

            if (RenderRequests[0].Item == null || 
                RenderRequests[0].tileMap == null)
            {
                RenderRequests.RemoveAt(0);
                return;
            }

            var tileMap = RenderRequests[0].tileMap;
            var texture = RenderRequests[0].TooltipTexture;

            try
            {
                DrawCapture(tileMap, RenderRequests[0]);
                spriteBatch.Begin();
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Draw(helperTarget, new Rectangle(0, 0, helperTarget.Width, helperTarget.Height), Color.White);
                if (RenderRequests[0].reviewNotesPoints != null)
                {
                    foreach (var p in RenderRequests[0].reviewNotesPoints)
                    {
                        spriteBatch.Draw(TextureAssets.Extra[ExtrasID.LaserRuler].Value, new Vector2((p.X - TilePadding / 2) * 16f, (p.Y - TilePadding / 2) * 16f), new Rectangle(0, 0, 16, 16), Color.Red * 0.8f);
                    }
                }

                spriteBatch.End();

                texture.Value = _target;
            }
            catch
            {
            }
            _target = null;

            RenderRequests.RemoveAt(0);

        }
        private static void DrawCapture(TileMapCache map, ShutterstockerClip clip)
        {
            var area = map.Area;

            area.X = (int)(clip.worldXPercent * Main.maxTilesX);
            area.Y = (int)(clip.worldYPercent * Main.maxTilesY);

            area = area.Fluffize(10);

            var screenPos = Main.screenPosition;
            var entities = new List<Entity>();
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active)
                {
                    entities.Add(Main.item[i]);
                    Main.item[i].active = false;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    entities.Add(Main.npc[i]);
                    Main.npc[i].active = false;
                }
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active)
                {
                    entities.Add(Main.projectile[i]);
                    Main.projectile[i].active = false;
                }
            }

            var myPlayer = Main.LocalPlayer.position;
            Main.BlackFadeIn = 255;
            Main.LocalPlayer.position = new Vector2(area.X * 16 - Main.LocalPlayer.width * 2f, area.Y * 16 - Main.LocalPlayer.height * 2f);
            Main.screenPosition = Main.LocalPlayer.position.Floor();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    entities.Add(Main.player[i]);
                    Main.player[i].active = false;
                    Main.player[i].outOfRange = true;
                }
            }

            var gore = new List<Gore>();
            for (int i = 0; i < Main.maxGore; i++)
            {
                if (Main.gore[i].active)
                {
                    gore.Add(Main.gore[i]);
                    Main.gore[i].active = false;
                }
            }

            var dust = new List<Dust>();
            for (int i = 0; i < Main.maxGore; i++)
            {
                if (Main.dust[i].active)
                {
                    dust.Add(Main.dust[i]);
                    Main.dust[i].active = false;
                }
            }

            var rain = new List<Rain>();
            for (int i = 0; i < Main.maxRain; i++)
            {
                if (Main.rain[i] != null && Main.rain[i].active)
                {
                    rain.Add(Main.rain[i]);
                    Main.rain[i].active = false;
                }
            }

            var renderArea = area;
            renderArea.X += TilePadding / 2;
            renderArea.Y += TilePadding / 2;
            renderArea.Width -= TilePadding;
            renderArea.Height -= TilePadding;

            //AequusHelpers.dustDebug(area.WorldRectangle());
            //AequusHelpers.dustDebug(renderArea.WorldRectangle(), DustID.CursedTorch);

            var oldMap = new TileMapCache(area);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    var p = new Point(area.X + i, area.Y + j);
                    if (i == 0 || j == 0 || i == map.Width - 1 || j == map.Height - 1)
                    {
                        Main.tile[p].Get<TileTypeData>() = new TileTypeData() { Type = TileID.DiamondGemsparkOff, };
                        Main.tile[p].Get<LiquidData>() = map[i, j].Liquid;
                        Main.tile[p].Get<TileWallWireStateData>() = map[i, j].Misc;
                        Main.tile[p].Get<WallTypeData>() = map[i, j].Wall;
                        Main.tile[p].Get<AequusTileData>() = map[i, j].Aequus;
                        Main.tile[p].Active(value: true);
                        continue;
                    }
                    Main.tile[p].Get<TileTypeData>() = map[i, j].Type;
                    Main.tile[p].Get<LiquidData>() = map[i, j].Liquid;
                    Main.tile[p].Get<TileWallWireStateData>() = map[i, j].Misc;
                    Main.tile[p].Get<WallTypeData>() = map[i, j].Wall;
                    Main.tile[p].Get<AequusTileData>() = map[i, j].Aequus;
                }
            }

            var time = Main.time;
            bool daytime = Main.dayTime;
            Main.dayTime = clip.daytime;
            Main.time = clip.time;

            try
            {
                Main.LocalPlayer.ForceUpdateBiomes();
                for (int i = 0; i < 5; i++)
                    Main.instance.DrawCapture(renderArea, new CaptureSettings() { Area = renderArea, CaptureBackground = true, CaptureEntities = true, CaptureMech = false, UseScaling = false, Biome = CaptureBiome.GetCaptureBiome(-1), });
            }
            catch
            {
            }

            Main.screenPosition = screenPos;
            Main.dayTime = daytime;
            Main.time = time;

            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    var p = new Point(area.X + i, area.Y + j);
                    Main.tile[p].Get<TileTypeData>() = oldMap[i, j].Type;
                    Main.tile[p].Get<LiquidData>() = oldMap[i, j].Liquid;
                    Main.tile[p].Get<TileWallWireStateData>() = oldMap[i, j].Misc;
                    Main.tile[p].Get<WallTypeData>() = oldMap[i, j].Wall;
                    Main.tile[p].Get<AequusTileData>() = oldMap[i, j].Aequus;
                }
            }

            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    WorldGen.SquareWallFrame(area.X + i, area.Y + j);
                    WorldGen.SquareTileFrame(area.X + i, area.Y + j);
                }
            }

            foreach (var n in entities)
            {
                n.active = true;
            }

            foreach (var g in gore)
            {
                g.active = true;
            }

            foreach (var d in dust)
            {
                d.active = true;
            }

            foreach (var r in rain)
            {
                r.active = true;
            }

            Main.LocalPlayer.position = myPlayer;
        }

        protected override bool SelfRequest()
        {
            return RenderRequests.Count > 0;
        }
    }
}