using Aequus.Projectiles.Misc;
using System.Collections.Generic;

namespace Aequus.Content.ItemPrefixes.Potions;
public class SplashPrefix : PotionPrefixBase {
    public override bool Shimmerable => true;
    public override string GlintTexture => $"{this.NamespacePath()}/SplashGlint";

    public override void SetStaticDefaults() {
        // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
    }

    public override void Apply(Item item) {
        item.useStyle = ItemUseStyleID.Swing;
        item.UseSound = SoundID.Item1;
        item.shoot = ModContent.ProjectileType<SplashPotionProj>();
        item.noUseGraphic = true;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1f;
    }

    public override bool CanRoll(Item item) {
        return AequusItem.IsPotion(item);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return new TooltipLine(Mod, "PrefixSplash", TextHelper.GetTextValue("Prefixes.SplashPrefix.Ability")) { IsModifier = true, IsModifierBad = false, };
    }
}