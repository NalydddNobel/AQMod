using Aequus.Common.Structures.Conditions;
using Aequus.Content.Items.Consumable.RestoreMana;
using Aequus.Content.Tiles.Meadows;

namespace Aequus.Content.Items.Consumable.ThrowPowders;

public class MeadowPowder : ModItem {
    public override void SetStaticDefaults() {
        Item.CloneResearchUnlockCount(ItemID.PurificationPowder);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PurificationPowder);
        Item.shoot = ModContent.ProjectileType<MeadowPowderProj>();
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient<MeadowMushroom>()
            .AddTile(TileID.Bottles)
            .Register();
    }

    [Gen.AequusNPC_ModifyShop]
    internal static void ModifyShop(NPCShop shop) {
        if (shop.NpcType == NPCID.Dryad) {
            shop.InsertAfter(ItemID.PurificationPowder, ModContent.ItemType<MeadowPowder>(), ACondition.InMeadowsBiome);
        }
    }
}

public class MeadowPowderProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.PurificationPowder);

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.PurificationPowder);
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override bool PreAI() {
        // Override the base AI's dust.
        if (Projectile.ai[1] == 0f) {
            Projectile.ai[1] = 1f;
            for (int i = 0; i < 30; i++) {
                int type = (i % 4 == 0 || Main.rand.NextBool(4)) ? DustID.HallowSpray : DustID.SandSpray;

                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, SpeedX: Projectile.velocity.X, SpeedY: Projectile.velocity.Y, Alpha: 50, Scale: 0.45f);
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(1f);
            }
        }

        return true;
    }

    public override void AI() {
        int startX = (int)Projectile.position.X / 16;
        int endX = (int)(Projectile.position.X + Projectile.width) / 16 + 1;
        int startY = (int)Projectile.position.Y / 16;
        int endY = (int)(Projectile.position.Y + Projectile.height) / 16 + 1;

        for (int i = startX; i < endX; i++) {
            for (int j = startY; j < endY; j++) {
                Tile tile = Framing.GetTileSafely(i, j);

                if (!tile.HasTile) {
                    continue;
                }

                if (TryGetConversion(i, j, tile, out int type)) {
                    if (type <= 0) {
                        WorldGen.KillTile(i, j);
                    }
                    else {
                        tile.TileType = (ushort)type;
                        WorldGen.SquareTileFrame(i, i);
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                }

            }
        }
    }

    static bool TryGetConversion(int i, int j, Tile tile, out int type) {
        if (TileID.Sets.Grass[tile.TileType] && tile.TileType != TileID.AshGrass) {
            type = ModContent.TileType<MeadowGrass>(); return true;
        }

        if (tile.TileType == TileID.Plants && tile.TileFrameX < 810) {
            type = ModContent.TileType<MeadowPlants>(); return true;
        }

        if (tile.TileType == TileID.Plants2 && tile.TileFrameX < 108) {
            type = ModContent.TileType<MeadowPlants>(); return true;
        }

        if (tile.TileType == TileID.CorruptThorns || tile.TileType == TileID.CrimsonThorns) {
            type = 0; return true;
        }

        type = 0;
        return false;
    }
}
