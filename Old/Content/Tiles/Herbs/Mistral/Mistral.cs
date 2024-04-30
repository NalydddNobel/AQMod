using Aequus.Common.Tiles;
using Aequus.Core.Graphics.Tiles;
using Aequus.Old.Content.Potions.Prefixes.EmpoweredPotions;
using System;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Herbs.Mistral;

public class Mistral : ModHerb, IDrawWindyGrass {
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        if (Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetGrowthStage(i, j) != STAGE_BLOOMING) {
            return true;
        }

        Texture2D texture = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, tile.TileColor);
        if (texture == null) {
            return true;
        }

        Vector2 offset = (TileHelper.DrawOffset - Main.screenPosition).Floor();
        Vector2 groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();

        SpriteEffects effects = SpriteEffects.None;
        SetSpriteEffects(i, j, ref effects);
        Rectangle frame = new Rectangle(tile.TileFrameX + FrameWidth - 1, tile.TileFrameY, FrameWidth, 30);
        if (TileDrawing.IsVisible(tile)) {
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, frame.Height - 2f), 1f, effects, 0f);
        }

        Vector2 rayPosition = groundPosition + offset + new Vector2(0f, -20f);
        DrawPinwheel(i, j, spriteBatch, rayPosition);

        return false;
    }

    public bool DrawWindyGrass(TileDrawCall drawInfo) {
        if (GetGrowthStage(drawInfo.X, drawInfo.Y) != STAGE_BLOOMING) {
            return true;
        }

        Vector2 rayPosition = drawInfo.Position - new Vector2(0f, drawInfo.Origin.Y - 8f).RotatedBy(drawInfo.Rotation);
        drawInfo.DrawSelf();
        DrawPinwheel(drawInfo.X, drawInfo.Y, drawInfo.SpriteBatch, rayPosition);

        return false;
    }

    private void DrawPinwheel(int i, int j, SpriteBatch spriteBatch, Vector2 position) {
        Texture2D pinwheel = AequusTextures.Mistral_Pinwheel;
        int frame = Main.tileFrame[Type];
        spriteBatch.Draw(pinwheel, position, null, Lighting.GetColor(i, j), frame / 16f * MathHelper.TwoPi, pinwheel.Size() / 2f, 1f, SpriteEffects.None, 0f);
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        frameCounter += (int)Math.Abs(Main.windSpeedCurrent * 6f);
        if (frameCounter > 3) {
            frameCounter = 0;
            frame++;
            if (frame >= 16 || frame <= 0) {
                frame = 0;
            }
        }
    }

    protected override bool IsBlooming(int i, int j) {
        return Main.WindyEnoughForKiteDrops;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 1;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        int petals = 3;
        int growthStage = GetGrowthStage(i, j);

        if (growthStage == STAGE_MATURE) {
            petals = 6;
        }
        else if (growthStage == STAGE_BLOOMING) {
            petals = 12;

            Vector2 center = new Vector2(i * 16f + 8f, j * 16f + 4f);
            for (int k = 0; k < 12; k++) {
                var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.AncientLight);
                var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();
                d.position = center + n * 4f;
                d.velocity = n * 7.5f;
                d.noGravity = true;
            }
        }

        for (int k = 0; k < petals; k++) {
            Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Grass);
        }

        return false;
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (GetGrowthStage(i, j) == STAGE_BLOOMING) {
            SoundEngine.PlaySound(AequusSounds.MoonflowerBreak with { PitchVariance = 0.1f }, new Vector2(i * 16f, j * 16f));
            //return false;
        }

        return base.KillSound(i, j, fail);
    }

    protected override void GetDropParams(int growthStage, Player closestPlayer, Item playerHeldItem, out int plant) {
        plant = ModContent.GetInstance<EmpoweredPrefix>().Item.Type;
    }

    protected override void SafeSetStaticDefaults() {
        TileObjectData.newTile.CoordinateWidth = 26;
        TileObjectData.newTile.CoordinateHeights = new int[] { 30 };
        TileObjectData.newTile.DrawYOffset = -12;
        TileObjectData.newTile.AnchorValidTiles = new int[] {
            TileID.Cloud,
            TileID.RainCloud,
            TileID.SnowCloud,
        };

        AddMapEntry(new Color(185, 235, 255), CreateMapEntryName());
    }

    public override bool CanNaturallyGrow(int X, int Y, Tile tile, bool[] anchoredTiles) {
        return Main.hardMode;
    }
}
//public class MistralTile : HerbTileBase, ISpecialTileRenderer {
//    public virtual int TurnFrames => 155;

