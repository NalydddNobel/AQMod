using Aequus.Common.Projectiles;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.FishingPoles.Steampunker;

public class SteampunkerFishingPole : ModFishingPole {
    public override bool BobberPreAI(Projectile bobber) {
        if ((int)bobber.ai[0] == 0 && bobber.ai[1] < 0f && bobber.ai[1] > -30f && bobber.TryGetGlobalProjectile<ProjectileItemData>(out var aequusProjectile)) {
            aequusProjectile.ItemData = 1;
            Main.player[bobber.owner].GetModPlayer<AequusPlayer>().forceUseItem = true;
        }

        return true;
    }

    public override void BobberOnKill(Projectile bobber, int timeLeft) {
        if (bobber.TryGetGlobalProjectile<ProjectileItemData>(out var aequusProjectile) && aequusProjectile.ItemData == 1) {
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
        Item.shoot = Bobber.Type;
    }

    public override void GetDrawData(Projectile bobber, ref float polePosX, ref float polePosY, ref Color lineColor) {
        polePosX += 50f * Main.player[bobber.owner].direction;
        polePosY -= 30f;
        lineColor = Color.LightGray;
    }
}
