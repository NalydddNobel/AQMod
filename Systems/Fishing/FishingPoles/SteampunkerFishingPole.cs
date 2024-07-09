using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Entities.Projectiles;
using Terraria.DataStructures;

namespace AequusRemake.Systems.Fishing.FishingPoles;

public class SteampunkerFishingPole : UnifiedFishingPole {
    public override bool BobberPreAI(Projectile bobber) {
        if ((int)bobber.ai[0] == 0 && bobber.ai[1] < 0f && bobber.ai[1] > -30f && bobber.TryGetGlobalProjectile<ProjectileItemData>(out var AequusRemakeProjectile)) {
            AequusRemakeProjectile.ItemData = 1;
            Main.player[bobber.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
        }

        return true;
    }

    public override void BobberOnKill(Projectile bobber, int timeLeft) {
        if (bobber.TryGetGlobalProjectile<ProjectileItemData>(out var AequusRemakeProjectile) && AequusRemakeProjectile.ItemData == 1) {
            Main.player[bobber.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
        }
    }

    public override void SetStaticDefaults() {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = 20;
        Item.shootSpeed = 10f;
        Item.rare = ItemRarityID.LightPurple;
        Item.value = Item.buyPrice(gold: 50);
        Item.shoot = _bobber.Type;
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        lineOriginOffset = new Vector2(50f, -30f);
        lineColor = Color.LightGray;
    }
}
