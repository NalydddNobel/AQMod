using Aequus;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.DataSets;
using Aequus.Common.Drawing;
using Aequus.Common.Effects;
using Aequus.Common.Net;
using Aequus.Content.Events.DemonSiege;
using Aequus.NPCs.Town.OccultistNPC;
using Aequus.Particles.Dusts;
using System;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Oblivion;

[LegacyName("GoreNestTile", "GoreNest")]
public class OblivionAltar : ModTile, ITileDrawSystem {
    public static int BiomeCount => OblivionAltarCount.TileCount;
    public static MiscShaderWrap<OblivionAltarShaderData> GoreNestPortal { get; private set; }

    int ITileDrawSystem.Type => Type;

    public readonly ModItem Item;
    public OblivionAltar() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        if (!Main.dedServ) {
            GoreNestPortal = new("Aequus/Assets/Effects/GoreNestPortal", "GoreNestPortal", "DemonicPortalPass", (effect, pass) => new OblivionAltarShaderData(effect, pass));
        }

        Mod.AddContent(Item);
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "GoreNest", "GoreNestItem");
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
        TileObjectData.newTile.AnchorInvalidTiles = [(int)TileID.MagicalIceBlock,];
        TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
        TileObjectData.addTile(Type);
        DustType = DustID.Blood;
        AdjTiles = [TileID.DemonAltar];
        AddMapEntry(new Color(175, 15, 15), CreateMapEntryName());
    }

    public override void Unload() {
        GoreNestPortal = null;
    }

    public override bool CanKillTile(int i, int j, ref bool blockDamaged) {
        return Aequus.MediumMode;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0) {
            return false;
        }
        return GetUsableDemonSiegeItem(settings.player) != null;
    }

    public override void MouseOver(int i, int j) {
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

    public override bool AutoSelect(int i, int j, Item item) {
        return DemonSiegeSystem.RegisteredSacrifices.ContainsKey(item.type);
    }

    public override bool RightClick(int i, int j) {
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
            if (Main.netMode != NetmodeID.SinglePlayer)
                PacketSystem.Send(sacrifice.SendStatusPacket, PacketType.DemonSiegeSacrificeStatus);
        }
        return true;
    }
    public static bool ConsumeGoreNestItem(Item item) {
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

    public static Point TopLeft(int x, int y) {
        return new Point(x - Main.tile[x, y].TileFrameX / 18, y - Main.tile[x, y].TileFrameY / 18);
    }

    public static Item GetUsableDemonSiegeItem(Player player) {
        if (DemonSiegeSystem.RegisteredSacrifices.ContainsKey(player.HeldItemFixed().type)) {
            return player.HeldItemFixed();
        }
        for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
            if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(player.inventory[i].type, out var val) && val.OriginalItem != val.NewItem) {
                return player.inventory[i];
            }
        }
        for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
            if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(player.inventory[i].type, out var val)) {
                return player.inventory[i];
            }
        }
        return null;
    }

    public static bool IsGoreNest(Point p) {
        return IsGoreNest(p.X, p.Y);
    }
    public static bool IsGoreNest(int x, int y) {
        return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<OblivionAltar>();
    }

    public override void RandomUpdate(int i, int j) {
        int x = i + WorldGen.genRand.Next(-10, 10);
        int y = j + WorldGen.genRand.Next(-10, 10);
        if (!WorldGen.InWorld(x, y, 5)) {
            return;
        }

        TileHelper.KillLiquid(x, y, quiet: false);
    }

    public static void PortalItems(Point p, Vector2 position, DemonSiegeSacrifice invasion) {
        float opacity = 1f;
        float upgradeOpacity = 0f;
        if (invasion.PreStart > 0) {
            if (invasion.PreStart < 60) {
                opacity = 1f - invasion.PreStart / 60f;
            }
            else {
                opacity = 0f;
            }
        }
        if (invasion.TimeLeft < 360) {
            float time = (360 - invasion.TimeLeft) / 30f;
            upgradeOpacity = Helper.Wave(time, 1f, 0f);
            opacity = Helper.Wave(time, 0f, 1f);
        }
        if (invasion.Items.Count == 1) {
            DrawSinglePortalItem(invasion.Items[0], position, opacity, upgradeOpacity);
        }
        else if (invasion.Items.Count != 0) {
            float outwards = 40f + invasion.Items.Count * 4f + Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, -4f, 4f);
            var c = Helper.CircularVector(invasion.Items.Count, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < invasion.Items.Count; i++) {
                DrawSinglePortalItem(invasion.Items[i], position + c[i] * outwards, opacity, upgradeOpacity);
            }
        }
    }
    public static void DrawSinglePortalItem(Item i, Vector2 where, float opacity, float upgradeOpacity, float rotation = 0f) {
        Main.instance.LoadItem(i.netID);

        var texture = TextureAssets.Item[i.type].Value;
        i.GetItemDrawData(out var frame);
        float scale = Helper.GetIconScale(32f, texture);
        var origin = frame.Size() / 2f;
        var backColor = Color.Lerp(Color.Red * 0.5f, Color.OrangeRed * 0.75f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));
        for (int k = 0; k < 4; k++) {
            Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }
        Main.spriteBatch.Draw(texture, where, frame, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0f);

        if (upgradeOpacity > 0f) {
            DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.netID, out var upgrade);
            Main.instance.LoadItem(upgrade.NewItem);

            texture = TextureAssets.Item[upgrade.NewItem].Value;
            Helper.GetItemDrawData(upgrade.NewItem, out frame);
            scale = Helper.GetIconScale(32f, texture);
            scale = MathHelper.Lerp(scale, 1f, upgradeOpacity);
            origin = frame.Size() / 2f;
            backColor = Color.Lerp(Color.Red * 0.6f, Color.OrangeRed * 0.8f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));

            for (int k = 0; k < 4; k++) {
                Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, where, frame, Color.White * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public static void PortalDust(Vector2 where, DemonSiegeSacrifice invasion) {
        if (Aequus.GameWorldActive) {
            if (invasion.PreStart > 0) {
                if (invasion.PreStart < 75) {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, 80f), ModContent.DustType<MonoDust>(),
                        newColor: new Color(200, 120 + Main.rand.Next(-60, 40), 30, 25));
                    d.velocity = (where - d.position) / 20f;
                }
                else {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(40f, 80f) + (invasion.PreStart - 75f) / 2f), ModContent.DustType<MonoSparkleDust>(),
                        newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                    d.velocity = (where - d.position) / 20f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                }
            }
            else if (invasion.TimeLeft < 360 || Main.rand.NextBool(50)) {
                var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 240f), ModContent.DustType<MonoSparkleDust>(),
                    newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                d.velocity = (where - d.position) / 20f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
            }
        }
    }

    private void DrawPortals(SpriteBatch sb) {
        float portalOffset = Helper.Wave(Main.GlobalTimeWrappedHourly / 4f, -5f, 5f) - 40f;
        Main.spriteBatch.EndCached();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        foreach (Point p in this.GetDrawPoints()) {
            OccultistHostile.CheckSpawn(p.X, p.Y, Main.myPlayer);
            Vector2 portalCoords = new Vector2(p.X * 16f + 24f, p.Y * 16f + 8f + portalOffset);

            LegacyEffects.DrawShader(GoreNestPortal.ShaderData, Main.spriteBatch, portalCoords - Main.screenPosition, Color.White, scale: 82f);

            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(p, out var invasion)) {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin_World();

                PortalItems(p, portalCoords - Main.screenPosition, invasion);

                PortalDust(portalCoords + Main.screenPosition, invasion);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        Main.spriteBatch.End();
        Main.spriteBatch.BeginCached(SpriteSortMode.Deferred, Main.Transform);
    }

    bool ITileDrawSystem.Accept(Point p) {
        return Main.tile[p].TileFrameX % 48 == 0 & Main.tile[p].TileFrameY % 48 == 0;
    }

    void IDrawSystem.Activate() {
        DrawLayers.Instance.PostDrawLiquids += DrawPortals;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.PostDrawLiquids -= DrawPortals;
    }
}