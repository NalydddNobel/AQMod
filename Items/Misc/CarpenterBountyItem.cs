using Aequus.Common.Building;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Misc {
    public class CarpenterBountyItem : ModItem {
        public string bountyName { get; private set; }
        public string bountyMod { get; private set; }

        private string bountyFullName;
        public string BountyFullName {
            get => bountyFullName;
            set {
                bountyFullName = value;
                if (!value.Contains('/'))
                    return;
                var split = BountyFullName.Split('/');
                bountyName = split[1];
                bountyMod = split[0];
            }
        }

        public virtual string BountyName => Language.GetTextValue($"Mods.{bountyMod}.CarpenterBounty.{bountyName}");
        public string BountyFancyName => "~ " + BountyName + " ~";
        public virtual string BountyDescription => Language.GetTextValue($"Mods.{bountyMod}.CarpenterBounty.{bountyName}.Description");
        public virtual string BountyRequirements {
            get {
                if (CarpenterSystem.TryGetBounty(bountyMod, bountyName, out var bounty)) {
                    var textValue = "";
                    foreach (var s in bounty.steps) {
                        var key = s.GetStepText(bounty);
                        if (!string.IsNullOrEmpty(key)) {
                            if (!string.IsNullOrEmpty(textValue))
                                textValue += "\n";
                            textValue += Language.GetTextValue(key);
                        }
                    }
                    if (!string.IsNullOrEmpty(textValue))
                        return textValue;
                }
                return "n/a";
            }
        }
        public string BountyFancyRequirements {
            get {
                string requirementText = BountyRequirements;
                var split = requirementText.Split('\n');
                requirementText = "";
                for (int i = 0; i < split.Length; i++) {
                    if (requirementText != "")
                        requirementText += "\n";
                    requirementText += TextHelper.ColorCommand(">", Color.Lerp(Color.Yellow, Color.White, 0.4f)) + " " + split[i];
                }
                return requirementText;
            }
        }
        public virtual string BountyTexture => $"{bountyMod}/UI/CarpenterUI/Blueprints/" + bountyName;

        protected override bool CloneNewInstances => false;

        public override ModItem Clone(Item newEntity) {
            var b = (CarpenterBountyItem)base.Clone(newEntity);
            b.BountyFullName = BountyFullName;
            return b;
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2, silver: 50);
            try {
                if (CarpenterSystem.BountiesByID != null)
                    BountyFullName = CarpenterSystem.BountiesByID[0].FullName;
            }
            catch {
                BountyFullName = "Aequus/None";
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            int index = tooltips.GetIndex("Tooltip#");
            tooltips.Insert(index, new TooltipLine(Mod, "Bounty", BountyFancyName));

            tooltips.Insert(index + 1, new TooltipLine(Mod, "BountyDescription", BountyDescription));

            string requirementText = BountyRequirements;
            var split = requirementText.Split('\n');
            string arrow = TextHelper.ColorCommand(">", Color.Lerp(Color.Yellow, Color.White, 0.4f));
            for (int i = 0; i < split.Length; i++) {
                tooltips.Insert(index + 2 + i, new TooltipLine(Mod, "BountyRequirements" + i, $"{arrow} {split[i]}"));
            }

            if (!Item.buy || Main.npcShop != -1)
                return;

            var ttLine = AequusItem.GetPriceTooltipLine(Main.LocalPlayer, Item);

            ttLine.Text = TextHelper.GetTextValueWith("Chat.Carpenter.UI.PurchaseBounty", new { Coins = TextHelper.ColorCommand(ttLine.Text, Colors.AlphaDarken(ttLine.OverrideColor.GetValueOrDefault(Color.White))) });
            ttLine.OverrideColor = null;
            tooltips.Insert(tooltips.GetIndex("Price"), ttLine);
        }

        public override void NetSend(BinaryWriter writer) {
            if (string.IsNullOrWhiteSpace(BountyFullName)) {
                writer.Write(false);
                return;
            }
            writer.Write(true);
            writer.Write(BountyFullName);
        }

        public override void NetReceive(BinaryReader reader) {
            if (reader.ReadBoolean()) {
                BountyFullName = reader.ReadString();
            }
        }

        public override void SaveData(TagCompound tag) {
            if (!string.IsNullOrWhiteSpace(BountyFullName)) {
                tag["Bounty"] = BountyFullName;
            }
        }

        public override void LoadData(TagCompound tag) {
            BountyFullName = "None";
            if (tag.ContainsKey("Bounty")) {
                BountyFullName = tag.GetString("Bounty");
            }
        }
    }
}