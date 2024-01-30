using Aequus.Common.Tiles;
using Aequus.Content.DataSets;
using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Assets;
using Aequus.Core.Graphics.Tiles;
using Aequus.Old.Content.TownNPCs.OccultistNPC;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Events.DemonSiege.Tiles;

[LegacyName("GoreNestTile", "GoreNest")]
public class OblivionAltar : ModTile, ISpecialTileRenderer {
    public static Int32 BiomeCount { get; set; }
    public static RequestCache<Effect> GoreNestPortal { get; private set; }

    public static ModItem Item { get; private set; }

    public override void Load() {
        if (!Main.dedServ) {
            GoreNestPortal = new("Aequus/Old/Assets/Shaders/GoreNestPortal");
        }

        Item = new InstancedTileItem(this, rarity: ItemRarityID.Orange, value: Terraria.Item.buyPrice(gold: 5));
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsSandfall[Type] = true;
        TileSets.PreventsSlopesBelow.Add(Type);

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorInvalidTiles = new[] { (Int32)TileID.MagicalIceBlock, };
        TileObjectData.newTile.CoordinateHeights = new Int32[] { 16, 16, 18 };
        TileObjectData.addTile(Type);
        DustType = DustID.Blood;
        AdjTiles = new Int32[] { TileID.DemonAltar };
        MinPick = 110;
        AddMapEntry(new Color(175, 15, 15), CreateMapEntryName());
    }

    public override void Unload() {
        Item = null;
        GoreNestPortal = null;
    }

