using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class Globebulb : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<Globebulb>("_Glow"),
            () => Color.Lerp(new Color(95, 80, 20, 0), new Color(0, 0, 0, 0), ((float)Math.Sin(Main.GlobalTime * 15f) + 1f) * 0.25f + 0.25f));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 1);
            item.useTime = 20;
            item.useAnimation = 20;
            item.mana = 20;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override void HoldItem(Player player)
        {
            player.accWatch += 3;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                AQSystem.DayrateIncrease += 24;
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 9);
            r.AddRecipeGroup(AQRecipes.RecipeGroups.DemoniteBarOrCrimtaneBar, 5);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}