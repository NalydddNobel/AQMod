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
                player.velocity -= v * Item.knockBack;
                var spawnPos = player.Center + v * (player.width + 10);
                if (Main.rand.NextBool(4))
                {
                    var g = Gore.NewGoreDirect(player.GetSource_ItemUse(Item), spawnPos,
                        v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f), GoreID.Smoke1 + Main.rand.Next(3));
                    g.scale = Main.rand.NextFloat(0.5f, 1.1f);
                    g.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                var d = Dust.NewDustDirect(spawnPos, 10, 10, DustID.Smoke);
                d.velocity *= 0.1f;
                d.velocity += v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f);
                d.scale = Main.rand.NextFloat(0.8f, 1.5f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            return player.itemAnimation < 45;
        }
    }
}