    public override Boolean HasSmartInteract(Int32 i, Int32 j, SmartInteractScanSettings settings) {
        if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0) {
            return false;
        }
        return GetUsableDemonSiegeItem(settings.player) != null;
    }

    public override void MouseOver(Int32 i, Int32 j) {
        if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0) {
            return;
        }
        var player = Main.LocalPlayer;
        var upgradeableItem = GetUsableDemonSiegeItem(player);
        if (upgradeableItem != null) {
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = upgradeableItem.type;
        }
    }

    public override Boolean AutoSelect(Int32 i, Int32 j, Item item) {
        return AltarSacrifices.OriginalToConversion.ContainsKey(item.type);
    }

    public override Boolean RightClick(Int32 i, Int32 j) {
        var topLeft = TopLeft(i, j);
        var item = GetUsableDemonSiegeItem(Main.LocalPlayer);
        if (item == null) {
            return false;
        }
        if (DemonSiegeSystem.NewInvasion(topLeft.X, topLeft.Y, item, Main.myPlayer, allowAdding: true, allowAdding_IgnoreMax: true)) {
            ConsumeGoreNestItem(item);
        }
        else if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(topLeft, out var sacrifice)) {
            sacrifice.PreStart = Math.Min(sacrifice.PreStart, 60);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                Aequus.GetPacket<DemonSiegeSacrificeInfo.DemonSiegeStatusPacket>().Send(sacrifice);
            }
        }
        return true;
    }
    public static Boolean ConsumeGoreNestItem(Item item) {
        //if (item.type != ModContent.ItemType<VoidRing>())
        //{
        //    return false;
        //}
        item.stack--;
        if (item.stack <= 0) {
            item.TurnToAir();
        }
        return true;
    }

    public override void DrawEffects(Int32 i, Int32 j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % 48 != 0 || drawData.tileFrameY % 48 != 0) {
            return;
        }
        SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
    }

    public static void InnerDrawPortal(Point ij, Vector2 position) {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        Effect effect = GoreNestPortal.Value;
        effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
        effect.CurrentTechnique.Passes[0].Apply();
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, Utils.CenteredRectangle(position, new Vector2(82f, 82f)), Color.White);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(ij, out var invasion)) {
            InnerDrawPorter_DoDust(position + Main.screenPosition, invasion);

            Single opacity = 1f;
            Single upgradeOpacity = 0f;
            if (invasion.PreStart > 0) {
                if (invasion.PreStart < 60) {
                    opacity = 1f - invasion.PreStart / 60f;
                }
                else {
                    opacity = 0f;
                }
            }
            if (invasion.TimeLeft < 360) {
                Single time = (360 - invasion.TimeLeft) / 30f;
                upgradeOpacity = Helper.Oscillate(time, 1f, 0f);
                opacity = Helper.Oscillate(time, 0f, 1f);
            }
            if (invasion.Items.Count == 1) {
                InnerDrawPortalItem(invasion.Items[0], position, opacity, upgradeOpacity);
            }
            else if (invasion.Items.Count != 0) {
                Single outwards = 40f + invasion.Items.Count * 4f + Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, -4f, 4f);
                Single rotationAmount = MathHelper.TwoPi / invasion.Items.Count;
                for (Int32 i = 0; i < invasion.Items.Count; i++) {
                    InnerDrawPortalItem(invasion.Items[i], position + (rotationAmount * i).ToRotationVector2() * outwards, opacity, upgradeOpacity);
                }
            }
        }
    }

    // TODO -- Make this spawn dust in time intervals rather than draw ticks
    public static void InnerDrawPorter_DoDust(Vector2 where, DemonSiegeSacrificeInfo invasion) {
        if (Aequus.GameWorldActive) {
            if (invasion.PreStart > 0) {
                if (invasion.PreStart < 75) {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, 80f), DustID.SilverFlame,
                        newColor: new Color(200, 120 + Main.rand.Next(-60, 40), 30, 25));
                    d.velocity = (where - d.position) / 20f;
                    d.noGravity = true;
                }
                else {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(40f, 80f) + (invasion.PreStart - 75f) / 2f), DustID.SilverFlame,
                        newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                    d.velocity = (where - d.position) / 20f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                    d.noGravity = true;
                }
            }
            else if (invasion.TimeLeft < 360 || Main.rand.NextBool(50)) {
                var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 240f), DustID.SilverFlame,
                    newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                d.velocity = (where - d.position) / 20f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                d.noGravity = true;
            }
        }
    }
    public static void InnerDrawPortalItem(Item i, Vector2 where, Single opacity, Single upgradeOpacity, Single rotation = 0f) {
        Main.GetItemDrawFrame(i.type, out Texture2D texture, out Rectangle frame);
        Single scale = 1f;
        Int32 largest = texture.Width > texture.Height ? texture.Width : texture.Height;
        if (largest > 32) {
            scale = 32f / largest;
        }
        var origin = frame.Size() / 2f;
        var backColor = Color.Lerp(Color.Red * 0.5f, Color.OrangeRed * 0.75f, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));
        for (Int32 k = 0; k < 4; k++) {
            Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }
        Main.spriteBatch.Draw(texture, where, frame, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0f);

        if (upgradeOpacity > 0f) {
            AltarSacrifices.OriginalToConversion.TryGetValue(i.netID, out var upgrade);
            Main.instance.LoadItem(upgrade.NewItem);

            texture = TextureAssets.Item[upgrade.NewItem].Value;
            Main.GetItemDrawFrame(i.type, out texture, out frame);
            scale = 1f;
            largest = texture.Width > texture.Height ? texture.Width : texture.Height;
            if (largest > 32) {
                scale = 32f / largest;
            }
            scale = MathHelper.Lerp(scale, 1f, upgradeOpacity);
            origin = frame.Size() / 2f;
            backColor = Color.Lerp(Color.Red * 0.6f, Color.OrangeRed * 0.8f, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));

            for (Int32 k = 0; k < 4; k++) {
                Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, where, frame, Color.White * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public static Point TopLeft(Int32 x, Int32 y) {
        return new Point(x - Main.tile[x, y].TileFrameX / 18, y - Main.tile[x, y].TileFrameY / 18);
    }

    public static Item GetUsableDemonSiegeItem(Player player) {
        if (AltarSacrifices.OriginalToConversion.ContainsKey(player.HeldItemFixed().type)) {
            return player.HeldItemFixed();
        }
        for (Int32 i = 0; i < Main.InventoryItemSlotsCount; i++) {
            if (AltarSacrifices.OriginalToConversion.TryGetValue(player.inventory[i].type, out var val) && val.OriginalItem != val.NewItem) {
                return player.inventory[i];
            }
        }
        for (Int32 i = 0; i < Main.InventoryItemSlotsCount; i++) {
            if (AltarSacrifices.OriginalToConversion.TryGetValue(player.inventory[i].type, out var val)) {
                return player.inventory[i];
            }
        }
        return null;
    }

    public static Boolean IsGoreNest(Point p) {
        return IsGoreNest(p.X, p.Y);
    }
    public static Boolean IsGoreNest(Int32 x, Int32 y) {
        return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<OblivionAltar>();
    }

    void ISpecialTileRenderer.Render(Int32 i, Int32 j, Byte layer) {
        OccultistHostile.CheckSpawn(i, j, Main.myPlayer);
        InnerDrawPortal(new Point(i, j), new Vector2(i * 16f + 24f, j * 16f + 8f + Helper.Oscillate(Main.GlobalTimeWrappedHourly / 4f, -5f, 5f) - 40f) - Main.screenPosition);
    }

    public override void RandomUpdate(Int32 i, Int32 j) {
        Int32 x = i + WorldGen.genRand.Next(-10, 10);
        Int32 y = j + WorldGen.genRand.Next(-10, 10);
        if (!WorldGen.InWorld(x, y, 5)) {
            return;
        }

        TileHelper.KillLiquid(x, y, quiet: false);
    }
}