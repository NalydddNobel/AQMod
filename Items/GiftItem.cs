using AQMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Items
{
    public class GiftItem : ModItem
    {
        public byte GiftType { get; private set; }

        public override bool CloneNewInstances => true;

        public static int SpawnGiftItem(Rectangle rect, byte type = 0)
        {
            int i = Item.NewItem(rect, ModContent.ItemType<GiftItem>());
            if (i == -1)
                return i;
            var item = Main.item[i];
            ((GiftItem)item.modItem).GiftType = type;
            return i;
        }

        public static Item CreateGiftItem(byte type = 0)
        {
            var item = new Item();
            item.SetDefaults(ModContent.ItemType<GiftItem>());
            ((GiftItem)item.modItem).GiftType = type;
            return item;
        }

        public static GiftItem CreateGift(byte type = 0)
        {
            return new GiftItem() { GiftType = type };
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick()
        {
            return GiftType != 0;
        }

        public override void RightClick(Player player)
        {
            switch (GiftType)
            {
                case 1:
                    {
                        bool nice = player.name.ToLower() == "nalyd t.";
                        if (!nice)
                        {
                            nice = new UnifiedRandom(Main.LocalPlayer.name.GetHashCode()).NextBool(160);
                        }
                        if (nice)
                        {
                            if (Main.myPlayer == player.whoAmI) // I am pretty sure this hook runs on the client that right clicks this item but whatever.
                            {
                                int text = CombatText.NewText(player.getRect(), new Color(230, 230, 255, 255), 0, true);
                                Main.combatText[text].text = Language.GetTextValue("Mods.AQMod.XmasGift.Nice");
                            }
                            player.QuickSpawnItem(ModContent.ItemType<Narrizuul>());
                        }
                        else
                        {
                            if (Main.myPlayer == player.whoAmI)
                            {
                                int text = CombatText.NewText(player.getRect(), new Color(230, 230, 255, 255), 0, true);
                                Main.combatText[text].text = Language.GetTextValue("Mods.AQMod.XmasGift.Naughty");
                            }
                            player.QuickSpawnItem(ItemID.Coal);
                        }
                    }
                    break;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            switch (GiftType)
            {
                case 1:
                    {
                        tooltips.Add(new TooltipLine(mod, "Xmas", Language.GetTextValue("Mods.AQMod.XmasGift.Tooltip")) { overrideColor = new Color(100, 40, 128, 255) });
                    }
                    break;
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["GiftType"] = GiftType,
            };
        }

        public override void Load(TagCompound tag)
        {
            GiftType = tag.GetByte("GiftType");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(GiftType);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            GiftType = reader.ReadByte();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            switch (GiftType)
            {
                case 1:
                    {
                        var rand = new UnifiedRandom(Main.LocalPlayer.name.GetHashCode());
                        Color bodyColor = new Color(rand.Next(10) + 100, 255, rand.Next(50) + 100, 255);
                        Color bowColor = new Color(255, rand.Next(10) + 10, rand.Next(50) + 25, 255);
                        if (rand.NextBool())
                        {
                            var oldBody = bodyColor;
                            bodyColor = bowColor;
                            bowColor = oldBody;
                        }
                        var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + Main.itemTexture[item.type].Width / 2 + item.width / 2 - Main.itemTexture[item.type].Width / 2, item.position.Y - Main.screenPosition.Y + Main.itemTexture[item.type].Height / 2 + item.height - Main.itemTexture[item.type].Height + 2f);
                        var drawFrame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
                        var origin = Main.itemTexture[item.type].Size() / 2;

                        Main.spriteBatch.Draw(Main.itemTexture[item.type], drawCoordinates, drawFrame, bodyColor, rotation, origin, scale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_Bow")), drawCoordinates, drawFrame, bowColor, rotation, origin, scale, SpriteEffects.None, 0f);
                    }
                    return false;
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            switch (GiftType)
            {
                case 1:
                    {
                        var rand = new UnifiedRandom(Main.LocalPlayer.name.GetHashCode());
                        Color bodyColor = new Color(rand.Next(10) + 100, 255, rand.Next(50) + 100, 255);
                        Color bowColor = new Color(255, rand.Next(10) + 10, rand.Next(50) + 25, 255);
                        if (rand.NextBool())
                        {
                            var oldBody = bodyColor;
                            bodyColor = bowColor;
                            bowColor = oldBody;
                        }
                        Main.spriteBatch.Draw(Main.itemTexture[item.type], position, frame, bodyColor, 0f, origin, scale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_Bow")), position, frame, bowColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                    return false;
            }
            return true;
        }
    }
}