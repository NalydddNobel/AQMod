using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Content;
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

namespace AQMod.Items.BuffItems
{
    public class MoliteTag : ModItem
    {
        public override bool CloneNewInstances => true;

        public ushort potion = (ushort)ItemID.ObsidianSkinPotion;

        private Item getPotion()
        {
            var item = new Item();
            item.netDefaults(potion);
            return item;
        }

        public void SetupPotionStats()
        {
            try
            {
                var p = getPotion();
                item.buffType = p.buffType;
                item.buffTime = p.buffTime * 2;
            }
            catch
            {
                potion = (ushort)ItemID.ObsidianSkinPotion;
                item.buffType = BuffID.ObsidianSkin;
                item.buffTime = 6000;
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = AQItem.PotionValue * 2;
            item.rare = ItemRarityID.Green;
            item.consumable = true;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            SetupPotionStats();
        }

        private static readonly bool _usePotionBuffTexture = true;
        private static Dictionary<ushort, Color> _colorsCache;

        private static void TryGetColors(int item, out Color color)
        {
            if (_colorsCache == null)
                _colorsCache = new Dictionary<ushort, Color>();
            if (_colorsCache.TryGetValue((ushort)item, out var color2))
            {
                color = color2;
                return;
            }
            switch (item)
            {
                default:
                {
                    try
                    {
                        var texture = AQTextureAssets.GetItem(item);
                        var clrs = new Color[texture.Width * texture.Height];
                        texture.GetData(clrs, 0, clrs.Length);
                        int pixelPaddingX = 4;
                        if (texture.Width <= 6)
                        {
                            pixelPaddingX = 0;
                        }
                        else if (texture.Width <= 8)
                        {
                            pixelPaddingX = 2;
                        }
                        int pixelPaddingY = 4;
                        if (texture.Height <= 6)
                        {
                            pixelPaddingY = 0;
                        }
                        else if (texture.Height <= 8)
                        {
                            pixelPaddingY = 2;
                        }
                        int textureHeightHalf = texture.Height / 2;
                        Rectangle frame = new Rectangle(pixelPaddingX, textureHeightHalf + pixelPaddingY, texture.Width - pixelPaddingX * 2, textureHeightHalf - pixelPaddingY);
                        List<Color> colors = new List<Color>();
                        for (int i = frame.X; i < frame.X + frame.Width; i++)
                        {
                            for (int j = frame.Y; j < frame.Y + frame.Height; j++)
                            {
                                int index = j * texture.Width + i;
                                if (clrs[index].A == 255)
                                    colors.Add(clrs[index]);
                            }
                        }
                        var mixedColor = new Color(255, 255, 255, 255);
                        color = new Color(255, 255, 255, 255);
                        foreach (var c in colors)
                        {
                            mixedColor = Color.Lerp(color, c, 0.25f);
                        }
                        float minDarkness = mixedColor.ToVector3().Length();
                        foreach (var c in colors)
                        {
                            if (c.ToVector3().Length() >= minDarkness)
                                color = Color.Lerp(color, c, 0.25f);
                        }
                        _colorsCache.Add((ushort)item, color);
                    }
                    catch (Exception e)
                    {
                        var aQMod = AQMod.Instance;
                        aQMod.Logger.Error(e.Message);
                        aQMod.Logger.Error(e.StackTrace);

                        color = new Color(255, 255, 255, 255);
                        _colorsCache.Add((ushort)item, color);
                    }
                }
                break;

                case ItemID.ObsidianSkinPotion:
                {
                    _colorsCache.Add((ushort)item, new Color(90, 72, 168, 255));
                    color = _colorsCache[(ushort)item];
                }
                break;
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (potion <= 0 || ItemID.Sets.Deprecated[potion])
                return;

            TryGetColors(potion, out var moliteColor);
            var texture = SpriteUtils.Textures.Extras[ExtraID.MoliteBody];
            Main.spriteBatch.Draw(texture, position, frame, moliteColor, 0f, origin, scale, SpriteEffects.None, 0f);

            Texture2D potionTexture;
            if (_usePotionBuffTexture)
            {
                try
                {
                    var p = getPotion();
                    potionTexture = AQTextureAssets.GetBuff(item.buffType);
                }
                catch
                {
                    potionTexture = AQTextureAssets.GetBuff(BuffID.ObsidianSkin);
                }
            }
            else
            {
                potionTexture = AQTextureAssets.GetItem(potion);
            }
            float iconScale = 0.6f * scale;
            Vector2 drawPos = position + frame.Size() / 2f * scale + new Vector2((40f - potionTexture.Width) * iconScale * Main.inventoryScale + 4f, (40f - potionTexture.Height) * iconScale * Main.inventoryScale + 4f);
            Main.spriteBatch.Draw(potionTexture, drawPos, null, new Color(250, 250, 250), 0f, potionTexture.Size() / 2f, iconScale, SpriteEffects.None, 0f);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["potion"] = (int)potion,
            };
        }

        public override void Load(TagCompound tag)
        {
            potion = (ushort)tag.GetInt("potion");
            SetupPotionStats();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(potion);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            potion = reader.ReadUInt16();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                var p = getPotion();
                string key;
                string text;
                if (p.type < Main.maxItemTypes)
                {
                    key = "ItemTooltip." + ItemID.Search.GetName(p.type);
                    text = Language.GetTextValue(key);
                }
                else
                {
                    key = "Mods." + p.modItem.mod.Name + ".ItemTooltip." + p.modItem.Name;
                    text = Language.GetTextValue(key);
                }
                if (text != key)
                    tooltips.Add(new TooltipLine(mod, "OriginalPotionTooltip", text) { overrideColor = new Color(211, 211, 211, 255) });
            }
            catch (Exception e)
            {
                var aQMod = AQMod.Instance;
                aQMod.Logger.Error(e.Message);
                aQMod.Logger.Error(e.StackTrace);
            }
        }

        public override void AddRecipes()
        {

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                try
                {
                    Item item = new Item();
                    item.SetDefaults(i);
                    if (item.buffType > 0 && item.buffTime > 0 && item.consumable &&
                        item.useStyle == ItemUseStyleID.EatingUsing && item.buffType != BuffID.WellFed &&
                        item.healLife <= 0 && item.healMana <= 0 && !item.summon && item.type != this.item.type &&
                        !AQItem.Sets.CantBeTurnedIntoMolitePotion[item.type])
                    {
                        MolitePotionRecipe.ConstructRecipe(i, this);
                    }
                }
                catch
                {
                }
            }
        }
    }
}