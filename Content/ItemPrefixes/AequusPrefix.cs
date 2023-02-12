using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes
{
    public abstract class AequusPrefix : ModPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
        }

        public virtual void UpdateEquip(Item item, Player player)
        {
        }

        public virtual void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
        }

        protected void AddPrefixLine(List<TooltipLine> tooltips, TooltipLine line)
        {
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), line);
        }
    }
}