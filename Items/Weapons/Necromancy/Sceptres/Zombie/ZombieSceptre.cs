using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    [AutoloadGlowMask]
    public class ZombieSceptre : SceptreBase {
        public override void SetDefaults() {
            Item.DefaultToNecromancy(10);
            Item.SetWeaponValues(4, 1f, 0);
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<ZombieSceptreProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 5;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
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