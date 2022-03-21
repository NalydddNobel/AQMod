using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class StariteinaBottle : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item2;
            item.value = Item.sellPrice(silver: 2);
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.Pets.DwarfStaritePet>();
            item.buffType = ModContent.BuffType<Buffs.Pets.DwarfStariteBuff>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 255);

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<DwarfStariteItem>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}