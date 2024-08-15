using Aequus.Common.Items;
using Aequus.Common.Utilities.Helpers;
using Aequus.Systems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Items.Vanity;

public class CelesitalEightBall : ModItem, ICooldownItem {
    const byte Invalid = byte.MaxValue;

    const string TagKey = "Text2";
    const byte MaxText = 20;
    static readonly LocalizedText[] Text = new LocalizedText[MaxText];

    byte _textId = Invalid;

    int ICooldownItem.CooldownTime => 60;

    static Color TextColor => Color.Lerp(Main.mouseColor, Color.White, 0.5f);

    public override void SetStaticDefaults() {
        for (int i = 0; i < Text.Length; i++) {
            Text[i] = this.GetLocalization($"Result.{i}");
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Green;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = Item.sellPrice(silver: 20);
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        _textId = Invalid;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool? UseItem(Player player) {
        if (Main.myPlayer == player.whoAmI) {
            var old = _textId;
            while (old == _textId) {
                _textId = (byte)Main.rand.Next(Text.Length);
            }
            PushUpCombatText(player.Top);
            string textValue = Text[_textId].Value;
            var spawnRectangle = new Rectangle((int)player.position.X, (int)player.position.Y + 10, player.width, player.height / 3);
            var color = Color.Lerp(Main.mouseColor, Color.White, 0.5f);
            int t = CombatText.NewText(spawnRectangle, color, textValue, true);
            Main.combatText[t].position.X = player.Center.X - FontAssets.CombatText[1].Value.MeasureString(textValue).X / 2f;
            Main.combatText[t].rotation *= 0.33f;
        }
        this.SetCooldown(player);
        return true;
    }
    static void PushUpCombatText(Vector2 position) {
        var r = new Rectangle((int)position.X - 300, (int)position.Y - 160, 500, 180);
        float highestY = 0f;
        float lowestY = float.MaxValue;
        int combatTextId = -1;

    Reiterate:
        for (int i = 0; i < Main.maxCombatText; i++) {
            if (!Main.combatText[i].active || Main.combatText[i].position.Y <= highestY || Main.combatText[i].position.Y >= lowestY || !r.Contains(Main.combatText[i].position.ToPoint())) {
                continue;
            }

            combatTextId = i;
            highestY = Main.combatText[i].position.Y;
        }

        if (combatTextId != -1) {
            var combatText = Main.combatText[combatTextId];
            combatText.velocity.Y = Math.Min(combatText.velocity.Y, -8f);

            combatTextId = -1;
            position.Y = combatText.position.Y;
            highestY = 0f;
            lowestY = combatText.position.Y - 1f;
            goto Reiterate;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (!Text.IndexInRange(_textId)) {
            return;
        }

        string text = Text[_textId].Value;
        Color textColor = TextColor;
        tooltips.AddTooltip(new TooltipLine(Mod, "EightBall", text) { OverrideColor = textColor });
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(_textId);
    }

    public override void NetReceive(BinaryReader reader) {
        _textId = reader.ReadByte();
    }

    public override void SaveData(TagCompound tag) {
        if (_textId != Invalid) {
            tag[TagKey] = _textId;
        }
    }

    public override void LoadData(TagCompound tag) {
        _textId = tag.GetOrDefault(TagKey, Invalid);
        ParseLegacySaveData(tag);
    }

    void ParseLegacySaveData(TagCompound tag) {
        // Legacy string id.
        if (tag.TryGet("Text", out string legacyText) && !string.IsNullOrWhiteSpace(legacyText)) {
            int index = legacyText.LastIndexOf('.');
            string numberText = "";
            for (index++; index < legacyText.Length; index++) {
                if (!char.IsNumber(legacyText[index])) {
                    break;
                }

                numberText += legacyText[index];
            }

            if (byte.TryParse(numberText, NumberStyles.None, TextHelper.InvariantCulture, out byte parseResult)) {
                _textId = parseResult;
            }
        }

        // Legacy sbyte id.
        if (tag.TryGet("id", out byte legacyId)) {
            sbyte legacyIdValue = new BitsConvert.Bit8(legacyId).ToSByte;
            if (legacyIdValue < 0) {
                _textId = Invalid;
            }
            else {
                _textId = (byte)legacyIdValue;
            }
        }
    }
}