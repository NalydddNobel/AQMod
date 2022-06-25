using Aequus.Content.CursorDyes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.CursorDyes
{
    public class DemonicCursorDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            CursorDyeManager.Register(Type, new TextureChangeCursor(Aequus.AssetsPath + "UI/Cursor/DemonCursor"));
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.rare = ItemRarityID.Orange;
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