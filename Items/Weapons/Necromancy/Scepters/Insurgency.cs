using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Weapons.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Scepters {
    [AutoloadGlowMask]
    public class Insurgency : ScepterBase {
        public override void SetDefaults() {
            Item.DefaultToNecromancy(50);
            Item.SetWeaponValues(125, 0.8f, 0);
            Item.shoot = ModContent.ProjectileType<InsurgentSkull>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Revenant>()
                .AddIngredient<Hexoplasm>(5)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}