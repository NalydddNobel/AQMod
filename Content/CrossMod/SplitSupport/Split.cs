using Aequus.Items.Consumables.SlotMachines;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.SplitSupport
{
    internal class Split : ModSupport<Split>
    {
        public override void PostSetupContent()
        {
            ModItem modItem;

            if (Instance.TryFind("AnxiousnessPotion", out modItem))
            {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("PurifyingPotion", out modItem))
            {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("DiligencePotion", out modItem))
            {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("AttractionPotion", out modItem))
            {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
        }
    }
}