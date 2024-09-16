#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Tiles;
using Aequus.Common.ContentTemplates.Tiles.Sand;
using Aequus.Content.Biomes.PollutedOcean;

namespace Aequus.Content.Tiles.PollutedOcean.PolymerSands;

public class PolymerSand : MultiMergeTile, IAddRecipes {
    public readonly ModItem Item;
    public readonly ModProjectile SandBallProjectile;
    public readonly ModProjectile SandGunProjectile;

    public PolymerSand() {
        var sandBallItem = new InstancedSandBallItem(this, bonusSandGunDamage: 2);
        Item = sandBallItem;
        SandBallProjectile = new InstancedSandBallProjectile(this, Item, friendly: false);
        SandGunProjectile = new InstancedSandBallProjectile(this, Item, friendly: true);
        sandBallItem.SetProjectile(SandGunProjectile);
    }

    public override void Load() {
        Mod.AddContent(Item);
        Mod.AddContent(SandBallProjectile);
        Mod.AddContent(SandGunProjectile);
    }

    public override void SetStaticDefaults() {
        Main.tileSand[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);
        AddMerge(ModContent.TileType<PolymerSandstone>());

        TileID.Sets.Conversion.Sand[Type] = true;
        TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
        TileID.Sets.CanBeDugByShovel[Type] = true;
        //TileID.Sets.Falling[Type] = true;
        TileID.Sets.Suffocate[Type] = true;
        TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(SandBallProjectile.Type);

        TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        TileID.Sets.GeneralPlacementTiles[Type] = false;
        TileID.Sets.ChecksForMerge[Type] = true;

        AddMapEntry(new(117, 142, 154));
        DustType = DustID.Sand;
        HitSound = SoundID.Dig;
        MineResist = 0.75f;

        Instance<PollutedTiles>().IsPolluted.Add(Type);
    }

    void IAddRecipes.AddRecipes() {
#if POLLUTED_OCEAN_TODO
        Item.CreateRecipe(5)
            .AddIngredient(ItemID.SandBlock, 5)
            .AddIngredient(ScrapBlock.Item)
            .AddTile(TileID.Furnaces)
            .Register();
#endif
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override bool HasWalkDust() {
        return Main.rand.NextBool(3);
    }

    public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color) {
        dustType = DustType;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        if (WorldGen.noTileActions) {
            return true;
        }

        for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
                int x = i + k;
                int y = j + l;
                Tile tile = Framing.GetTileSafely(x, y);

                // Return early and dont call the SpawnFallingBlockProjectile code if any water is nearby
                if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water) {
                    return true;
                }
            }
        }

        WorldGen.SpawnFallingBlockProjectile(i, j, Main.tile[i, j], Framing.GetTileSafely(i, j - 1), Framing.GetTileSafely(i, j + 1), Type);

        return true;
    }
}
#endif