﻿using Aequus.Common.Buffs;
using Aequus.Items;
using Aequus.Projectiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions {
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
            return item.shoot == ProjectileID.None && AequusItem.IsPotion(item);
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            return new[] { new TooltipLine(Mod, "PrefixSplash", TextHelper.GetTextValue("Prefixes.SplashPrefix.Ability"))
            { IsModifier = true, IsModifierBad = false, } };
        }
    }
}