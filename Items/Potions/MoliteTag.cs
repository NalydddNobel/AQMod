using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Items.Recipes;
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
using Terraria.ModLoader.IO;

namespace AQMod.Items.Potions
{
    public class MoliteTag : ModItem
    {
        public struct StarbyteTagData
        {
            public readonly ushort potionItemID;
            public ushort BuffID { get; private set; }
            public uint BuffTime { get; private set; }

            public static StarbyteTagData Default => new StarbyteTagData(ItemID.ObsidianSkinPotion, Terraria.ID.BuffID.ObsidianSkin, 6000);

            public static StarbyteTagData Error => new StarbyteTagData(ItemID.ObsidianSkinPotion, ushort.MaxValue, 6000);

            internal StarbyteTagData(int item, int buffID, uint buffTime)
            {
                potionItemID = (ushort)item;
                BuffID = (ushort)buffID;
                BuffTime = buffTime;
            }

            public StarbyteTagData(int item)
            {
                potionItemID = (ushort)item;
                BuffID = 0;
                BuffTime = 0;
                SetupTag();
            }

            public static StarbyteTagData FromKey(string key)
            {
                try
                {
                    if (key[0] == '0')
                    {
                        string id = "";
                        for (int i = 2; i < key.Length - 1 && key[i] != ':' && key[i] != ';'; i++)
                        {
                            id += key[i];
                        }
                        int itemType = int.Parse(id);
                        if (itemType >= Main.maxItemTypes)
                        {
                            return Error;
                        }
                        else
                        {
                            return new StarbyteTagData(itemType);
                        }
                    }
                    else
                    {
                        string mod = "";
                        int cursor = 2;
                        for (; cursor < key.Length - 1 && key[cursor] != ':'; cursor++)
                        {
                            mod += key[cursor];
                        }
                        cursor++;
                        string name = "";
                        for (; cursor < key.Length - 1 && key[cursor] != ':' && key[cursor] != ';'; cursor++)
                        {
                            name += key[cursor];
                        }
                        var modInstance = ModLoader.GetMod(mod);
                        int itemType = modInstance.ItemType(name);
                        if (itemType < Main.maxItemTypes)
                        {
                            return Error;
                        }
                        else
                        {
                            return new StarbyteTagData(itemType);
                        }
                    }
                }
                catch
                {
                    return Error;
                }
            }

            public string GetKey()
            {
                if (potionItemID < Main.maxItemTypes)
                {
                    return "0:" + potionItemID + ";";
                }
                else
                {
                    var item = new Item();
                    item.netDefaults(potionItemID);
                    return "1:" + item.modItem.mod.Name + ":" + item.modItem.Name + ";";
                }
            }

            public bool SetupTag()
            {
                var item = new Item();
                item.netDefaults(potionItemID);
                if (item.buffType <= 0 || item.buffTime <= 0)
                    return false;
                BuffID = (ushort)item.buffType;
                BuffTime = (uint)(item.buffTime * 2);
                return true;
            }
        }

        public StarbyteTagData potion = StarbyteTagData.Default;

        public override bool CloneNewInstances => true;

