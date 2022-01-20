using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Pets
{
    public class DragonBall : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
            item.value = Item.sellPrice(gold: 5);
            item.rare = ItemRarityID.Pink;
            item.expert = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Pets.MicroStarite>();
            item.buffType = ModContent.BuffType<Buffs.Pets.MicroStarite>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 150);

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }
    }
}