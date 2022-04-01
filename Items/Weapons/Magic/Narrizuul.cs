using Aequus.Common.Players;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class Narrizuul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 777;
            Item.knockBack = 7.77f;
            Item.crit = 3;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 7;
            Item.useAnimation = 14;
            Item.rare = ItemRarityID.Purple;
            Item.shootSpeed = 27.77f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(gold: 50);
            Item.mana = 7;
            Item.shoot = ModContent.ProjectileType<NarrizuulProj>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Main.LocalPlayer.GetModPlayer<DrawEffectsPlayer>().NalydGradientPersonal.GetColor(Main.GlobalTimeWrappedHourly);
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                ItemTooltipsHelper.DrawDevTooltip(line);
                return false;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 * 0.5f), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 * 0.5f), type, damage, knockback, player.whoAmI);
            return true;
        }
    }
}