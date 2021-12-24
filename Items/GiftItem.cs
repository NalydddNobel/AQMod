using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Items
{
    public class GiftItem : ModItem
    {
        public byte GiftType { get; private set; }

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
            GiftType = 1;
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
                    player.QuickSpawnItem(ModContent.ItemType<Weapons.Magic.Narrizuul>());
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

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            switch (GiftType)
            {
                case 1:
                {
                    var rand = new UnifiedRandom(Main.LocalPlayer.name.GetHashCode());
                    Color bodyColor = new Color(rand.Next(10), 255, rand.Next(50), 255);
                    Color bowColor = new Color(255, rand.Next(10), rand.Next(50), 255);
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