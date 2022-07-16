using Aequus.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.CursorDyes
{
    public class CursorDyeRemover : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ShopQuotes.Database.GetNPC(NPCID.DyeTrader).AddModItemQuote(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().cursorDye != -1;
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().cursorDye = -1;
            return true;
        }
    }
}