//    protected override int[] GrowableTiles => new int[] {
//        TileID.Grass,
//        TileID.HallowedGrass,
//        TileID.Cloud,
//        TileID.RainCloud,
//        TileID.SnowCloud,
//    };

//    protected override Color MapColor => new Color(186, 122, 255, 255);
//    public override Vector3 GlowColor => new Vector3(0.1f, 0.66f, 0.15f);
//    protected override int DrawOffsetY => -8;

//    public override bool IsBlooming(int i, int j) {
//        return Main.WindyEnoughForKiteDrops;
//    }

//    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
//        var clr = GlowColor;
//        float multiplier = Math.Max(Main.tile[i, j].TileFrameX / 56, 0.1f);
//        r = clr.X * multiplier;
//        g = clr.Y * multiplier;
//        b = clr.Z * multiplier;
//    }

//    public override IEnumerable<Item> GetItemDrops(int i, int j) {
//        bool regrowth = Main.player[Player.FindClosest(new Vector2(i * 16f, j * 16f), 16, 16)].HeldItemFixed().type == ItemID.StaffofRegrowth;
//        List<Item> l = new();
//        if (Main.tile[i, j].TileFrameX >= FrameShiftX) {
//            l.Add(new(ModContent.ItemType<MistralPollen>(), regrowth ? WorldGen.genRand.Next(1, 3) : 1));
//        }
//        if (CanBeHarvestedWithStaffOfRegrowth(i, j)) {
//            l.Add(new(ModContent.ItemType<MistralSeeds>(), regrowth ? WorldGen.genRand.Next(1, 6) : WorldGen.genRand.Next(1, 4)));
//        }
//        return l;
//    }

//    public override void NumDust(int i, int j, bool fail, ref int num) {
//        num = 6;
//    }

//    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
//    }

//    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
//        var texture = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, Main.tile[i, j].TileColor);
//        if (texture == null) {
//            return true;
//        }
//        if (Main.tile[i, j].TileFrameX >= FrameWidth * 2) {
//            SpecialTileRenderer.Add(i, j, TileRenderLayer.PreDrawVines);
//        }
//        if (Main.tile[i, j].IsTileInvisible) {
//            return true;
//        }
//        var effects = SpriteEffects.None;
//        SetSpriteEffects(i, j, ref effects);
//        var frame = new Rectangle(Main.tile[i, j].TileFrameX, 0, FrameWidth, FrameHeight);
//        var offset = (Helper.TileDrawOffset - Main.screenPosition).Floor();
//        var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
//        Main.spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
//        return false;
//    }

//    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
//        return false;
//    }

//    public override void AnimateTile(ref int frame, ref int frameCounter) {
//        frame = (frame + (int)(Main.windSpeedCurrent * 100)) % (int)(MathHelper.TwoPi * TurnFrames);
//    }

//    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
//        if (Main.tile[i, j].TileFrameX < FrameWidth * 2) {
//            return;
//        }
//        var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
//        var pinwheel = PaintsRenderer.TryGetPaintedTexture(i, j, AequusTextures.MistralTile_Pinwheel.Path);
//        Main.spriteBatch.Draw(pinwheel, groundPosition - Main.screenPosition - new Vector2(0f, 20f), null, Lighting.GetColor(i, j),
//            Main.tileFrame[Type] / (float)TurnFrames, pinwheel.Size() / 2f, 1f, SpriteEffects.None, 0f);
//    }

//    public static void GlobalRandomUpdate(int i, int j) {
//        if (!AequusWorld.downedDustDevil || j >= Main.rockLayer || WorldGen.genRand.NextBool(1600)) {
//            return;
//        }

//        TryPlaceHerb<MistralTile>(i, j, 6, TileID.Cloud, TileID.RainCloud, TileID.SnowCloud);
//    }
//}