        public void SetupPotionStats()
        {
            item.buffType = potion.BuffID;
            item.buffTime = (int)potion.BuffTime;
            try
            {
                var cloneItem = AQItem.GetDefault(potion.potionItemID);
                item.rare = cloneItem.rare + 1;

                item.potion = cloneItem.potion;
            }
            catch
            {
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = AQItem.Prices.PotionValue * 2;
            item.rare = ItemRarityID.Green;
            item.consumable = true;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.noUseGraphic = true;
            SetupPotionStats();
        }

        public override bool CanUseItem(Player player)
        {
            return ItemLoader.CanUseItem(AQItem.GetDefault(potion.potionItemID), player);
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var color = BuffColorCache.GetColorFromItemID(potion.potionItemID);
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
            return ItemLoader.UseItem(AQItem.GetDefault(potion.potionItemID), player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var item2 = AQItem.GetDefault(potion.potionItemID);

            position -= Main.inventoryBackTexture.Size() * Main.inventoryScale / 2f;

            var drawData = InvUI.GetDrawData(position, item2);

            position += frame.Size() * drawData.scale / 2f;

            drawData = InvUI.GetDrawData(position, item2);

            position = drawData.drawPos;
            frame = drawData.frame;

            drawColor = drawData.drawColor;
            origin = drawData.origin;
            scale = drawData.scale * drawData.scale2;

            var auraClr = BuffColorCache.GetColorFromItemID(potion.potionItemID);
            auraClr *= 0.5f + AQUtils.Wave(Main.GlobalTime * 4f, -0.1f, 0.1f);
            auraClr.A = 0;

            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(-2f, 0f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(0f, 2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(0f, -2f), frame, auraClr, 0f, origin, scale, SpriteEffects.None, 0f);

            if (ItemLoader.PreDrawInInventory(AQItem.GetDefault(potion.potionItemID), spriteBatch, position, frame, drawColor, itemColor, origin, scale))
            {
                spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 4f, 0f, 0.4f), 0f, origin, scale, SpriteEffects.None, 0f);
            }
            ItemLoader.PostDrawInInventory(AQItem.GetDefault(potion.potionItemID), spriteBatch, position, frame, drawColor, itemColor, origin, scale);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var item2 = AQItem.GetDefault(potion.potionItemID);
            item2.position = item.position;
            var auraClr = BuffColorCache.GetColorFromItemID(potion.potionItemID);
            auraClr *= 0.75f;
            auraClr.A = 0;
            var position = AQItem.WorldDrawPos(item2);
            var frame = new Rectangle(0, 0, Main.itemTexture[potion.potionItemID].Width, Main.itemTexture[potion.potionItemID].Height);
            var origin = frame.Size() / 2f;

            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(-2f, 0f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(0f, 2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position + new Vector2(0f, -2f), frame, auraClr, rotation, origin, scale, SpriteEffects.None, 0f);

            if (ItemLoader.PreDrawInWorld(item2, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI))
            {
                spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.itemTexture[potion.potionItemID], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 4f, 0f, 0.4f), rotation, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            ItemLoader.PostDrawInWorld(AQItem.GetDefault(potion.potionItemID), spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "ItemName")
                    {
                        t.text = Lang.GetItemName(potion.potionItemID).Value;
                        break;
                    }
                }
                string key;
                string text;
                if (potion.potionItemID < Main.maxItemTypes)
                {
                    key = "ItemTooltip." + ItemID.Search.GetName(potion.potionItemID);
                    text = Language.GetTextValue(key);
                }
                else
                {
                    var potionItem = new Item();
                    potionItem.netDefaults(potion.potionItemID);
                    key = "Mods." + potionItem.modItem.mod.Name + ".ItemTooltip." + potionItem.modItem.Name;
                    text = Language.GetTextValue(key);
                }
                if (text != key)
                {
                    int tooltipIndex = AQItem.LegacyGetLineIndex(tooltips, "Tooltip#");
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

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["starbyteTag"] = potion.GetKey(),
            };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("potion"))
            {
                int legacyTag = tag.GetInt("potion");
                potion = StarbyteTagData.FromKey("0:" + legacyTag);
            }
            else
            {
                string key = tag.GetString("starbyteTag");
                potion = StarbyteTagData.FromKey(key);
            }
            SetupPotionStats();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(potion.potionItemID);
            writer.Write(potion.BuffID);
            writer.Write(potion.BuffTime);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            ushort type = reader.ReadUInt16();
            ushort buffID = reader.ReadUInt16();
            uint buffTime = reader.ReadUInt32();
            potion = new StarbyteTagData(type, buffID, buffTime);
            SetupPotionStats();
        }

        private bool CustomDataCheck(Item item)
        {
            if (item.type < Main.maxItemTypes)
            {
                return true;
            }
            return !item.modItem.CloneNewInstances;
        }
        private bool BuffCheck(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.EatingUsing
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.meleeBuff[item.buffType] &&
                !AQBuff.Sets.NoStarbyteUpgrade.Contains(item.buffType) && CustomDataCheck(item);
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                try
                {
                    var item = AQItem.GetDefault(i);
                    if (BuffCheck(item))
                    {
                        StarbyteRecipe.ConstructRecipe(i, this);
                    }
                }
                catch
                {
                }
            }
        }
    }
}