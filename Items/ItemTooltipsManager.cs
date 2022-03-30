using Aequus.Common.ID;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items
{
    public sealed partial class ItemTooltipsManager : GlobalItem
    {
        public static Dictionary<int, ItemDedication> Dedicated;

        public override void SetStaticDefaults()
        {
            Dedicated = new Dictionary<int, ItemDedication>()
            {
                //[ModContent.ItemType<NoonPotion>()] = new ItemDedication(new Color(200, 80, 50, 255)),
                [ModContent.ItemType<FamiliarPickaxe>()] = new ItemDedication(new Color(200, 65, 70, 255)),
                //[ModContent.ItemType<MothmanMask>()] = new ItemDedication(new Color(50, 75, 250, 255)),
                //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
                //[ModContent.ItemType<Thunderbird>()] = new ItemDedication(new Color(200, 125, 255, 255)),
                [ModContent.ItemType<Baguette>()] = new ItemDedication(new Color(187, 142, 42, 255)),
                //[ModContent.ItemType<StudiesoftheInkblot>()] = new ItemDedication(new Color(110, 110, 128, 255)),
            };
        }

        public override void Unload()
        {
            Dedicated?.Clear();
            Dedicated = null;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Dedicated.TryGetValue(item.type, out var dedication))
            {
                tooltips.Add(new TooltipLine(Mod, "DedicatedItem", Aequus.GetText("Tooltips.DedicatedItem")) { overrideColor = dedication.color });
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Aequus" && line.Name == "DedicatedItem")
            {
                DrawDedicatedTooltip(line);
                return false;
            }
            return true;
        }

        public static void DrawDedicatedTooltip(DrawableTooltipLine line)
        {
            DrawDedicatedTooltip(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.Color));
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, Color color)
        {
            DrawDedicatedTooltip(text, x, y, color);
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            float brightness = Main.mouseTextColor / 255f;
            float brightnessProgress = (Main.mouseTextColor - 190f) / (byte.MaxValue - 190f);
            color = Colors.AlphaDarken(color);
            color.A = 0;
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, coords, new Color(0, 0, 0, 255), rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y) + f.ToRotationVector2() * (brightness / 2f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.8f, rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
            {
                var coords = new Vector2(x, y) + (f + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (brightnessProgress * 3f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.2f, rotation, origin, baseScale);
            }
        }
    }
}