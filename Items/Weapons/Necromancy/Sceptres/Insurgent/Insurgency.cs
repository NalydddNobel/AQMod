using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Weapons.Necromancy.Sceptres.Revenant;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Insurgent {
    [AutoloadGlowMask]
    public class Insurgency : SceptreBase {
        public override void SetDefaults() {
            Item.DefaultToNecromancy(50);
            Item.SetWeaponValues(125, 0.8f, 0);
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Revenant.Revenant>()
                .AddIngredient<Hexoplasm>(5)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}