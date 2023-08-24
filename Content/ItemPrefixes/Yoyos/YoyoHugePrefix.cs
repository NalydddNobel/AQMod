using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Yoyos;

public class YoyoHugePrefix : YoyoPrefixBase {
    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        return new[] {
                new TooltipLine(Mod, "YoyoHugePrefix", $"+10" + Lang.tip[43].Value)
                { IsModifier = true, IsModifierBad = false, },
            };
    }

    public override void OnShoot(Player player, Item item, Projectile projectile) {
        projectile.scale *= 1.1f;
        projectile.width = (int)(projectile.width * 1.1f);
        projectile.height = (int)(projectile.height * 1.1f);
    }
}