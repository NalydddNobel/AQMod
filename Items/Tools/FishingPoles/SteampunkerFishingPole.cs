using Aequus.Common.Fishing;
using Aequus.Common.Projectiles;
using Aequus.Projectiles;
using Terraria.DataStructures;

namespace Aequus.Items.Tools.FishingPoles;

public class SteampunkerFishingPole : FishingPoleItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = 20;
        Item.shootSpeed = 10f;
        Item.rare = ItemRarityID.LightPurple;
        Item.value = Item.buyPrice(gold: 50);
        Item.shoot = ModContent.ProjectileType<SteampunkerBobber>();
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        lineOriginOffset = new Vector2(50f, -30f);
        lineColor = Color.LightGray;
    }

    public override bool BobberPreAI(Projectile bobber) {
        if ((int)bobber.ai[0] == 0 && bobber.ai[1] < 0f && bobber.ai[1] > -30f && bobber.TryGetGlobalProjectile<AequusProjectile>(out var aequusProjectile)) {
            aequusProjectile.specialState = ProjectileSpecialStates.SteampunkerFishingRodAutoReuse;
            Main.player[bobber.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
        }

        return true;
    }

    public override void BobberOnKill(Projectile bobber, int timeLeft) {
        if (bobber.TryGetGlobalProjectile<AequusProjectile>(out var aequusProjectile) && aequusProjectile.specialState == ProjectileSpecialStates.SteampunkerFishingRodAutoReuse) {
            Main.player[bobber.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
        }
    }

    public override void SetStaticDefaults() {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
    }
}

public class SteampunkerBobber : ModProjectile {
    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.BobberWooden);
        DrawOriginOffsetY = -8;
    }
}