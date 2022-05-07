using Aequus.Items.Misc;
using Aequus.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class InsurgencyScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(50);
            Item.shoot = ModContent.ProjectileType<InsurgentSkull>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Keybrand)
                .AddIngredient<Hexoplasm>(12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}