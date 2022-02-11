using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class Narrizuul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.damage = 777;
            item.knockBack = 7.77f;
            item.crit = 3;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 7;
            item.useAnimation = 14;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.shootSpeed = 27.77f;
            item.autoReuse = true;
            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 50);
            item.mana = 7;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Narrizuul>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = TextColor();
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                TooltipText.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI); // kinda lazy so why not?
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(-MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI);
            return true;
        }

        public static Color TextColor()
        {
            return Color.Lerp(Color.Violet, Color.MediumPurple, AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f));
        }
    }
}