using Aequus.Common.Items;

namespace Aequus.Old.Content.Weapons.Melee.Valari;

public class Valari : ModItem {
    public override void SetDefaults() {
        Item.width = 40;
        Item.height = 40;
        Item.damage = 28;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.rare = ItemCommons.Rarity.DungeonLoot;
        Item.value = ItemCommons.Price.DungeonLoot;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 5f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<ValariProj>();
        Item.shootSpeed = 6.5f;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 2;
    }
}