﻿using Aequus.Items.Consumables.SlotMachines;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class Split : ModSupport<Split>
    {
        public override void PostSetupContent()
        {
            if (Instance.TryFind("AnxiousnessPotion", out ModItem modItem))
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