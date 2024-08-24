using Aequus.Common.Utilities;
using System;

namespace Aequus.Content.Items.Tools.Consumable.ScarabDynamite;

public class ScarabDynamite : ModItem {
    public static readonly int BombDuration = 360;
    public static readonly int CraftStack = 15;
    public static readonly int ExplodeRepeatCount = 3;
    public static readonly int ExplodeMove = 200 /*168*/;

    public override void SetStaticDefaults() {
        Item.CloneResearchUnlockCount(ItemID.Dynamite);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.ScarabBomb);
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<ScarabDynamiteProj>();
    }

    public override void AddRecipes() {
        CreateRecipe(CraftStack)
            .AddIngredient(ItemID.Dynamite, CraftStack)
            .AddIngredient(ItemID.AncientBattleArmorMaterial)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.ScarabBomb);
    }
}

public class ScarabDynamiteProj : ModProjectile {
    private int _timesExploded;

    private static Func<Projectile, Point>? GetScarabBombDigDirectionSnap8;

    public override string Texture => ModContent.GetInstance<ScarabDynamite>().Texture;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.Explosive[Type] = true;
        GetScarabBombDigDirectionSnap8 = typeof(Projectile).GetMethodAsDelegate<Func<Projectile, Point>>(nameof(GetScarabBombDigDirectionSnap8));
    }

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.ScarabBomb);
        Projectile.timeLeft = ScarabDynamite.BombDuration;
        AIType = ProjectileID.ScarabBomb;
    }

    public override void AI() {
        base.AI();
    }

    public override void OnKill(int timeLeft) {
        Vector2 direction = GetScarabBombDigDirectionSnap8!(Projectile).ToVector2();
        Vector2 spawnCoordinates = Projectile.Center + direction * ScarabDynamite.ExplodeMove;

        DoBigDustCircle(Projectile.Center, direction);

        if (Main.myPlayer == Projectile.owner && _timesExploded++ < ScarabDynamite.ExplodeRepeatCount) {
            // Lazy solution. Pray that it works in multiplayer.
            int p = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ProjectileID.ScarabBomb, 0, 0f, Projectile.owner);
            Main.projectile[p].Kill();

            // Summon an 'extender' projectile.
            p = Projectile.NewProjectile(Projectile.GetSource_Death(), spawnCoordinates, Projectile.velocity, ModContent.ProjectileType<ScarabDynamiteTunnelExtenderProj>(), 0, 0f, Projectile.owner);
            (Main.projectile[p].ModProjectile as ScarabDynamiteProj)!._timesExploded = _timesExploded;
        }
    }

    void DoBigDustCircle(Vector2 where, Vector2 direction) {
        const int DustWanted = 50;
        const int DustType = 59;

        float next = MathHelper.TwoPi / DustWanted;
        float rotationOffset = direction.ToRotation();
        for (float r = 0; r < MathHelper.TwoPi; r += next) {
            Vector2 spin = (r.ToRotationVector2() * new Vector2(0.35f, 1f)).RotatedBy(rotationOffset);

            Vector2 spawnCoords = where + spin * 20f;
            Vector2 velocity = spin * 4f - direction * 4f;
            Dust d = Dust.NewDustPerfect(spawnCoords, DustType, velocity, Scale: 1f);
            d.fadeIn = d.scale + 1f;
            d.noGravity = true;

            if (Main.rand.NextBool(4)) {
                d = Dust.NewDustPerfect(spawnCoords, DustType, velocity + -direction * 4f * Main.rand.NextFloat(1f, 2f), Scale: Main.rand.NextFloat(0.5f, 1f));
                d.fadeIn = d.scale * 2f;
                d.noGravity = true;
            }
        }
    }
}

public class ScarabDynamiteTunnelExtenderProj : ScarabDynamiteProj {
    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.hide = true;
        Projectile.timeLeft = 10;
    }

    public override bool ShouldUpdatePosition() {
        return false;
    }
}