using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Tools.Misc
{
    public class CarpenterBountyItem : ModItem
    {
        public string bountyName { get; private set; }
        public string bountyMod { get; private set; }

        private string bountyFullName;
        public string BountyFullName
        {
            get => bountyFullName;
            set
            {
                bountyFullName = value;
                if (!value.Contains('/'))
                    return;
                var split = BountyFullName.Split('/');
                bountyName = split[1];
                bountyMod = split[0];
            }
        }

        public virtual string BountyName => Language.GetTextValue($"Mods.{bountyMod}.Bounty.{bountyName}");
        public string BountyFancyName => "~ " + BountyName + " ~";
        public virtual string BountyDescription => Language.GetTextValue($"Mods.{bountyMod}.Bounty.{bountyName}.Description");
        public virtual string BountyRequirements => Language.GetTextValue($"Mods.{bountyMod}.Bounty.{bountyName}.Requirements");
        public string BountyFancyRequirements
        {
            get
            {
                string requirementText = BountyRequirements;
                var split = requirementText.Split('\n');
                requirementText = "";
                for (int i = 0; i < split.Length; i++)
                {
                    if (requirementText != "")
                        requirementText += "\n";
                    requirementText += AequusText.ColorText(">", Color.Lerp(Color.Yellow, Color.White, 0.4f)) + " " + split[i];
                }
                return requirementText;
            }
        }
        public virtual string BountyTexture => $"{bountyMod}/Assets/UI/Carpenter/" + bountyName;

        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.WireKite;

        protected override bool CloneNewInstances => false;

        public override ModItem Clone(Item newEntity)
        {
            var b = (CarpenterBountyItem)base.Clone(newEntity);
            b.BountyFullName = BountyFullName;
            return b;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2, silver: 50);
            BountyFullName = "None";
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.GetIndex("Tooltip#");
            tooltips.Insert(index, new TooltipLine(Mod, "Bounty", BountyFancyName));

            tooltips.Insert(index + 1, new TooltipLine(Mod, "BountyDescription", BountyDescription));

            tooltips.Insert(index + 2, new TooltipLine(Mod, "BountyRequirements", BountyFancyRequirements));

            if (!Item.buy)
                return;

            var ttLine = Item.GetGlobalItem<AequusTooltips.TooltipsGlobal>().GetPriceTooltipLine(Main.LocalPlayer, Item);

            ttLine.Text = AequusText.GetTextWith("Chat.Carpenter.UI.PurchaseBounty", new { Coins = AequusText.ColorText(ttLine.Text, Colors.AlphaDarken(ttLine.OverrideColor.GetValueOrDefault(Color.White))) });
            ttLine.OverrideColor = null;
            if (ttLine != null)
                tooltips.Insert(index + 3, ttLine);
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (string.IsNullOrWhiteSpace(BountyFullName))
            {
                writer.Write(false);
                return;
            }
            writer.Write(true);
            writer.Write(BountyFullName);
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                BountyFullName = reader.ReadString();
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (!string.IsNullOrWhiteSpace(BountyFullName))
            {
                tag["Bounty"] = BountyFullName;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            BountyFullName = "None";
            if (tag.ContainsKey("Bounty"))
            {
                BountyFullName = tag.GetString("Bounty");
            }
        }
    }
}