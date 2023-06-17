using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Misc {
    public class CelesitalEightBall : ModItem {
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

        public const string LanguageKey = "Mods.Aequus.Misc.EightballAnswer.";
        public const int MaxIDs = 20;

        public sbyte textId;

        public static string GetTextKey(sbyte textId) {
            if (textId < 0) {
                throw new ArgumentOutOfRangeException(nameof(textId), "id value cannot be below 0. (Value: " + textId + ")");
            }
            return LanguageKey + textId;
        }

        public string GetTextKey() {
            return GetTextKey(textId);
        }

        public string GetTextValue() {
            return Language.GetTextValue(GetTextKey());
        }

        public override void SetStaticDefaults() {
            AequusItem.HasCooldown.Add(Type);
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
            return !player.Aequus().HasCooldown;
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
                var old = textId;
                while (old == textId) {
                    textId = (sbyte)Main.rand.Next(20);
                }
                PushUpCombatText(player.Top);
                string textValue = GetTextValue();
                var spawnRectangle = new Rectangle((int)player.position.X, (int)player.position.Y + 10, player.width, player.height / 3);
                var color = Color.Lerp(Main.mouseColor, Color.White, 0.5f);
                int t = CombatText.NewText(spawnRectangle, color, textValue, true);
                Main.combatText[t].position.X = player.Center.X - FontAssets.CombatText[1].Value.MeasureString(textValue).X / 2f;
                Main.combatText[t].rotation *= 0.33f;
            }
            player.Aequus().SetCooldown(50, ignoreStats: true, Item);
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (textId < 0)
                return;

            tooltips.Insert(tooltips.GetIndex("Tooltip#"), new TooltipLine(Mod, "EightBall", GetTextValue()) { OverrideColor = Color.Lerp(Main.mouseColor, Color.White, 0.5f) });
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

                if (sbyte.TryParse(numberText, NumberStyles.None, TextHelper.InvariantCulture, out sbyte parseResult)) {
                    textId = parseResult;
                }
            }

            if (tag.TryGet("id", out byte tagResult)) {
                textId = new DataConversion(tagResult).AsSignedByte;
            }
        }
    }
}