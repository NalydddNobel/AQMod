using Aequus.Common.Fishing;
using Aequus.Common.Items;
using Aequus.Projectiles.Misc.Bobbers;

namespace Aequus.Items.Tools.FishingPoles;

public class Starcatcher : FishingPoleItem, ItemHooks.IModifyFishingPower {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = 45;
        Item.shootSpeed = 16f;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(silver: 72);
        Item.shoot = ModContent.ProjectileType<StarcatcherBobber>();
    }

    public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        lineOriginOffset = new(38f, -30f);
        lineColor = bobber.whoAmI % 2 == 0 ? new Color(100, 200, 255, 200) : new Color(255, 255, 25, 200);
    }

    public void ModifyFishingPower(Player player, AequusPlayer fishing, Item fishingRod, ref float fishingLevel) {
        if (Main.ColorOfTheSkies.ToVector3().Length() < 1f) {
            fishingLevel += 0.2f;
        }
    }
}