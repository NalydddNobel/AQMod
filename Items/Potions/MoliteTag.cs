using AQMod.Assets;
using AQMod.Content;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public struct StarbytePotionTag
        {
            public readonly ushort potionItemID;
            public ushort BuffID { get; private set; }
            public uint BuffTime { get; private set; }

            public static StarbytePotionTag Default => new StarbytePotionTag(ItemID.ObsidianSkinPotion, Terraria.ID.BuffID.ObsidianSkin, 6000);

            public static StarbytePotionTag Error => new StarbytePotionTag(ItemID.ObsidianSkinPotion, ushort.MaxValue, 6000);

            internal StarbytePotionTag(int item, int buffID, uint buffTime)
            {
                potionItemID = (ushort)item;
                BuffID = (ushort)buffID;
                BuffTime = buffTime;
            }

            public StarbytePotionTag(int item)
            {
                potionItemID = (ushort)item;
                BuffID = 0;
                BuffTime = 0;
                SetupTag();
            }

            public static StarbytePotionTag FromKey(string key)
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
                            return new StarbytePotionTag(itemType);
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
                            return new StarbytePotionTag(itemType);
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

        public override bool CloneNewInstances => true;

        public StarbytePotionTag potion = StarbytePotionTag.Default;

        public void SetupPotionStats()
        {
            item.buffType = potion.BuffID;
            item.buffTime = (int)potion.BuffTime;
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

        private static readonly bool _usePotionBuffTexture = true;

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var color = StarbyteColorCache.GetColor(potion.potionItemID);
                var color2 = color * 4;
                var rectangle = new Rectangle((int)player.position.X - 2, (int)player.position.Y - 2, player.width + 4, player.height + 4);
                var center = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
                int type = ModContent.DustType<MonoDust>();
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, type, 0f, 0f, 0, color2, Main.rand.NextFloat(0.7f, 1.2f));
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
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            try
            {
                var color = StarbyteColorCache.GetColor(potion.potionItemID);
                var texture = TextureGrabber.GetItem(item.type);
                Main.spriteBatch.Draw(texture, position, frame, color, 0f, origin, scale, SpriteEffects.None, 0f);

                if (Main.keyState.IsKeyDown(Keys.LeftShift) || _usePotionBuffTexture && potion.BuffID < 0 || !_usePotionBuffTexture && potion.potionItemID <= 0 || ItemID.Sets.Deprecated[potion.potionItemID])
                    return;
                Texture2D potionTexture;
                if (_usePotionBuffTexture)
                {
                    try
                    {
                        potionTexture = TextureGrabber.GetBuff(potion.BuffID);
                    }
                    catch
                    {
                        potionTexture = TextureGrabber.GetBuff(BuffID.ObsidianSkin);
                    }
                }
                else
                {
                    try
                    {
                        potionTexture = TextureGrabber.GetItem(potion.potionItemID);
                    }
                    catch
                    {
                        potionTexture = TextureGrabber.GetItem(ItemID.ObsidianSkinPotion);
                    }
                }
                float iconScale = 0.6f * scale;
                Vector2 drawPos = position + frame.Size() / 2f * scale + new Vector2((40f - potionTexture.Width) * iconScale * Main.inventoryScale + 4f, (40f - potionTexture.Height) * iconScale * Main.inventoryScale + 4f);
                Main.spriteBatch.Draw(potionTexture, drawPos, null, new Color(250, 250, 250, 250), 0f, potionTexture.Size() / 2f, iconScale, SpriteEffects.None, 0f);
            }
            catch
            {
                var texture = Main.cdTexture;
                Vector2 drawPos = position + frame.Size() / 2f * scale;
                float intensity = 0.8f + ((float)Math.Sin(Main.GlobalTime * 3f + position.X + position.Y / 90f) + 1f) / 2f * 0.2f;
                Main.spriteBatch.Draw(texture, drawPos, null, new Color(250, 250, 250, 250), 0f, texture.Size() / 2f, intensity, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            try
            {
                var color = StarbyteColorCache.GetColor(potion.potionItemID);
                var texture = TextureGrabber.GetItem(item.type);
                var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawRotation = rotation;
                var origin = Main.itemTexture[item.type].Size() / 2;
                Main.spriteBatch.Draw(texture, drawCoordinates, drawFrame, item.GetAlpha(color), drawRotation, origin, scale, SpriteEffects.None, 0);
            }
            catch
            {
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
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
                potion = StarbytePotionTag.FromKey("0:" + legacyTag);
            }
            else
            {
                string key = tag.GetString("starbyteTag");
                potion = StarbytePotionTag.FromKey(key);
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
            potion = new StarbytePotionTag(type, buffID, buffTime);
            SetupPotionStats();
        }

        private Color GetTextColor(float time)
        {
            var color = StarbyteColorCache.GetColor(potion.potionItemID);
            color = Color.Lerp(color, new Color(255, 255, 255, 255), Main.mouseTextColor / 510f);
            return color;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
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
                    tooltips.Add(new TooltipLine(mod, "OriginalPotionTooltip", text) { overrideColor = GetTextColor(Main.GlobalTime * 6f) });
            }
            catch (Exception e)
            {
                var aQMod = AQMod.Instance;
                aQMod.Logger.Error(e.Message);
                aQMod.Logger.Error(e.StackTrace);
            }
        }

        public static bool CanBuffBeTurnedIntoMolitePotion(int buff)
        {
            return buff != BuffID.WellFed && buff != BuffID.Tipsy && !AQBuff.Sets.IsFoodBuff[buff];
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                try
                {
                    var item = new Item();
                    item.SetDefaults(i);
                    if (item.buffType > 0 && item.buffTime > 0 && item.consumable &&
                        item.useStyle == ItemUseStyleID.EatingUsing && CanBuffBeTurnedIntoMolitePotion(item.buffType) &&
                        item.healLife <= 0 && item.healMana <= 0 && !item.summon && item.type != this.item.type)
                    {
                        AQRecipes.r_MolitePotionRecipe.ConstructRecipe(i, this);
                    }
                }
                catch
                {
                }
            }
        }
    }
}