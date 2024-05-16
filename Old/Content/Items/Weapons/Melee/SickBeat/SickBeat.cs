namespace Aequus.Old.Content.Items.Weapons.Melee.SickBeat;

public class SickBeat : ModItem {
    public static int DropRate { get; set; } = 20;

    public override void SetDefaults() {
        Item.width = 40;
        Item.height = 40;
        Item.damage = 44;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.rare = ItemRarityID.LightRed;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.value = Item.sellPrice(gold: 5);
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 2f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<SickBeatProj>();
        Item.shootSpeed = 10.5f;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }
}