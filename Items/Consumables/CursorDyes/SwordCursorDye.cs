using Aequus.Content.CursorDyes;
using Aequus.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.CursorDyes
{
    public class SwordCursorDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ShopQuotes.Database.GetNPC(NPCID.DyeTrader).AddModItemQuote(Type);
            CursorDyeManager.Register(Type, new TextureChangeCursor(Aequus.AssetsPath + "UI/Cursor/SwordCursor"));
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().cursorDye != CursorDyeManager.ItemIDToCursor(Type).Type;
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().cursorDye = CursorDyeManager.ItemIDToCursor(Type).Type;
            return true;
        }
    }
}