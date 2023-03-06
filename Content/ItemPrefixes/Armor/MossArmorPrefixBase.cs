using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor
{
    public abstract class MossArmorPrefixBase : AequusPrefix
    {
        public override bool Shimmerable => true;

        public abstract int MossItem { get; }

        public override bool CanRoll(Item item)
        {
            return !item.vanity && item.defense > 0 && (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0);
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            AddPrefixLine(tooltips, new TooltipLine(Mod, "MossPrefix", $"Empowered with: {Lang.GetItemNameValue(MossItem)}") { OverrideColor = Color.Lerp(Color.Orange, Color.White, 0.8f) });
        }
    }
}