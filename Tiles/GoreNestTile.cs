using Aequus.Biomes;
using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Items.Placeable;
using Aequus.Particles.Dusts;
using Aequus.Tiles.Natural;
using AQMod.Effects.GoreNest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public class GoreNestTile : ModTile
    {
        public static int MaxY => Main.UnderworldLayer + 150;
        public static int MinY => Main.UnderworldLayer + 75;

        public static HashSet<int> AvoidTiles { get; private set; }
        public static HashSet<int> AvoidWalls { get; private set; }
        public static List<Point> RenderPoints { get; private set; }
        public static StaticMiscShaderInfo<GoreNestShaderData> GoreNestPortal { get; private set; }

        public override void Load()
        {
            AvoidTiles = new HashSet<int>()
            {
                TileID.ObsidianBrick,
                TileID.HellstoneBrick,
            };
            AvoidWalls = new HashSet<int>()
            {
                WallID.ObsidianBrick,
                WallID.ObsidianBrickUnsafe,
                TileID.HellstoneBrick,
            };
            if (!Main.dedServ)
            {
                RenderPoints = new List<Point>();
                GoreNestPortal = new StaticMiscShaderInfo<GoreNestShaderData>("GoreNestPortal", "Aequus:GoreNestPortal", "DemonicPortalPass", (effect, pass) => new GoreNestShaderData(effect, pass));

                Aequus.ResetTileRenderPoints += () => RenderPoints.Clear();
                Aequus.DrawSpecialTilePoints += DrawPortals;
            }
        }

        public override void Unload()
        {
            AvoidTiles?.Clear();
            AvoidTiles = null;
            RenderPoints?.Clear();
            RenderPoints = null;
            GoreNestPortal = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileHammer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            DustType = DustID.Blood;
            AdjTiles = new int[] { TileID.DemonAltar };
            AddMapEntry(new Color(175, 15, 15), CreateMapEntryName("GoreNest"));
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            if (DemonSiegeInvasion.Sacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0)
            {
                return false;
            }
            return GetUsableDemonSiegeItem(settings.player) != null;
        }

        public override void MouseOver(int i, int j)
        {
            if (DemonSiegeInvasion.Sacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0)
            {
                return;
            }
            var player = Main.LocalPlayer;
            var upgradeableItem = GetUsableDemonSiegeItem(player);
            if (upgradeableItem != null)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = upgradeableItem.type;
            }
        }

        public override bool AutoSelect(int i, int j, Item item)
        {
            return DemonSiegeInvasion.registeredSacrifices.ContainsKey(item.type);
        }

        public override bool RightClick(int i, int j)
        {
            var topLeft = TopLeft(i, j);
            var item = GetUsableDemonSiegeItem(Main.LocalPlayer);
            if (item == null)
            {
                return false;
            }
            if (DemonSiegeInvasion.NewInvasion(topLeft.X, topLeft.Y, item, Main.myPlayer))
            {
                goto ConsumeItem;
            }
            if (DemonSiegeInvasion.Sacrifices.TryGetValue(topLeft, out var sacrifice))
            {
                sacrifice.PreStart = Math.Min(sacrifice.PreStart, 60);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketSender.Send(sacrifice.SendStatusPacket, PacketType.DemonSiegeSacrificeStatus);
                return true;
            }
        ConsumeItem:
            item.stack--;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }
            return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<GoreNest>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % 48 == 0 && drawData.tileFrameY % 48 == 0)
            {
                RenderPoints.Add(new Point(i, j));
            }
        }

        public static void DrawPortals()
        {
            foreach (var p in RenderPoints)
            {
                InnerDrawPortal(p, p.ToWorldCoordinates() + new Vector2(16f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly / 4f, -5f, 5f) - 40f) - Main.screenPosition);
            }
        }
        public static void InnerDrawPortal(Point ij, Vector2 position)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            
            ModEffects.DrawShader(GoreNestPortal.ShaderData, Main.spriteBatch, position, Color.White, scale: 82f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            if (DemonSiegeInvasion.Sacrifices.TryGetValue(ij, out var invasion))
            {
                InnerDrawPorter_DoDust(position + Main.screenPosition, invasion);

                float opacity = 1f;
                float upgradeOpacity = 0f;
                if (invasion.PreStart > 0)
                {
                    if (invasion.PreStart < 60)
                    {
                        opacity = 1f - invasion.PreStart / 60f;
                    }
                    else
                    {
                        opacity = 0f;
                    }
                }
                if (invasion.TimeLeft < 360)
                {
                    float time = (360 - invasion.TimeLeft) / 30f;
                    upgradeOpacity = AequusHelpers.Wave(time, 1f, 0f);
                    opacity = AequusHelpers.Wave(time, 0f, 1f);
                }
                if (invasion.Items.Count == 1)
                {
                    InnerDrawPortalItem(invasion.Items[0], position, opacity, upgradeOpacity);
                }
                else if (invasion.Items.Count != 0)
                {
                    float outwards = 40f + invasion.Items.Count * 4f + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, -4f, 4f);
                    var c = AequusHelpers.CircularVector(invasion.Items.Count, Main.GlobalTimeWrappedHourly);
                    for (int i = 0; i < invasion.Items.Count; i++)
                    {
                        InnerDrawPortalItem(invasion.Items[i], position + c[i] * outwards, opacity, upgradeOpacity);
                    }
                }
            }
            //if (DemonSiege.IsActive && DemonSiege.BaseItem != null && DemonSiege.BaseItem.type > ItemID.None && DemonSiege.AltarCorner() == new Point(tileX[k], tileY[k]))
            //{
            //    var texture = TextureGrabber.GetItem(DemonSiege.BaseItem.type);
            //    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            //    var origin = frame.Size() / 2f;
            //    float scale = DemonSiege.BaseItem.scale;
            //    float rotation = (float)Math.Sin(Main.GlobalTime) * 0.05f;
            //    var drawPosition = new Vector2(portalPosition.X, portalPosition.Y);
            //    float y2 = texture.Height / 2f;
            //    if (y2 > 24f)
            //        drawPosition.Y += 24f - y2;
            //    if (ItemLoader.PreDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, ref rotation, ref scale, 666))
            //        Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            //    ItemLoader.PostDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, rotation, scale, 666);
            //}
        }
        public static void InnerDrawPorter_DoDust(Vector2 where, DemonSiegeInvasion.EventSacrifice invasion)
        {
            if (Aequus.GameWorldActive)
            {
                if (invasion.PreStart > 0)
                {
                    if (invasion.PreStart < 75)
                    {
                        var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, 80f), ModContent.DustType<MonoDust>(),
                            newColor: new Color(200, 120 + Main.rand.Next(-60, 40), 30, 222));
                        d.velocity = (where - d.position) / 20f;
                    }
                    else
                    {
                        var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(40f, 80f) + (invasion.PreStart - 75f) / 2f), ModContent.DustType<MonoSparkleDust>(),
                            newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 200) * Main.rand.NextFloat(0.9f, 1.5f));
                        d.velocity = (where - d.position) / 20f;
                        d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                    }
                }
                else if (invasion.TimeLeft < 360 || Main.rand.NextBool(50))
                {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(100f, 240f)), ModContent.DustType<MonoSparkleDust>(),
                        newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 200) * Main.rand.NextFloat(0.9f, 1.5f));
                    d.velocity = (where - d.position) / 20f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                }
            }
        }
        public static void InnerDrawPortalItem(Item i, Vector2 where, float opacity, float upgradeOpacity, float rotation = 0f)
        {
            Main.instance.LoadItem(i.netID);

            var texture = TextureAssets.Item[i.type].Value;
            i.GetItemDrawData(out var frame);
            float scale = 1f;
            int largest = texture.Width > texture.Height ? texture.Width : texture.Height;
            if (largest > 32)
            {
                scale = 32f / largest;
            }
            var origin = frame.Size() / 2f;
            var backColor = Color.Lerp(Color.Red * 0.5f, Color.OrangeRed * 0.75f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));
            foreach (var v in AequusHelpers.CircularVector(4, rotation))
            {
                Main.spriteBatch.Draw(texture, where + v * 2f, frame, backColor, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, where, frame, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0f);

            if (upgradeOpacity > 0f)
            {
                DemonSiegeInvasion.TryFromID(i.netID, out var upgrade);
                Main.instance.LoadItem(upgrade.NewItem);

                texture = TextureAssets.Item[upgrade.NewItem].Value;
                AequusHelpers.GetItemDrawData(upgrade.NewItem, out frame);
                scale = 1f;
                largest = texture.Width > texture.Height ? texture.Width : texture.Height;
                if (largest > 32)
                {
                    scale = 32f / largest;
                }
                scale = MathHelper.Lerp(scale, 1f, upgradeOpacity);
                origin = frame.Size() / 2f;
                backColor = Color.Lerp(Color.Red * 0.6f, Color.OrangeRed * 0.8f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));

                foreach (var v in AequusHelpers.CircularVector(4, rotation))
                {
                    Main.spriteBatch.Draw(texture, where + v * 2f, frame, backColor * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.Draw(texture, where, frame, Color.White * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public static Point TopLeft(int x, int y)
        {
            return new Point(x - Main.tile[x, y].TileFrameX / 18, y - Main.tile[x, y].TileFrameY / 18);
        }

        public static Item GetUsableDemonSiegeItem(Player player)
        {
            if (DemonSiegeInvasion.registeredSacrifices.ContainsKey(player.HeldItem.type))
            {
                return player.HeldItem;
            }
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                if (DemonSiegeInvasion.registeredSacrifices.ContainsKey(player.inventory[i].type))
                {
                    return player.inventory[i];
                }
            }
            return null;
        }

        public static bool TryGrowGoreNest(int x, int y)
        {
            if (Main.tile[x, y].HasTile)
            {
                if (!Main.tile[x, y - 1].HasTile)
                {
                    y--;
                }
                else
                {
                    return false;
                }
            }
            else if (!Main.tile[x, y + 1].HasTile)
            {
                return false;
            }
            //Main.NewText("Pass 1");
            if (InnerGoreNestGenCheckForBlacklistedTiles(x, y))
            {
                return false;
            }
            y -= 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x2 = x + i;
                    int y2 = y + j;
                        Main.tile[x2, y2].Active(value: false);
                    if (Main.tile[x2, y2].HasTile || Main.tile[x2, y2].LiquidAmount > 0)
                        return false;
                }
            }
            y += 3;
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                if (!Main.tile[x2, y].HasTile || !Main.tileSolid[Main.tile[x2, y].TileType] || Main.tileCut[Main.tile[x2, y].TileType])
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                Main.tile[x2, y].Slope(value: 0);
                Main.tile[x2, y].HalfBrick(value: false);
            }
            y--;
            x++;
            DustDebug(x, y);
            WorldGen.PlaceTile(x, y, ModContent.TileType<GoreNestTile>(), mute: true, forced: true);
            if (Main.tile[x, y].TileType != ModContent.TileType<GoreNestTile>())
            {
                return false;
            }
            GenerateSurroundingGoreNestHill(x, y);
            GenerateAmbientTiles(x, y);
            return true;
        }
        public static void DustDebug(int x, int y)
        {
            Rectangle rect = new Rectangle(x * 16, y * 16, 16, 16);
            for (int i = 0; i < 16; i++)
            {
                var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), DustID.Torch);
                d.noGravity = true;
                d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), DustID.Torch);
                d.noGravity = true;
                d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), DustID.Torch);
                d.noGravity = true;
                d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), DustID.Torch);
                d.noGravity = true;
            }
        }
        public static bool InnerGoreNestGenCheckForBlacklistedTiles(int x, int y)
        {
            for (int i = x - 25; i < x + 25; i++)
            {
                for (int j = y - 25; j < y + 25; j++)
                {
                    if (Main.tile[i, j].HasTile && AvoidTiles.Contains(Main.tile[i, j].TileType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void GenerateSurroundingGoreNestHill(int x, int y)
        {
            GenerateHill_SpawnAsh(x, y);
            int k = 0;
            while (y < MaxY)
            {
                if (WorldGen.genRand.NextBool(3))
                {
                    y++;
                }

                GenerateHill_SpawnAsh(x + k + 1, y);
                GenerateHill_SpawnAsh(x - k - 1, y);
                k++;
                if (k > 45 || (k > 20 && WorldGen.genRand.NextBool(15)))
                {
                    break;
                }
            }
            //int oldK = k;
            //while (true)
            //{
            //    if (x + k > Main.maxTilesX)
            //    {
            //        break;
            //    }
            //    if (Main.tile[x + k + 1, y].HasTile && !Main.tile[x + k + 1, y - 1].HasTile)
            //    {
            //        Main.NewText("ack");
            //        DustDebug(x + k + 1, y);
            //        break;
            //    }
            //    k++;
            //    if (WorldGen.genRand.NextBool(3))
            //    {
            //        y--;
            //    }
            //    Main.NewText(y);
            //    if (y < Main.UnderworldLayer)
            //    {
            //        for (int m = -10; m < 10; m++)
            //        {
            //            DustDebug(x + m, y);
            //        }
            //        DustDebug(x, Main.UnderworldLayer);
            //        Main.NewText("min y" + y + ", " + Main.UnderworldLayer + ", " + MaxY);
            //        break;
            //    }
            //    GenerateHill_SpawnAsh(x + k, y);
            //}

            GenerateHill_TryToSmoothyGoIntoRegularGeneration(x, y, k, 1);
            GenerateHill_TryToSmoothyGoIntoRegularGeneration(x, y, k, -1);
        }
        public static void GenerateHill_TryToSmoothyGoIntoRegularGeneration(int x, int y, int k, int dir)
        {
            k *= dir;
            if (y < Main.maxTilesY)
            {
                y = Main.maxTilesY - 1;
            }
            while (true)
            {
                if (x + k + dir < 0 || x + k + dir > Main.maxTilesX)
                {
                    break;
                }
                if (Main.tile[x + k + dir, y].HasTile && !Main.tile[x + k + dir, y - 1].HasTile)
                {
                    break;
                }
                k += dir;
                if (WorldGen.genRand.NextBool(3))
                {
                    y--;
                }
                if (y < MinY)
                {
                    break;
                }
                GenerateHill_SpawnAsh(x + k, y);
            }
            x -= dir;
            while (true)
            {
                x += dir;
                if (x + k < 0 || x + k > Main.maxTilesX)
                {
                    break;
                }
                y += WorldGen.genRand.Next(3);
                if (y > Main.maxTilesY)
                {
                    break;
                }
                GenerateHill_SpawnAsh(x + k, y, kill: false);
            }
        }
        public static void GenerateHill_SpawnAsh(int x, int y, bool kill = true)
        {
            int l = 0;
            while (true)
            {
                l++;
                if (y + l > Main.maxTilesY)
                {
                    break;
                }
                if (l > 75 || (l > 40 && WorldGen.genRand.NextBool(15)))
                {
                    break;
                }

                if (kill && !AvoidTiles.Contains(Main.tile[x, y - l].TileType) && !AvoidWalls.Contains(Main.tile[x, y - l].WallType)
                    && Main.tile[x, y - l].TileType != ModContent.TileType<GoreNestTile>())
                    WorldGen.KillTile(x, y - l, noItem: true);

                Main.tile[x, y + l].LiquidAmount = 0;

                if (AvoidTiles.Contains(Main.tile[x, y + l].TileType) || AvoidWalls.Contains(Main.tile[x, y + l].WallType))
                    continue;
                WorldGen.PlaceTile(x, y + l, TileID.Ash, mute: true, forced: true);
                Main.tile[x, y + l].Slope(value: 0);
                Main.tile[x, y + l].HalfBrick(value: false);
            }
        }

        public static void GenerateAmbientTiles(int x, int y)
        {
            var genTangle = new Rectangle(x - 40, y - 20, 80, 40);
            for (int i = 0; i < 1250; i++)
            {
                var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
                WorldGen.PlaceTile(v.X, v.Y, ModContent.TileType<GoreNestStalagmite>(), style: WorldGen.genRand.Next(6));
            }
        }

        public static void CleanLava(int x, int y)
        {
            for (int i = x - 60; i < x + 60; i++)
            {
                for (int j = y - 50; j < Main.maxTilesY && !Main.tile[i, j].IsSolid(); j++)
                {
                    Main.tile[i, j].LiquidAmount = 0;
                }
            }
        }
    }
}