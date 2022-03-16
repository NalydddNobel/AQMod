using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class Floatstick : ModItem
    {
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shootSpeed = 6f;
            item.shoot = ModContent.ProjectileType<Projectiles.FloatstickProj>();
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.consumable = true;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 15;
            item.useTime = 15;
            item.noMelee = true;
            item.value = Item.sellPrice(copper: 50);
            item.holdStyle = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(190, 40, 220 + (int)(Math.Sin(Main.GlobalTime * 5f) * 40f));
        }

        public override void HoldItem(Player player)
        {
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, item.GetAlpha(Color.White).ToVector3() * 0.8f);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Glowstick, 200);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.SetResult(this, 200);
            r.AddRecipe();
        }
    }
}