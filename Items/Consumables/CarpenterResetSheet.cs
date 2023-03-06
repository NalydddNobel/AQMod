using Aequus.Content.Town.CarpenterNPC.Quest;
using Aequus.Content.Town.CarpenterNPC.Quest.Bounties;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class CarpenterResetSheet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 1);
            Item.consumable = true;
            Item.UseSound = SoundID.Item92;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                CarpenterSystem.ResetBounties();
            player.GetModPlayer<CarpenterBountyPlayer>().collectedBounties?.Clear();
            player.GetModPlayer<CarpenterBountyPlayer>().SelectedBounty = -1;
            return true;
        }
    }
}