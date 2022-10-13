using ShopQuotesMod;
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
            ModContent.GetInstance<QuoteDatabase>().AddNPC(NPCID.DyeTrader, Mod).SetQuote(Type, "Mods.Aequus.Chat.DyeTrader.ShopQuote.CursorDyeRemover");
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().CursorDye != -1;
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().CursorDye = -1;
            return true;
        }
    }
}