using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Offense.Debuff;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools
{
    public class Bellows : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.knockBack = 0.3f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.reuseDelay = 5;
            Item.UseSound = SoundID.DoubleJump;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<BellowsProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var v = Vector2.Normalize(Main.MouseWorld - player.Center).UnNaN();
                if (v.Y > 0f)
                {
                    v.Y *= player.gravity / 0.4f;
                }
                player.velocity -= v * GetPushForce(player);
                if (player.velocity.X < 4f)
                {
                    player.fallStart = (int)player.position.Y / 16;
                }
            }
            return true;
        }
        public float GetPushForce(Player player)
        {
            float force = Item.knockBack;
            if (player.mount != null && player.mount.Active && player.mount._data.usesHover)
            {
                force *= 0.33f;
            }
            force /= Math.Max(player.velocity.Length().UnNaN() / 4f, 1f);
            return force;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                int index = tooltips.GetIndex("Knockback");
                tooltips.Insert(index, new TooltipLine(Mod, "Knockback", TextHelper.KnockbackLine(Item.knockBack)));
                index = tooltips.GetIndex("Speed");
                tooltips.Insert(index, new TooltipLine(Mod, "Speed", TextHelper.UseAnimationLine(Item.useAnimation)));
            }
            catch
            {

            }
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<BoneRing>());
        }
    }
}