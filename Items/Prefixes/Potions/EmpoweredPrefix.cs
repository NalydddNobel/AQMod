using Aequus.Buffs.Misc.Empowered;
using Aequus.Items.GlobalItems;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class EmpoweredPrefix : AequusPrefix
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
            item.buffTime /= 2;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item)
        {
            return ItemToEmpoweredBuff.ContainsKey(item.type);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (AequusText.TryGetText($"Mods.Aequus.ItemTooltip.Empowered.{AequusText.ItemKeyName(item.type, Mod)}", out string text))
            {
                foreach (var tt in tooltips)
                {
                    if (tt.Name == "Tooltip0")
                    {
                        tt.Text = text;
                    }
                }
            }

            float statIncrease = 1f;
            if (BuffLoader.GetBuff(item.buffType) is EmpoweredBuffBase empoweredBuff)
            {
                statIncrease = empoweredBuff.StatIncrease;
            }
            TooltipsGlobal.PercentageModifier(statIncrease, "BuffEmpowerment", tooltips, statIncrease > 0f);
        }
    }
}