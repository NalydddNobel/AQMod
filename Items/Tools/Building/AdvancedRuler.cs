using Aequus.Content.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Building {
    public class AdvancedRuler : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void HoldItem(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                switch (AdvancedRulerInterface.Instance.Type) {
                    case 0:
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 1;
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerGrid] = 1;
                        break;
                    case 1:
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 0;
                        break;
                }
                AdvancedRulerInterface.Instance.Enabled = true;
                AdvancedRulerInterface.Instance.Holding = true;
            }
        }

        public override void UpdateInventory(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                if (!AdvancedRulerInterface.Instance.Holding) {
                    player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 1;
                    player.builderAccStatus[Player.BuilderAccToggleIDs.RulerGrid] = 1;
                }
                AdvancedRulerInterface.Instance.Enabled = true;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (var t in tooltips) {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip")) {
                    t.Text = t.Text.FormatWith(new {
                        LEFTMOUSE = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.LeftMouseButton"), Color.Yellow, alphaPulse: true)}]",
                        RIGHTMOUSE = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.RightMouseButton"), Color.Orange.SaturationSet(0.5f) * 2f, alphaPulse: true)}]",
                        CTRL = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.ControlButton"), Color.Violet, alphaPulse: true)}]",
                        SHIFT = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.ShiftButton"), Color.Red, alphaPulse: true)}]",
                        LINE = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.Line"), Color.Lime, alphaPulse: true)}]",
                        GRID = $"[{TextHelper.ColorCommand(TextHelper.GetTextValue("ItemTooltip.AdvancedRuler.Grid"), Color.Turquoise, alphaPulse: true)}]",
                    });
                }
            }
        }
    }
}