using Aequus.Biomes;
using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Items.Placeable;
using Aequus.Particles.Dusts;
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
        public static List<Point> RenderPoints { get; private set; }
        public static StaticMiscShaderInfo<GoreNestShaderData> GoreNestPortal { get; private set; }

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

            if (!Main.dedServ)
            {
                RenderPoints = new List<Point>();
                GoreNestPortal = new StaticMiscShaderInfo<GoreNestShaderData>("GoreNestPortal", "Aequus:GoreNestPortal", "DemonicPortalPass", (effect, pass) => new GoreNestShaderData(effect, pass));

                Aequus.ResetTileRenderPoints += () => RenderPoints.Clear();
                Aequus.DrawSpecialTilePoints += DrawPortals;
            }
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            //if (DemonSiege.IsActive || !AQPlayer.InteractionDelay())
            //{
            //    return false;
            //}
            //return DemonSiege.FindUpgradeableItem(Main.LocalPlayer).item != null;
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            //if (DemonSiege.IsActive)
            //{
            //    return;
            //}
            //var player = Main.player[Main.myPlayer];
            //var upgradeableItem = DemonSiege.FindUpgradeableItem(player);
            //if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None)
            //{
            //    player.noThrow = 2;
            //    player.showItemIcon = true;
            //    player.showItemIcon2 = upgradeableItem.item.type;
            //}
        }

        public override bool AutoSelect(int i, int j, Item item)
        {
            //return DemonSiege.GetUpgrade(item) != null;
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            //if (DemonSiege.IsActive)
            //{
            //    return false;
            //}
            //var player = Main.player[Main.myPlayer];
            //var upgradeableItem = DemonSiege.FindUpgradeableItem(player);
            //if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None && AQPlayer.InteractionDelay(apply: 1800))
            //{
            //    DemonSiege.Activate(i, j, player.whoAmI, upgradeableItem.item);
            //    Main.PlaySound(SoundID.DD2_EtherianPortalOpen, new Vector2(i * 16f, j * 16f));
            //}
            //foreach (var s in DemonSiegeInvasion.registeredSacrifices)
            //{
            //    Main.NewText(AequusText.ItemText(s.Key) + " -> " + AequusText.ItemText(s.Value.NewItem));
            //}
            var topLeft = TopLeft(i, j);
            if (DemonSiegeInvasion.NewInvasion(topLeft.X, topLeft.Y, Main.LocalPlayer.HeldItem, Main.myPlayer))
            {
                SoundID.DD2_EtherianPortalOpen?.PlaySound(new Vector2(i * 16f, j * 16f));
                return true;
            }
            if (DemonSiegeInvasion.Sacrifices.TryGetValue(topLeft, out var sacrifice))
            {
                sacrifice.PreStart = Math.Min(sacrifice.PreStart, 60);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketSender.Send(sacrifice.SendStatusPacket, PacketType.DemonSiegeSacrificeStatus);
            }
            return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode/* && (!DemonSiege.IsActive || !DemonSiege.AltarRectangle().Contains(i, j))*/;
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

        internal static bool TryGrowGoreNest(int x, int y, bool checkOuterThirds, bool checkIsHellLayer)
        {
            if (checkOuterThirds)
            {
                int thirdX = Main.maxTilesX / 3;
                if (x <= thirdX || x >= Main.maxTilesX - thirdX)
                    return false;
            }
            if (checkIsHellLayer)
            {
                if (y < Main.UnderworldLayer - 100) // 300 tiles from the bottom of the world
                    return false;
            }
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
            y -= 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x2 = x + i;
                    int y2 = y + j;
                    if (Main.tileCut[Main.tile[x2, y2].TileType])
                        Main.tile[x2, y2].Active(value: false);
                    if (Framing.GetTileSafely(x2, y2).HasTile || Main.tile[x2, y2].LiquidAmount > 0)
                        return false;
                }
            }
            y += 3;
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                if (!Framing.GetTileSafely(x2, y).HasTile || !Main.tileSolid[Main.tile[x2, y].TileType] || Main.tileCut[Main.tile[x2, y].TileType])
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                Main.tile[x2, y].Slope(value: 0);
                Main.tile[x2, y].HalfBrick(value: false);
            }
            y--;
            int tileType = ModContent.TileType<GoreNestTile>();
            WorldGen.PlaceTile(x, y, tileType);
            if (Main.tile[x, y].TileType == tileType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}