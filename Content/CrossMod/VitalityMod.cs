using Aequus.Items.Accessories.Vanity.Cursors;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class VitalityMod : ModSupport<VitalityMod>
    {
    }

    internal class VitalityModSupportGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return VitalityMod.IsLoadingEnabled();
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (VitalityMod.Instance == null)
                return;
            if (VitalityMod.Instance.TryFind<ModNPC>("Miner", out var miner) && type == miner.Type)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordCursor>());
            }
        }
    }
}