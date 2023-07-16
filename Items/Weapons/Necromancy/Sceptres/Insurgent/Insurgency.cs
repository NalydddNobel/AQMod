using Aequus.Common.Items;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Insurgent {
    [AutoloadGlowMask]
    public class Insurgency : SceptreBase {
        public override Color GlowColor => Color.Teal;
        public override int DustSpawn => DustID.Vortex;

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
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}