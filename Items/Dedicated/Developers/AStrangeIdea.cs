using AQMod.Assets.Graphics;
using AQMod.Assets.Graphics.SceneLayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated.Developers
{
    public class AStrangeIdea : ModItem, IDedicatedItem, ICustomPickupText
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.value = Item.sellPrice(silver: 5);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.rare = ItemRarityID.Purple;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Furniture.Paintings3x3>();
            item.placeStyle = Tiles.Furniture.Paintings3x3.AStrangeIdea;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                DedicatedItemTooltips.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }

        Color IDedicatedItem.DedicatedItemColor => new Color(160, 80, 250, 255);
        IDedicationType IDedicatedItem.DedicationType => new ContributorDedication();

        bool ICustomPickupText.OnSpawnText(Item newItem, int stack, bool noStack, bool longText)
        {
            if (Main.showItemText && Main.netMode != NetmodeID.Server)
            {
                CustomPickupTextLayer.NewText(new NarrizuulPickupDrawObject(
                    "Mods.AQMod.ItemName.AStrangeIdea",
                    Main.player[Main.myPlayer].Center + new Vector2(0f, -Main.player[Main.myPlayer].height),
                    new Vector2(0f, -10f),
                    new Color(255, 255, 255, 255), 0f, 0.125f, 120));
            }
            return true;
        }
    }
}