using AequusRemake.Core;
using AequusRemake.Core.CodeGeneration;
using AequusRemake.Core.Util.Helpers;
using System.Collections.Generic;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace AequusRemake.Content.Items.Accessories.ValentinesRing;

[Gen.AequusPlayer_ResetField<string>("accGifterRing")]
public class ValentineRing : ModItem {
    public const string SAVE_KEY = "Gifter";

    public string gifterName;

    public static int LifeRegenerationAmount { get; set; } = 2;
    public static float DamageAmount { get; set; } = 0.1f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Decimals(LifeRegenerationAmount / 2f), ALanguage.Percent(DamageAmount));

    public static LocalizedText GiftTooltip { get; private set; }
    public static LocalizedText AltTooltip { get; private set; }

    public bool HasGifter => !string.IsNullOrEmpty(gifterName);

    public override void SetStaticDefaults() {
        AltTooltip = this.GetLocalization("Tooltip2");
        GiftTooltip = this.GetLocalization("GiftTooltip");
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;

        // Janky
        if (!Main.gameMenu && Main.player.IndexInRange(Main.myPlayer) && Main.LocalPlayer != null) {
            gifterName = Main.LocalPlayer.name;
        }
    }

    public void SetGifter(Player player) {
        gifterName = player.name.Trim();
    }

    public override void HoldItem(Player player) {
        if (!HasGifter) {
            SetGifter(player);
            Item.NetStateChanged();
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (gifterName != Main.LocalPlayer.name) {
            foreach (TooltipLine t in tooltips) {
                if (t.Name == "Tooltip0") {
                    t.Text = AltTooltip.Value;
                }
            }
        }
        if (HasGifter) {
            tooltips.AddTooltip(new TooltipLine(Mod, "ValentineTooltip", GiftTooltip.Format(ColorTagProvider.Color(Colors.AlphaDarken(Color.Pink), gifterName))));
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (!HasGifter) {
            SetGifter(player);
            Item.NetStateChanged();
        }

        player.GetModPlayer<AequusPlayer>().accGifterRing = gifterName;
    }

    public override void SaveData(TagCompound tag) {
        if (HasGifter) {
            tag[SAVE_KEY] = gifterName;
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet(SAVE_KEY, out string setGifterName)) {
            gifterName = setGifterName;
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(gifterName ?? string.Empty);
    }

    public override void NetReceive(BinaryReader reader) {
        gifterName = reader.ReadString();
    }
}
