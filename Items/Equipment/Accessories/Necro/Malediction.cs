using Aequus.Common;
using Aequus.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class Malediction : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.mana = 150;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (var t in tooltips) {
                if (t.Name.StartsWith("Tooltip")) {
                    t.Text = string.Format(t.Text, TextHelper.ArmorSetBonusKey);
                }
            }
        }
    }
}