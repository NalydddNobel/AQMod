using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class Osiris : BaseScepter
    {
        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(75, 1f, 0);
            Item.shoot = ModContent.ProjectileType<OsirisBolt>();
            Item.shootSpeed = 12.5f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4);
            Item.mana = 15;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddIngredient(ItemID.DarkShard, 2)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.RainbowRod));
        }
    }
}