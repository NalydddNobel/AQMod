using Terraria;

namespace Aequus.Items
{
    public interface IUpdateBank
    {
        void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank);
    }
}