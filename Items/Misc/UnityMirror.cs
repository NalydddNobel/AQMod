using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class UnityMirror : ModItem, IUpdatePiggybank
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
        }

        private void UpdateUnityMirror(AQPlayer aQPlayer)
        {
            aQPlayer.unityMirror = true;
        }

        public override void UpdateInventory(Player player)
        {
            UpdateUnityMirror(player.GetModPlayer<AQPlayer>());
        }

        void IUpdatePiggybank.UpdatePiggyBank(Player player, int i)
        {
            UpdateUnityMirror(player.GetModPlayer<AQPlayer>());
        }
    }
}