using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class JellyAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.defense = 2;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.ammoRenewal = true;
            aQPlayer.shockCollar = true;
            player.rangedDamage += 0.05f;
            var lighting = new Vector3(1f, 1f, 1.5f);
            if (player.wet)
            {
                player.rangedCrit += 10;
            }
            else
            {
                if (hideVisual)
                {
                    lighting *= 0f;
                }
                else
                {
                    lighting *= 0.25f;
                }
            }
            Lighting.AddLight(player.Center, lighting);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Amulet>());
            r.AddIngredient(ItemID.JellyfishNecklace);
            r.AddIngredient(ModContent.ItemType<ShockCollar>());
            r.AddIngredient(ModContent.ItemType<Terraroot>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}