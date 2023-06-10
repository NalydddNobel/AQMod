using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Vanity {
    [AutoloadGlowMask]
    public class CelesitalEightBall : ModItem {
        public string text;

        public CelesitalEightBall() {
            text = "";
        }

        public override void SetStaticDefaults() {
            AequusItem.HasWeaponCooldown.Add(Type);
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 1);
            text = "";
        }

        public override bool CanUseItem(Player player) {
            return !player.Aequus().HasCooldown;
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                text = "Mods.Aequus.EightballAnswer." + Main.rand.Next(20);
                string textValue = Language.GetTextValue(text);
                int t = CombatText.NewText(player.getRect(), new Color(Main.mouseColor.R, Main.mouseColor.G, Main.mouseColor.B, 255), 0, true);
                Main.combatText[t].text = textValue;
                Main.combatText[t].position.X = player.Center.X - FontAssets.CombatText[1].Value.MeasureString(textValue).X / 2f;
            }
            player.Aequus().SetCooldown(120, ignoreStats: true, Item);
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (string.IsNullOrWhiteSpace(text))
                return;
            tooltips.Insert(tooltips.GetIndex("Tooltip#"), new TooltipLine(Mod, "EightBall", Language.GetTextValue(text)) { OverrideColor = Color.Lerp(Main.mouseColor, Color.White, 0.25f) });
        }

        public override void NetSend(BinaryWriter writer) {
            if (string.IsNullOrWhiteSpace(text)) {
                writer.Write(false);
                return;
            }
            writer.Write(true);
            writer.Write(text);
        }

        public override void NetReceive(BinaryReader reader) {
            if (reader.ReadBoolean()) {
                text = reader.ReadString();
            }
        }

        public override void SaveData(TagCompound tag) {
            if (!string.IsNullOrWhiteSpace(text)) {
                tag["Text"] = text;
            }
        }

        public override void LoadData(TagCompound tag) {
            text = "";
            if (tag.ContainsKey("Text")) {
                text = tag.GetString("Text");
            }
        }
    }
}