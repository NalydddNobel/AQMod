using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Yoyos;

public class YoyoMiniPrefix : YoyoPrefixBase {
    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        return new[] {
                new TooltipLine(Mod, "YoyoMiniPrefix", $"-10" + Lang.tip[43].Value)
                { IsModifier = true, IsModifierBad = true, },
            };
    }

    public override void OnShoot(Player player, Item item, Projectile projectile) {
        projectile.scale *= 0.9f;
        projectile.width = (int)Math.Max(projectile.width * 0.9f, 2);
        projectile.height = (int)Math.Max(projectile.height * 0.9f, 2);
    }
}