using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Town.CarpenterNPC.Paint
{
    public class OmniPaint : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
            Item.consumable = false;
            Item.maxStack = 1;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.myPlayer == player.whoAmI && !Main.playerInventory)
            {
                OmniPaintUI.Instance.Enabled = true;
            }
            Item.paint = player.GetModPlayer<OmniPaintPlayer>().selectedPaint;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }
    }
}