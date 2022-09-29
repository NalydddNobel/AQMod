using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.LiquidGun
{
    [GlowMask()]
    public class LiquidGunLava : LiquidGun
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            LiquidToItemID.Add(LiquidID.Lava, Type);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            LiquidAmountTooltip(tooltips, Language.GetTextValue($"Mods.{Mod.Name}.Lava"));
        }
    }
}