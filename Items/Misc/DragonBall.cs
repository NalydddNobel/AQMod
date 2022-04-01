using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class DragonBall : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item8;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.master = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Pets.OmegaStaritePet>();
            Item.buffType = ModContent.BuffType<Buffs.Pets.OmegaStariteBuff>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 150);

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(Item.buffType, 3600, true);
        }
    }
}