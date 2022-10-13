using Aequus.Content.CursorDyes;
using ShopQuotesMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.CursorDyes
{
    public class DemonicCursorDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            CursorDyeSystem.Register(Type, new TextureChangeCursor(Aequus.AssetsPath + "UI/Cursor/DemonCursor"));
            ModContent.GetInstance<QuoteDatabase>().AddNPC(NPCID.DyeTrader, Mod).SetQuote(Type, "Mods.Aequus.Chat.DyeTrader.ShopQuote.DemonicCursorDye");
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().CursorDye != CursorDyeSystem.ItemIDToCursor(Type).Type;
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().CursorDye = CursorDyeSystem.ItemIDToCursor(Type).Type;
            return true;
        }
    }
}