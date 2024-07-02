using Aequus.Core.Entities.Items.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Items.Vanity;

[LegacyName("CelesitalEightBall")]
public class CelestialEightBall : ModItem, ICooldownItem {
    [StructLayout(LayoutKind.Explicit)]
    private struct DataConversion {
        [FieldOffset(0)]
        public byte AsByte;
        [FieldOffset(0)]
        public sbyte AsSignedByte;

        public DataConversion(byte value) {
            AsSignedByte = 0;
            AsByte = value;
        }

        public DataConversion(sbyte value) {
            AsByte = 0;
            AsSignedByte = value;
        }
    }

    public const int MaxIDs = 20;

    public sbyte textId;

    public int CooldownTime => 50;

    public bool ShowCooldownTip => false;

    public override void SetStaticDefaults() {
        for (int i = 0; i < MaxIDs; ++i) {
            XLanguage.RegisterKey(this.GetLocalizationKey($"Answer.{i}"));
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Green;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = Item.sellPrice(gold: 1);
        textId = -1;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public static void PushUpCombatText(Vector2 position) {
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

    public override bool? UseItem(Player player) {
        if (Main.myPlayer == player.whoAmI) {
            sbyte old = textId;
            while (old == textId) {
                textId = (sbyte)Main.rand.Next(20);
            }

            PushUpCombatText(player.Top);
            string textValue = this.GetLocalizedValue("Answer." + textId);
            var spawnRectangle = new Rectangle((int)player.position.X, (int)player.position.Y + 10, player.width, player.height / 3);
            var color = Color.Lerp(Main.mouseColor, Color.White, 0.5f);
            int t = CombatText.NewText(spawnRectangle, color, textValue, true);
            Main.combatText[t].position.X = player.Center.X - FontAssets.CombatText[1].Value.MeasureString(textValue).X / 2f;
            Main.combatText[t].rotation *= 0.33f;
        }

        this.SetCooldown(player);
        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (textId < 0) {
            return;
        }

        tooltips.AddTooltip(new TooltipLine(Mod, "EightBall", this.GetLocalizedValue("Answer." + textId)) { OverrideColor = Color.Lerp(Main.mouseColor, Color.White, 0.5f) });
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(textId);
    }

    public override void NetReceive(BinaryReader reader) {
        textId = reader.ReadSByte();
    }

    public override void SaveData(TagCompound tag) {
        tag["id"] = new DataConversion(textId).AsByte;
    }

    public override void LoadData(TagCompound tag) {
        textId = -1;

        if (tag.TryGet("Text", out string legacyText) && !string.IsNullOrWhiteSpace(legacyText)) {
            int index = legacyText.LastIndexOf('.');
            string numberText = "";
            for (index++; index < legacyText.Length; index++) {
                if (!char.IsNumber(legacyText[index])) {
                    break;
                }

                numberText += legacyText[index];
            }

            if (sbyte.TryParse(numberText, NumberStyles.None, CultureInfo.InvariantCulture, out sbyte parseResult)) {
                textId = parseResult;
            }
        }

        if (tag.TryGet("id", out byte tagResult)) {
            textId = new DataConversion(tagResult).AsSignedByte;
        }
    }
}