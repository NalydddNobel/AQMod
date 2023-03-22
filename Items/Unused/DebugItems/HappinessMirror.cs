using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused.DebugItems {
    internal class HappinessMirror : ModItem {

        public override string Texture => $"{Aequus.VanillaTexture}Item_{ItemID.MagicMirror}";

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Reflective Mirror");
            Tooltip.SetDefault("Your villagers are {Number}% happy");
            SacrificeTotal = 0;
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.color = Main.OurFavoriteColor;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (var t in tooltips) {
                if (t.Name == "Tooltip0") {

                    var happiness = AequusWorld.AverageHappiness - 1f;
                    if (happiness > 0f) {
                        happiness /= ShopHelper.HighestPossiblePriceMultiplier - 1f;
                    }
                    else {
                        happiness /= 1f - ShopHelper.LowestPossiblePriceMultiplier;
                    }
                    t.Text = t.Text.Replace("{Number}", TextHelper.ColorCommand((-(int)(happiness * 100)).ToString(), Color.LightSeaGreen * 2f, alphaPulse: true));
                    break;
                }
            }
        }
    }
}