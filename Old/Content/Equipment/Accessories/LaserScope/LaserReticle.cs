using Aequus.Core.Initialization;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.Accessories.LaserScope;

[AutoloadGlowMask()]
[LegacyName("PrecisionGloves")]
public class LaserReticle : ModItem {
    public static float BulletSpreadMultiplier { get; set; } = 0.5f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(BulletSpreadMultiplier));

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<LaserScopePlayer>().bulletSpreadReduction *= BulletSpreadMultiplier;
    }
}

public class LaserScopeGlobalProjectile : GlobalProjectile {
    public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
        return projectile.DamageType.CountsAsClass(DamageClass.Ranged) && !projectile.arrow;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent parent && parent.Entity is Player player
            && Main.myPlayer == player.whoAmI && player.TryGetModPlayer(out LaserScopePlayer laserScopePlayer)) {

            Vector2 wantedVelocity = player.DirectionTo(Main.MouseWorld) * projectile.velocity.Length();
            projectile.velocity = Vector2.Lerp(projectile.velocity, wantedVelocity, laserScopePlayer.bulletSpreadReduction);
        }
    }
}

public class LaserScopePlayer : ModPlayer {
    public float bulletSpreadReduction;

    public override void ResetEffects() {
        bulletSpreadReduction = 0f;
    }
}
