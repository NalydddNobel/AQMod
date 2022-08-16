using Aequus.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Builder
{
    public class AdvancedRuler : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                switch (AdvancedRulerInterface.Instance.Type)
                {
                    case 0:
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 1;
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerGrid] = 1;
                        break;
                    case 1:
                        player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 0;
                        break;
                }
                AdvancedRulerInterface.Instance.Enabled = true;
                AdvancedRulerInterface.Instance.Holding = true;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (!AdvancedRulerInterface.Instance.Holding)
                {
                    player.builderAccStatus[Player.BuilderAccToggleIDs.RulerLine] = 1;
                    player.builderAccStatus[Player.BuilderAccToggleIDs.RulerGrid] = 1;
                }
                AdvancedRulerInterface.Instance.Enabled = true;
            }
        }
    }
}