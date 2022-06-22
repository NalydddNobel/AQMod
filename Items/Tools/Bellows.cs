using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools
{
    public class Bellows : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
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
            Item.shootSpeed = 10f;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var v = Vector2.Normalize(Main.MouseWorld - player.Center).UnNaN();
                if (v.Y > 0f)
                {
                    v.Y *= (player.gravity / 0.4f);
                }
                player.velocity -= v * Item.knockBack;
                if (player.velocity.X < 4f)
                {
                    player.fallStart = (int)player.position.Y / 16;
                }
            }
            return player.itemAnimation < 45;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                int index = tooltips.GetIndex("Knockback");
                tooltips.Insert(index, new TooltipLine(Mod, "Knockback", AequusText.KBText(Item.knockBack)));
                index = tooltips.GetIndex("Speed");
                tooltips.Insert(index, new TooltipLine(Mod, "Speed", AequusText.UseAnimText(Item.useAnimation)));
            }
            catch
            {

            }
        }
    }
}