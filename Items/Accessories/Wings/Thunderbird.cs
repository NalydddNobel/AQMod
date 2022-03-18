using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class Thunderbird : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ArmorOverlays.AddWingsOverlay<Thunderbird>(new EquipThunderbirdWingsOverlay());
            }
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = AQItem.RarityDedicatedItem;
            item.value = Item.sellPrice(gold: 20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (lightColor * 0.2f).UseA(255);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var position = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Main.LocalPlayer.FX().ThunderbirdGradient.GetColor(Main.GlobalTime);
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                AQItem.DrawDeveloperTooltip(line);
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 200;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 12f;
            acceleration *= 3f;
        }
    }
}