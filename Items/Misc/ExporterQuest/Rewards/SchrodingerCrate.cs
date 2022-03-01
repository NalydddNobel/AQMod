using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.ExporterQuest.Rewards
{
    public abstract class SchrodingerCrate : ModItem
    {
        protected abstract List<int> LootTable { get; }
        public List<int> lootTable;

        public override bool CloneNewInstances => true;

        public virtual int PickTableIndex(int total)
        {
            return (int)(Main.GameUpdateCount / 30 % total);
        }

        public int GetItem()
        {
            return lootTable[PickTableIndex(lootTable.Count)];
        }

        // Addon mod support in a global item:
        // public override void SetDefaults(Item item)
        // {
        //     if (item.modItem is WoodenShrodingerCrate wooden)
        //     {
        //         wooden.lootTable.Add(ItemID.Meowmere);
        //     }
        // }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodenCrate);
            item.createTile = -1;
            item.placeStyle = 0;
            item.maxStack = 1;
            lootTable = LootTable;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(GetItem());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < 3; i++)
            {
                tooltips.Add(new TooltipLine(mod, "None" + i, " "));
            }
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (lootTable == null)
            {
                SetDefaults();
                return;
            }
            float longest = 30f;
            foreach (var l in lines)
            {
                float length = Main.fontMouseText.MeasureString(l.text).X;
                if (length > longest)
                {
                    longest = length;
                }
            }

            float x = lines[0].X + longest / 2f;
            float y = lines[lines.Count - 4].Y + 20f;

            //BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 1f;

            x -= Main.inventoryBackTexture.Width * Main.inventoryScale / 2f;
            Main.spriteBatch.Draw(Main.inventoryBackTexture, new Vector2(x, y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), Main.inventoryScale, SpriteEffects.None, 0f);

            InvUI.DrawItem(new Vector2(x, y), AQItem.GetDefault(GetItem()));
            Main.inventoryScale = oldScale;

            //Main.spriteBatch.End();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)lootTable.Count);
            foreach (int loot in lootTable)
            {
                writer.Write(loot);
            }
        }

        public override void NetRecieve(BinaryReader reader)
        {
            lootTable?.Clear();
            lootTable = new List<int>();

            byte amt = reader.ReadByte();
            for (int i = 0; i < amt; i++)
            {
                lootTable.Add(reader.ReadInt32());
            }
        }
    }
}