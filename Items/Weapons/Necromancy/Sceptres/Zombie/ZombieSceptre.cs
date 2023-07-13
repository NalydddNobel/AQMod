using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    [AutoloadGlowMask]
    public class ZombieSceptre : SceptreBase {
        public override void SetDefaults() {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<ZombieSceptreProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
        }

        public override bool CanUseItem(Player player) {
            return true;
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