using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Unique
{
    public class PotionOfResurrection : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, Color.DarkRed, };
            SacrificeTotal = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WarmthPotion);
            Item.buffType = 0;
            Item.buffTime = 0;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == 2)
            {
                for (int i = 0; i < 60; i++)
                {
                    var d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.velocity = player.velocity * 0.2f;
                    d.noGravity = true;
                }

                if (player.lastDeathPostion == new Vector2(0f, 0f))
                    return;

                player.Teleport(player.lastDeathPostion - new Vector2(0f, 48f), -1);

                var s = Main.screenPosition;
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.screenPosition = player.Center - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                }

                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.velocity *= 0.5f;
                    d.velocity += player.velocity * 0.2f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.FoodPiece, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.velocity *= 0.5f;
                    d.velocity.Y = Math.Abs(d.velocity.Y);
                    d.velocity += player.velocity * 0.2f + new Vector2(Main.rand.NextFloat(-2f, 2f), -4f);
                    d.color = Color.Red;
                }
                Main.screenPosition = s;
            }
        }

        public override bool? UseItem(Player player)
        {
            return player.lastDeathPostion != new Vector2(0f, 0f) ? null : false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<BloodyTearstone>()
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}