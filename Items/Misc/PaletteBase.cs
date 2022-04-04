using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public abstract class PaletteBase : ModItem
    {
        protected abstract List<int> LootTable { get; }

        public virtual int PickTableIndex(int total)
        {
            return (int)(Main.GlobalTimeWrappedHourly * 2f % total);
        }

        public int GetItem()
        {
            return LootTable[PickTableIndex(LootTable.Count)];
        }

        public override void SetStaticDefaults()
        {
            this.SetResearch(10);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenCrate);
            Item.createTile = -1;
            Item.placeStyle = 0;
            Item.maxStack = 99;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), GetItem());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < 3; i++)
            {
                tooltips.Add(new TooltipLine(Mod, "None" + i, "-"));
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name.StartsWith("None"))
            {
                return false;
            }
            return true;
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            float longest = 30f;
            foreach (var l in lines)
            {
                float length = FontAssets.MouseText.Value.MeasureString(l.Text).X;
                if (length > longest)
                {
                    longest = length;
                }
            }

            float x = lines[0].X + longest / 2f;
            float y = 20f;

            foreach (var l in lines)
            {
                if (l.Mod == "Aequus" && l.Name == "None0")
                {
                    y += l.Y;
                }
            }

            //BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 1f;

            var back = TextureAssets.InventoryBack.Value;
            x -= back.Width * Main.inventoryScale / 2f;
            Main.spriteBatch.Draw(back, new Vector2(x, y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), Main.inventoryScale, SpriteEffects.None, 0f);

            //InvUI.DrawItem(new Vector2(x, y), AQItem.GetDefault(GetItem()));
            int item = GetItem();
            Main.instance.LoadItem(item);
            var itemTexture = TextureAssets.Item[item].Value;
            Main.spriteBatch.Draw(itemTexture, new Vector2(x, y) + back.Size() / 2f, null, Color.White, 0f, itemTexture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);

            Main.inventoryScale = oldScale;

            //Main.spriteBatch.End();
        }
    }
}