using AQMod.Common.ID;
using AQMod.Content.CursorDyes;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.CursorDyes
{
    public class DemonicCursorDye : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.buyPrice(gold: 1);
            item.maxStack = 999;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<PlayerCursorDyes>().cursorDye != CursorDyeID.Demon;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<PlayerCursorDyes>().cursorDye = CursorDyeID.Demon;
            return true;
        }
    }
}