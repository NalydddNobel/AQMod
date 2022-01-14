using AQMod.Common.ID;
using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class VineSword : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.damage = 10;
            item.useTime = 4;
            item.useAnimation = 4;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 2);
            item.rare = ItemRarityID.Blue;
            item.melee = true;
            item.knockBack = 1.25f;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.WhackAZombie>();
        }

        public override void HoldItem(Player player)
        {
            if (Main.cursorOverride <= 0 && !player.mouseInterface)
                player.GetModPlayer<PlayerCursorDyes>().VisibleCursorDye = CursorDyeID.WhackAZombie;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            var normal = Vector2.Normalize(position - player.Center);
            speedX = normal.X * 0.1f;
            speedY = normal.Y * 0.1f;
            if (Vector2.Distance(position, player.Center) > 200f)
            {
                position = player.Center + normal * 200f;
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}