using Aequus.Content;
using Aequus.Graphics;
using Aequus.Items.Misc;
using Aequus.Particles.Dusts;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.BuffPotions
{
    public class StuffedConcoction : ConcoctionResult
    {
        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.RecallPotion;

        public override void SetPotion()
        {
            base.SetPotion();
            Item.buffTime *= 2;
            Item.value *= 2;
            Item.noUseGraphic = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
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

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.instance.LoadItem(original.type);
                var color = BuffColorDatabase.GetColorFromItemID(original.type);
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
            Main.instance.LoadItem(original.type);
            var item2 = original;

            position -= TextureAssets.InventoryBack.Value.Size() * Main.inventoryScale / 2f;

            var drawData = ItemSlotRenderer.GetDrawInput(item2, position);

            position += frame.Size() * drawData.scale / 2f;

            drawData = ItemSlotRenderer.GetDrawInput(item2, position);

            position = drawData.drawPos;
            frame = drawData.frame;

            drawColor = drawData.drawColor;
            origin = drawData.origin;
            scale = drawData.scale * drawData.scale2;

            var auraClr = BuffColorDatabase.GetColorFromItemID(original.type);
            auraClr *= 0.75f + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 6f, -0.1f, 0.1f);
            auraClr.A = 0;

            var itemTexture = TextureAssets.Item[original.type].Value;
            spriteBatch.Draw(itemTexture, position + new Vector2(2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(-2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(0f, 2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(0f, -2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);

            int originalContext = AequusUI.itemSlotContext;
            AequusUI.itemSlotContext = -1;
            if (ItemLoader.PreDrawInInventory(original, spriteBatch, position, frame, drawColor, itemColor, origin, scale))
            {
                spriteBatch.Draw(itemTexture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(itemTexture, position, frame, new Color(255, 255, 255, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 4f, 0f, 0.4f), 0f, origin, scale, SpriteEffects.None, 0f);
            }
            ItemLoader.PostDrawInInventory(original, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            AequusUI.itemSlotContext = originalContext;
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (original == null)
            {
                return false;
            }
            Main.instance.LoadItem(original.type);

            var item2 = original;
            item2.position = Item.position;
            var auraClr = BuffColorDatabase.GetColorFromItemID(original.type);
            auraClr *= 0.75f;
            auraClr.A = 0;
            var itemTexture = TextureAssets.Item[original.type].Value;
            var position = ItemDefaults.WorldDrawPos(item2, itemTexture);
            var frame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            var origin = frame.Size() / 2f;

            spriteBatch.Draw(itemTexture, position + new Vector2(2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(-2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(0f, 2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position + new Vector2(0f, -2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);

            if (ItemLoader.PreDrawInWorld(item2, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI))
            {
                spriteBatch.Draw(itemTexture, position, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(itemTexture, position, frame, new Color(255, 255, 255, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 4f, 0f, 0.4f), rotation, origin, scale, SpriteEffects.None, 0f);
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
                    if (t.Mod == "Terraria" && t.Name == "ItemName")
                    {
                        t.Text = AequusText.GetTextWith("ItemName.StuffedConcoctionPotionName", new
                        {
                            ItemName = Lang.GetItemName(original.type)
                        });
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
                    key = "Mods." + original.ModItem.Mod.Name + ".ItemTooltip." + original.ModItem.Name;
                    text = Language.GetTextValue(key);
                }
                if (text != key)
                {
                    int tooltipIndex = tooltips.GetIndex("Tooltip#");
                    tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "OriginalPotionTooltip", text));
                }
            }
            catch (Exception e)
            {
                var aQMod = Aequus.Instance;
                aQMod.Logger.Error(e.Message);
                aQMod.Logger.Error(e.StackTrace);
            }
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (ConcoctionDatabase.ConcoctiblePotion(ContentSamples.ItemsByType[i]))
                {
                    var r = CreateRecipe()
                        .AddIngredient(i)
                        .AddIngredient<MoonflowerPollen>()
                        .RegisterAfter(i);
                    var s = r.createItem.ModItem as StuffedConcoction;
                    s.original = ContentSamples.ItemsByType[i].Clone();
                    s.SetPotion();
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(original.type);
        }

        public override void NetReceive(BinaryReader reader)
        {
            int type = reader.ReadInt32();
            original = new Item();
            original.SetDefaults(type);
            SetPotion();
        }
    }
}