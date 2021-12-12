using AQMod.Content.WorldEvents;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class TheFan : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.knockBack = 2f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 18;
            item.useAnimation = 18;
            item.UseSound = SoundID.Item39;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.FriendlyWind>();
            item.shootSpeed = 9f;
            item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = AQItem.Tooltips.FindVanillaTooltipLineIndex(tooltips, "Tooltip");
            tooltips.Insert(index, new TooltipLine(mod, "Knockback", AQUtils.KnockbackItemTooltip(item.knockBack)));
            tooltips.Insert(index, new TooltipLine(mod, "Speed", AQUtils.UseTimeAnimationTooltip(item.shootSpeed)));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (speedX < 0f)
            {
                if (Main.windSpeedSet > ImitatedWindyDay.MinWindSpeed)
                {
                    Main.windSpeedSet += -0.01f * (1f - speedY.Abs() / item.shootSpeed);
                }
            }
            else
            {
                if (Main.windSpeedSet < ImitatedWindyDay.MaxWindSpeed)
                {
                    Main.windSpeedSet += 0.01f * (1f - speedY.Abs() / item.shootSpeed);
                }
            }
            Main.windSpeedTemp = Main.windSpeedSet;
            Projectiles.FriendlyWind.NewWind(player, position, new Vector2(speedX, speedY), item.knockBack / 10f, 60, 80);
            return false;
        }
    }
}