using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    [AutoloadGlowMask]
    public class ZombieScepter : SceptreBase {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealAmount);

        public override void SetDefaults() {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shootSpeed = 9f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            HealAmount = 2;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 4)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterAfter(ItemID.RainbowRod)
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}