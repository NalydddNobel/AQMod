using AQMod.Dusts;
using AQMod.Effects;
using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Potions.Special
{
    public class MoliteTag : ConcoctionResult
    {
        public override void SetPotion()
        {
            base.SetPotion();
            item.buffTime *= 2;
            item.value *= 2;
            item.noUseGraphic = true;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item3;
            item.rare = ItemRarityID.Green;
            item.consumable = true;
            if (original == null)
            {
                original = new Item();
                original.SetDefaults(ItemID.RegenerationPotion);
            }
            SetPotion();
        }

        public override bool CanUseItem(Player player)
        {
            return ItemLoader.CanUseItem(original, player);
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var color = BuffColorCache.GetColorFromItemID(original.type);
                color.A = 0;
                var color2 = color * 4;
                var rectangle = new Rectangle((int)player.position.X - 2, (int)player.position.Y - 2, player.width + 4, player.height + 4);
                var center = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
                int type = ModContent.DustType<MonoDust>();
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, ModContent.DustType<MonoSparkleDust>(),
                        0f, 0f, 0, color2, Main.rand.NextFloat(0.7f, 1.2f));
                    var velocity = Vector2.Normalize(center - Main.dust[d].position + new Vector2(0f, -4f)) * 8f;
                    Main.dust[d].velocity = velocity;
                }
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, type, 0f, 0f, 0, color, Main.rand.NextFloat(0.9f, 2.1f));
                    var velocity = Vector2.Normalize(center - Main.dust[d].position) * Main.rand.NextFloat(2f, 4f);
                    Main.dust[d].velocity = velocity;
                }
                color *= 0.4f;
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, type, 0f, 0f, 0, color, Main.rand.NextFloat(0.4f, 1f));
                    var velocity = Vector2.Normalize(center - Main.dust[d].position) * Main.rand.NextFloat(2f, 12f);
                    Main.dust[d].velocity = velocity;
                }
            }
            return ItemLoader.UseItem(original, player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (original == null)
            {
                return true;
            }
            var item2 = original;

            position -= Main.inventoryBackTexture.Size() * Main.inventoryScale / 2f;

            var drawData = InvUI.GetDrawData(position, item2);

            position += frame.Size() * drawData.scale / 2f;

            drawData = InvUI.GetDrawData(position, item2);

            position = drawData.drawPos;
            frame = drawData.frame;

            drawColor = drawData.drawColor;
            origin = drawData.origin;
            scale = drawData.scale * drawData.scale2;

            var auraClr = BuffColorCache.GetColorFromItemID(original.type);
            auraClr *= 0.5f + AQUtils.Wave(Main.GlobalTime * 4f, -0.1f, 0.1f);
            auraClr.A = 0;

            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(-2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(0f, 2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(0f, -2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);

            int originalContext = InvUI.Hooks.CurrentSlotContext;
            InvUI.Hooks.CurrentSlotContext = -1;
            if (ItemLoader.PreDrawInInventory(original, spriteBatch, position, frame, drawColor, itemColor, origin, scale))
            {
                spriteBatch.Draw(Main.itemTexture[original.type], position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.itemTexture[original.type], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 4f, 0f, 0.4f), 0f, origin, scale, SpriteEffects.None, 0f);
            }
            ItemLoader.PostDrawInInventory(original, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            InvUI.Hooks.CurrentSlotContext = originalContext;

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (original == null)
            {
                return false;
            }

            var item2 = original;
            item2.position = item.position;
            var auraClr = BuffColorCache.GetColorFromItemID(original.type);
            auraClr *= 0.75f;
            auraClr.A = 0;
            var position = AQItem.WorldDrawPos(item2);
            var frame = new Rectangle(0, 0, Main.itemTexture[original.type].Width, Main.itemTexture[original.type].Height);
            var origin = frame.Size() / 2f;

            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(-2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(0f, 2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[original.type], position + new Vector2(0f, -2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);

            if (ItemLoader.PreDrawInWorld(item2, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI))
            {
                spriteBatch.Draw(Main.itemTexture[original.type], position, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.itemTexture[original.type], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 4f, 0f, 0.4f), rotation, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (original == null)
            {
                return;
            }
            ItemLoader.PostDrawInWorld(original, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "ItemName")
                    {
                        t.text = Lang.GetItemName(original.type).Value;
                        break;
                    }
                }
                string key;
                string text;
                if (original.type < Main.maxItemTypes)
                {
                    key = "ItemTooltip." + ItemID.Search.GetName(original.type);
                    text = Language.GetTextValue(key);
                }
                else
                {
                    key = "Mods." + original.modItem.mod.Name + ".ItemTooltip." + original.modItem.Name;
                    text = Language.GetTextValue(key);
                }
                if (text != key)
                {
                    int tooltipIndex = AQItem.GetLineIndex(tooltips, "Tooltip#");
                    tooltips.Insert(tooltipIndex, new TooltipLine(mod, "OriginalPotionTooltip", text));
                }
            }
            catch (Exception e)
            {
                var aQMod = AQMod.GetInstance();
                aQMod.Logger.Error(e.Message);
                aQMod.Logger.Error(e.StackTrace);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(original.type);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            int type = reader.ReadInt32();
            original = new Item();
            original.SetDefaults(type);
            SetPotion();
        }
    }
}