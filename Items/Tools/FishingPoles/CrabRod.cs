using Aequus.Common.Fishing;
using Aequus.Projectiles.Misc.Bobbers;

namespace Aequus.Items.Tools.FishingPoles;
public class CrabRod : FishingPoleItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = 45;
        Item.shootSpeed = 24f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 80);
        Item.shoot = ModContent.ProjectileType<CrabBobber>();
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        lineOriginOffset = new(38f, -34f);
        lineColor = new Color(255, 200, 200, 255);
    }
}