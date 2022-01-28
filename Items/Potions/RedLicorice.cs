using AQMod.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class RedLicorice : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(silver: 20);
            item.rare = ItemRarityID.Yellow;
            item.maxStack = 999;
            item.consumable = true;
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.buffType = ModContent.BuffType<RedLicoriceBuff>();
            item.buffTime = 21600;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 200);
        }
    }
}
