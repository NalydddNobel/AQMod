using Aequus.Content.Tiles.Herbs;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Metadata;
using Terraria.ObjectData;

namespace Aequus.Common.Entities.Tiles;

public abstract class UnifiedHerb : ModTile {
    public static readonly int GrowChance = 30;

    protected int FrameWidth { get; private set; }
    protected int FullFrameWidth { get; private set; }

    protected HerbSettings Settings;

    protected virtual bool BloomConditionsMet(int i, int j) {
        return true;
    }

    public virtual void SetStaticDefaultsInner(TileObjectData obj) { }

    public bool IsBlooming(int i, int j) {
        Tile tile = Main.tile[i, j];
        return GetState(i, j) == HerbState.Bloom;
    }

    public HerbState GetState(int i, int j) {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX > 0) {
            return !BloomConditionsMet(i, j) ? HerbState.Mature : HerbState.Bloom;
        }

        return HerbState.Baby;
    }

    public sealed override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileCut[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;
        //Main.tileAlch[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.IgnoredInHouseScore[Type] = true;
        TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
        TileObjectData.newTile.AnchorAlternateTiles = [
            TileID.ClayPot,
            TileID.PlanterBox
        ];

        SetStaticDefaultsInner(TileObjectData.newTile);

        FrameWidth = TileObjectData.newTile.CoordinateWidth;
        FullFrameWidth = TileObjectData.newTile.CoordinateFullWidth;
        TileObjectData.addTile(Type);

        DustType = DustID.Grass;
        HitSound = SoundID.Grass;
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        spriteEffects = i % 2 == 0 ? SpriteEffects.FlipHorizontally : spriteEffects;
    }

    public override bool CanPlace(int i, int j) {
        Tile tile = Framing.GetTileSafely(i, j); // Safe way of getting a tile instance

        if (tile.HasTile) {
            int tileType = tile.TileType;
            if (tileType == Type) {
                return IsBlooming(i, j);
            }

            if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip) {
                bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
                bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
                bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

                if (foliageGrass || moddedFoliage || harvestableVanillaHerb) {
                    WorldGen.KillTile(i, j);
                    if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                    }

                    return true;
                }
            }

            return false;
        }

        return true;
    }

    public override void RandomUpdate(int i, int j) {
        Tile tile = Main.tile[i, j];
        HerbState state = GetState(i, j);

        if (state == HerbState.Baby && Main.rand.NextBool(GrowChance)) {
            tile.TileFrameX = (short)(tile.TileFrameX + FullFrameWidth);

            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j, 1);
            }

            return;
        }
    }

    public override bool IsTileSpelunkable(int i, int j) {
        return BloomConditionsMet(i, j);
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        if (GetState(i, j) == HerbState.Bloom) {
            tileFrameX = (short)(FullFrameWidth * 2);
        }
    }

    public override bool CanDrop(int i, int j) {
        return GetState(i, j) != HerbState.Baby;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        var state = GetState(i, j);

        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
        Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

        int plantStack = 1;

        int seedStack = 1;

        if (nearestPlayer.active && (nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth || nearestPlayer.HeldItem.type == ItemID.AcornAxe)) {
            plantStack = Main.rand.Next(1, 3);
            seedStack = Main.rand.Next(1, 6);
        }
        else if (state == HerbState.Bloom) {
            plantStack = 1;
            seedStack = Main.rand.Next(1, 4);
        }

        if (plantStack > 0) {
            yield return new Item(Settings.PlantDrop, plantStack);
        }

        if (seedStack > 0) {
            yield return new Item(Settings.SeedDrop, seedStack);
        }
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (GetState(i, j) == HerbState.Bloom) {
            SoundEngine.PlaySound(AequusSounds.PollenCollect with { PitchVariance = 0.1f }, new Vector2(i * 16f, j * 16f));
        }

        return base.KillSound(i, j, fail);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 1;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        if (Main.netMode == NetmodeID.Server) {
            return true;
        }

        int petals = 3;
        var state = GetState(i, j);

        if (state == HerbState.Mature) {
            petals = 6;
        }
        else if (state == HerbState.Bloom) {
            petals = 12;


            Vector2 center = new Vector2(i * 16f + 8f, j * 16f + 4f);

            var breakEffect = Instance<HerbBreakEffectParticle>().New();
            breakEffect.Location = center;
            breakEffect.Opacity = 1f;
            Helper.Punch(Vector2.UnitY, frames: 30);
            for (int k = 0; k < 12; k++) {
                var p = Instance<HerbBreakParticles>().New();
                var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();

                p.Location = center + n * 1f;
                p.Velocity = n * 6f;
                if (p.Velocity.Y < 0f) {
                    p.Velocity.Y *= 0.2f;
                }
                p.Color = Settings.BloomParticleColor;
                p.Opacity = 1f;
            }
        }

        for (int k = 0; k < petals; k++) {
            Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Grass);
        }

        return false;
    }
}

public record struct HerbSettings(int SeedDrop, int PlantDrop, Color BloomParticleColor);

public enum HerbState {
    Baby,
    Mature,
    Bloom
}