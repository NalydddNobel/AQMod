using Aequus.Common.Drawing;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Rendering;
using System;
using Terraria.Audio;
using Terraria.Utilities;

namespace Aequus.Tiles.MossCaves.Radon;
public class RadonMossTile : ModTile, ITileDrawSystem, IPlaceTile {
    int ITileDrawSystem.Type => Type;

    public override void SetStaticDefaults() {
        Main.tileMoss[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        AequusTile.NoVanillaRandomTickUpdates.Add(Type);
        AddMapEntry(new Color(80, 90, 90));

        DustType = DustID.Ambient_DarkBrown;
        RegisterItemDrop(ItemID.StoneBlock);
        HitSound = SoundID.Dig;

        MineResist = 3f;
        MinPick = 100;
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
        //if (!effectOnly)
        //    Main.tile[i, j].TileType = TileID.Stone;
        //fail = true;
        //effectOnly = true;
    }

    public override void RandomUpdate(int i, int j) {
        GrowLongMoss(i, j);
        //GrowEvilPlant(i, j);
        TileHelper.SpreadGrass(i, j, TileID.Stone, ModContent.TileType<RadonMossTile>(), 1, color: Main.tile[i, j].TileColor);
        TileHelper.SpreadGrass(i, j, TileID.GrayBrick, ModContent.TileType<RadonMossBrickTile>(), 1, color: Main.tile[i, j].TileColor);
    }

    public static bool GrowEvilPlant(int i, int j) {
        int checkSize = 20;
        int plant = ModContent.TileType<RadonPlantTile>();
        var top = Main.tile[i, j - 1];
        if (top.LiquidType > 0 || top.HasTile && top.TileType != ModContent.TileType<RadonMossTile>()) {
            return false;
        }
        var rect = new Rectangle(i - checkSize, j - checkSize, checkSize * 2, checkSize * 2).Fluffize(20);
        if (!TileHelper.ScanTiles(rect, TileHelper.HasTileAction(plant), TileHelper.IsTree)) {
            if (top.TileType == ModContent.TileType<RadonMossGrass>()) {
                top.HasTile = false;
            }
            WorldGen.PlaceTile(i, j - 1, plant, mute: true);
            if (top.TileType == plant) {
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i - 1, j - 1, 3, 3);
                return true;
            }
        }
        return false;
    }
    public static void GrowLongMoss(int i, int j) {
        int radonMossGrass = ModContent.TileType<RadonMossGrass>();
        for (int k = -1; k <= 1; k += 2) {
            if (WorldGen.genRand.NextBool(4) && !Main.tile[i + k, j].HasTile && TileLoader.CanPlace(i + k, j, radonMossGrass)) {
                var tile = Main.tile[i + k, j];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileType = (ushort)radonMossGrass;
                tile.TileFrameY = 144; // Framing fix, so that the TileFrame method randomizes grass better
                WorldGen.TileFrame(i + k, j, resetFrame: true);
                return;
            }
        }
        for (int l = -1; l <= 1; l += 2) {
            if (WorldGen.genRand.NextBool(4) && !Main.tile[i, j + l].HasTile && TileLoader.CanPlace(i, j + l, radonMossGrass)) {
                var tile = Main.tile[i, j + l];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileType = (ushort)radonMossGrass;
                WorldGen.TileFrame(i, j + l, resetFrame: true);
                return;
            }
        }
    }

    public virtual bool? ModifyPlaceTile(ref PlaceTileInfo info) {
        Tile tile = info.Tile;

        if (tile.TileType == TileID.GrayBrick) {
            tile.TileType = (ushort)ModContent.TileType<RadonMossBrickTile>();
            WorldGen.SquareTileFrame(info.X, info.Y, resetFrame: true);

            if (!info.Mute) {
                SoundEngine.PlaySound(SoundID.Dig, new Vector2(info.X * 16f + 8f, info.Y * 16f + 8f));
            }

            return true;
        }

        return null;
    }

    void DrawAllFog(SpriteBatch sb) {
        var texture = AequusTextures.FogParticle;
        var origin = AequusTextures.FogParticle.GetCenteredFrameOrigin(verticalFrames: 8);
        Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        try {
            foreach (Point p in this.GetDrawPoints()) {
                int i = p.X;
                int j = p.Y;
                var rand = new FastRandom(i * i + j * j * i);

                float intensityMultiplier = MathF.Sin(Main.GlobalTimeWrappedHourly * rand.NextFloat(1f));
                if (intensityMultiplier <= 0f) {
                    continue;
                }

                var lighting = Helper.GetBrightestLight(new Point(i, j), 2);
                float intensity = 1f - (lighting.R + lighting.G + lighting.B) / 765f;
                intensity = MathHelper.Lerp(intensity, 1f, (float)MathHelper.Clamp(Vector2.Distance(new Vector2(i * 16f + 8f, j * 16f + 8f), screenCenter) / 300f - MathF.Sin(Main.GlobalTimeWrappedHourly * rand.Float(0.1f, 0.6f)).Abs(), 0f, 1f));
                intensity *= intensityMultiplier;

                Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) + new Vector2(8f).RotatedBy(rand.Float(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.Float(0.3f, 0.6f));
                Rectangle frame = AequusTextures.FogParticle.Frame(verticalFrames: 8, frameY: rand.Next(8));
                float rotation = rand.Next(4) * MathHelper.PiOver2;

                sb.Draw(texture, drawCoordinates - Main.screenPosition, frame, Color.Black * intensity, rotation, origin, 4f, SpriteEffects.None, 0f);
            }
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
    }

    void IDrawSystem.Activate() {
        RadonMossFogRenderer.Instance.Draw += DrawAllFog;
        RadonMossFogRenderer.Instance.Active = true;
    }

    void IDrawSystem.Deactivate() {
        RadonMossFogRenderer.Instance.Active = false;
        RadonMossFogRenderer.Instance.Draw -= DrawAllFog;
    }
}