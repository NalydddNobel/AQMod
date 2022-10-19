using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class EmpoweredPrefix : ModPrefix
    {
        public static Dictionary<int, int> ItemToEmpoweredBuff { get; private set; }

        public override void Load()
        {
            ItemToEmpoweredBuff = new Dictionary<int, int>();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item)
        {
            if (ItemToEmpoweredBuff.TryGetValue(item.type, out int buff))
            {
                item.buffType = buff;
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item)
        {
            return ItemToEmpoweredBuff.ContainsKey(item.type);
        }
    }
}
