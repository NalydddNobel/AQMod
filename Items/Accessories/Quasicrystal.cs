using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Quasicrystal : ModItem, IItemOverlaysWorldDraw
    {
        private static GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<Quasicrystal>("_Glow"), () => new Color(50, 50, 50, 0) * ((float)Math.Sin(Main.GlobalTime * 3f) + 1f));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeCrit += 5;
            player.magmaStone = true;
            player.starCloak = true;
            player.GetModPlayer<AQPlayer>().hyperCrystal = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Ultranium>());
            r.AddIngredient(ItemID.MagmaStone);
            r.AddIngredient(ItemID.StarCloak);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}