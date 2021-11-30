using AQMod.Assets.ItemOverlays;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class Globebulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), getGlowmaskColor, drawInventory: true), item.type);
        }

        private static Color getGlowmaskColor()
        {
            return Color.Lerp(new Color(95, 80, 20, 0), new Color(0, 0, 0, 0), ((float)Math.Sin(Main.GlobalTime * 15f) + 1f) * 0.25f + 0.25f);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 1);
            item.useTime = 30;
            item.useAnimation = 30;
            item.mana = 50;
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
                AQMod.dayrateIncrease += 24;
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 9);
            r.AddIngredient(ItemID.CrystalShard, 15);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}